// Copyright 2017 Dishant Langayan
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Author:
//  Dishant Langayan <dishant.langayan@solace.com>
//

using NLog;
using Solace.Labs.Excel.SolaceXLCore.Json;
using SolaceSystems.Solclient.Messaging;
using System;
using System.Collections.Generic;

namespace Solace.Labs.Excel.SolaceXLCore
{
    /// <summary>
    /// Singleton class for managing communication with Solace Messaging Routers.
    /// </summary>
    public class SolaceTransport : IObservable<SolaceMessageEvent>
    {
        private Logger Logger { get; } = ApplicationLogging.CreateLogger<SolaceTransport>();

        private static SolaceTransport instance;

        private SolaceTransport()
        {
            // Create the message convertor
            msgConvertorFactory = new JsonMsgConvertorFactory();
            msgConvertor = msgConvertorFactory.CreateConvertor();

            // List of observers
            observers = new List<IObserver<SolaceMessageEvent>>();
        }

        public static SolaceTransport Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SolaceTransport();
                }
                return instance;
            }
        }

        #region Members
        // Solace API
        private ISession session;
        private IContext context;
        private volatile bool isConnected = false;
        private bool isInitialized = false;

        private List<IObserver<SolaceMessageEvent>> observers;
        private SolaceConfiguration solaceConfig;

        private MsgConvertor msgConvertor;
        private MsgConvertorFactory<JsonMsgConvertor> msgConvertorFactory;
        #endregion

        #region "Transport Methods"
        public bool IsConnected { get { return isConnected; } }

        public SolaceConfiguration Config
        {
            get { return solaceConfig; }
            set { solaceConfig = value; }
        }

        public bool Connect(SolaceConfiguration config)
        {
            if (isConnected)
            {
                // ignore duplicate calls to connect
                return true;
            }
            if (!isInitialized)
            {
                Logger.Trace("Initializing the Solace .NET API");
                // Initialize the API
                ContextFactoryProperties cfp = new ContextFactoryProperties();
                // Set logging if enabled
                if (config.AppLogLevel != LogLevel.Off)
                {
                    SolLogLevel level = GetSolaceLogLevel(config.AppLogLevel);
                    cfp.LogDelegate += ApiLogger;
                    cfp.SolClientLogLevel = GetSolaceLogLevel(config.AppLogLevel);
                }
                ContextFactory.Instance.Init(cfp);
            }

            // Save the Config
            solaceConfig = config;

            // Context
            context = ContextFactory.Instance.CreateContext(new ContextProperties(), null);

            // Session Properties
            SessionProperties properties = new SessionProperties()
            {
                Host = config.Host,
                VPNName = config.MsgVpn,
                UserName = config.Username,
                Password = config.Password,
                ClientName = config.ClientName,
                ConnectRetries = config.ConnectRetries,
                ConnectRetriesPerHost = config.ConnectRetriesPerHost,
                ReconnectRetries = config.ReconnectRetries,
                ReconnectRetriesWaitInMsecs = config.ReconnectRetriesWaitInMsecs,
                ReapplySubscriptions = true
            };

            // Session
            session = context.CreateSession(properties, HandleMessage, HandleSessionEvent);

            // Connect
            ReturnCode rc = session.Connect();
            if (rc != ReturnCode.SOLCLIENT_OK)
            {
                Logger.Error("Failed to connect Solace session - ensure configuration " +
                    "is correct and Solace Message Router is accessible.");
                return false;
            }

            // We now have an active Solace connection
            isConnected = true;
            Logger.Info("Connected to Solace Host: {0} on Message-VPN: {1}", config.Host, config.MsgVpn);

            return isConnected;
        }

        /// <summary>
        /// Attempt to disconnect Solace connect gracefully and uninitializes the API.
        /// </summary>
        public void Disconnect()
        {
            Logger.Trace("Disconnecting from Solace...");
            // Disconnect session
            if (session != null)
            {
                session.Disconnect();
                session.Dispose();
                session = null;
            }
            if (context != null)
            {
                context.Dispose();
                context = null;
            }
            isConnected = false;

            // Cleanup API resources
            ContextFactory.Instance.Cleanup();
            isInitialized = false;

            Logger.Info("Disconnected from Solace gracefully");
        }

        public void Subscribe(string topicName)
        {
            if (string.IsNullOrEmpty(topicName))
                throw new ArgumentNullException("Topic name must not be null");

            Logger.Trace("Subscribing to topic: {0}", topicName);

            ITopic topic = ContextFactory.Instance.CreateTopic(topicName);
            try
            {
                session.Subscribe(topic, true);
            }
            catch (OperationErrorException e)
            {
                Logger.Error(e, "Unable to subscribe to topic: {0}", topicName);
                throw new Exception("Unable to subscribe to topic: " + topicName, e);
            }
        }

        public void Unsubscribe(string topicName)
        {
            if (string.IsNullOrEmpty(topicName))
                throw new ArgumentNullException("Topic name must not be null");

            Logger.Trace("Unsubscribing to topic: {0}", topicName);

            ITopic topic = ContextFactory.Instance.CreateTopic(topicName);
            try
            {
                session.Unsubscribe(topic, true);
            }
            catch (OperationErrorException e)
            {
                Logger.Error(e, "Unable to unsubscribe from topic: {0}", topicName);
                // Ignore and don't throw
            }
        }

        #endregion

        #region "Observable methods"
        public IDisposable Subscribe(IObserver<SolaceMessageEvent> observer)
        {
            // Check whether observer is already registered. If not, add it
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
            return new Unsubscriber<SolaceMessageEvent>(observers, observer);
        }
        #endregion

        #region "Callbacks"

        private void HandleMessage(object sender, MessageEventArgs args)
        {
            using (IMessage message = args.Message)
            {
                var solMsgEvent = new SolaceMessageEvent();
                solMsgEvent.Message = msgConvertor.ConvertMessage(message);
                solMsgEvent.Source = solMsgEvent.Message.Destination;

                // Notify all observers
                observers.ForEach(o =>
                {
                    o.OnNext(solMsgEvent);
                });
            }
        }

        private void HandleSessionEvent(object sender, SessionEventArgs args)
        {
            Logger.Trace("Solace Session Event - [Event: {0}, ResponseCode: {1}, Info: {2}]",
                args.Event.ToString(), args.ResponseCode, args.Info);
            switch (args.Event)
            {
                case SessionEvent.UpNotice:
                case SessionEvent.Reconnected:
                    isConnected = true;
                    break;
                case SessionEvent.DownError:
                    isConnected = false;
                    // Notify all observers
                    observers.ForEach(o =>
                    {
                        o.OnError(new Exception("Connection lost from Solace."));
                    });
                    break;
                case SessionEvent.Reconnecting:
                    isConnected = false;
                    break;
            }
        }

        #endregion

        #region "Helper Methods"
        private void ApiLogger(SolLogInfo logInfo)
        {
            SolLogLevel level = logInfo.LogLevel;
            switch (level)
            {
                case SolLogLevel.Critical:
                    Logger.Fatal(logInfo.LogMessage);
                    break;
                case SolLogLevel.Error:
                    Logger.Error(logInfo.LogMessage);
                    break;
                case SolLogLevel.Warning:
                    Logger.Warn(logInfo.LogMessage);
                    break;
                case SolLogLevel.Notice:
                    Logger.Info(logInfo.LogMessage);
                    break;
                case SolLogLevel.Info:
                    Logger.Debug(logInfo.LogMessage);
                    break;
                case SolLogLevel.Debug:
                    Logger.Trace(logInfo.LogMessage);
                    break;
            }
        }

        private static SolLogLevel GetSolaceLogLevel(LogLevel logLevel)
        {
            SolLogLevel solLogLevel = SolLogLevel.Notice;
            if (logLevel == LogLevel.Fatal)
                solLogLevel = SolLogLevel.Critical;
            else if (logLevel == LogLevel.Error)
                solLogLevel = SolLogLevel.Error;
            else if (logLevel == LogLevel.Warn)
                solLogLevel = SolLogLevel.Warning;
            else if (logLevel == LogLevel.Info)
                solLogLevel = SolLogLevel.Notice;
            else if (logLevel == LogLevel.Debug)
                solLogLevel = SolLogLevel.Info;
            else if (logLevel == LogLevel.Trace)
                solLogLevel = SolLogLevel.Debug;
            else
                solLogLevel = SolLogLevel.Notice;

            return solLogLevel;
        }
        #endregion
    }
}
