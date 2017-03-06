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
using ExcelDna.Integration.Rtd;
using System;
using System.Collections.Concurrent;
using System.Timers;
using System.Runtime.InteropServices;
using Solace.Labs.Excel.SolaceXLCore;
using Solace.Labs.Excel.SolaceXLCore.Json;
using NLog;
using System.Collections.Generic;

namespace Solace.Labs.Excel.SolaceXLAddIn
{
    [Guid("CE28AD3C-0140-42FA-BE25-AF6FE4A76CC6")]
    public class SolaceRtdServer : IRtdServer, IObserver<SolaceMessageEvent>
    {
        private Logger Logger { get; } = ApplicationLogging.CreateLogger<SolaceRtdServer>();

        private IRTDUpdateEvent callback;
        private Timer timer;
        private Dictionary<int, SymbolData> topics;
        private IList<string> symbols;

        private ConcurrentDictionary<string, SolaceMessageEvent> messageCache;
        private IDisposable msgUnsubscriber = null;

        // Status codes
        private const int SUCCESS = 1;
        private const int FAILURE = 0;
        private static string NOT_CONNECTED_STATUS = "NOT_CONNECTED";
        private static string ERROR_STATUS = "ERROR";

        private class SymbolData
        {
            public string Symbol { get; set; }
            public string FieldName { get; set; }
            public string SolaceTopic { get; set; }

            public override string ToString()
            {
                return string.Format("SymbolData - [Symbol: {0}, FieldName: {1}, SolaceTopic: {2}]", 
                    Symbol, FieldName, SolaceTopic);
            }
        }

        public SolaceRtdServer()
        {
            messageCache = new ConcurrentDictionary<string, SolaceMessageEvent>();
        }

        public object ConnectData(int topicId, ref Array strings, ref bool newValues)
        {
            if (strings == null || strings.Length != 2)
                return ERROR_STATUS;

            string symbol = strings.GetValue(0).ToString();
            string fieldName = strings.GetValue(1).ToString();
            if (string.IsNullOrWhiteSpace(symbol) || string.IsNullOrWhiteSpace(fieldName))
            {
                Logger.Error("Required symbol or field parameter missing for SolaceXL RTD function");
                return ERROR_STATUS;
            }

            var symbolData = new SymbolData { Symbol = symbol, FieldName = fieldName };

            // Add the symbol to the list of data we need to refresh
            if (!topics.ContainsKey(topicId))
            {
                Logger.Debug("Adding symbol data - " + symbolData.ToString());
                topics.Add(topicId, symbolData);
            }

            if(symbols.Contains(symbol))
            {
                // We are already subscribed to the symbol
                return GetData(symbolData);
            }
            else
            {
                return SubscribeData(symbolData);
            }
        }

        public void DisconnectData(int topicID)
        {
            if (topics.ContainsKey(topicID))
            {
                var symbolData = topics[topicID];
                Logger.Debug("Removed data for - {0}", symbolData.ToString());

                // TODO: maintain a ref count for ConnectData calls for a single
                // symbol and unsubscribe from Solace.
            }
        }

        public int Heartbeat()
        {
            return SUCCESS;
        }

        public Array RefreshData(ref int topicCount)
        {
            topicCount = 0;
            object[,] data = new object[2, topics.Count];

            if (!SolaceTransport.Instance.IsConnected)
                symbols.Clear();

            foreach (int topicID in topics.Keys)
            {
                var symbolData = topics[topicID];
                if(!symbols.Contains(symbolData.Symbol))
                {
                    // Subscribe to the symbol
                    SubscribeData(symbolData);
                }
                data[0, topicCount] = topicID;
                data[1, topicCount] = GetData(symbolData);
                topicCount++;
            }
            return data;
        }

        public int ServerStart(IRTDUpdateEvent CallbackObject)
        {
            callback = CallbackObject;
            timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(TimeEventHandler);
            timer.Interval = 1000;
            symbols = new List<string>();
            topics = new Dictionary<int, SymbolData>();
            timer.Start();

            // Register to receive Solace Messages
            msgUnsubscriber = SolaceTransport.Instance.Subscribe(this);

            Logger.Info("SolaceXL RTD server has started successfully");
            return SUCCESS;
        }

        public void ServerTerminate()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }
            if (msgUnsubscriber != null)
                msgUnsubscriber.Dispose();
            if (messageCache != null)
                messageCache.Clear();
            if (symbols != null)
                symbols.Clear();

            Logger.Info("SolaceXL RTD server has stopped gracefully");
        }

        private string GetData(SymbolData symbolData)
        {
            if (!SolaceTransport.Instance.IsConnected)
                return NOT_CONNECTED_STATUS;
            
            if (string.IsNullOrEmpty(symbolData.SolaceTopic))
                symbolData.SolaceTopic = GetSolaceTopic(symbolData.Symbol);

            // Check if we have data for this topic
            SolaceMessageEvent msgEvent;
            if (messageCache.TryGetValue(symbolData.SolaceTopic, out msgEvent))
            {
                // Get the data for the given key from the JSON Message 
                JsonSolaceMessage message = (JsonSolaceMessage)msgEvent.Message;
                return message.GetData(symbolData.FieldName);
            }

            return null;
        }

        private string GetSolaceTopic(string topic)
        {
            // Append the key to the topic prefix configured
            if (SolaceTransport.Instance.Config == null)
                return "";
            else
                return SolaceTransport.Instance.Config.TopicPrefix + topic;
        }

        private object SubscribeData(SymbolData symbolData)
        {
            if (SolaceTransport.Instance.IsConnected)
            {
                try
                {
                    if (string.IsNullOrEmpty(symbolData.SolaceTopic))
                        symbolData.SolaceTopic = GetSolaceTopic(symbolData.Symbol);
                    SolaceTransport.Instance.Subscribe(symbolData.SolaceTopic);
                    symbols.Add(symbolData.Symbol);
                    Logger.Info("Subscription added for - {0}", symbolData.ToString());
                    return GetData(symbolData);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Exception while subscribing for data - {0}", symbolData.ToString());
                    return ERROR_STATUS;
                }
            }
            else
            {
                Logger.Trace("SolaceXL is not connected to a Solace Message Router - {0}", symbolData.ToString());
                return NOT_CONNECTED_STATUS;
            }
        }

        #region Timer Handler
        private void TimeEventHandler(object sender, ElapsedEventArgs e)
        {
            callback.UpdateNotify();
        }
        #endregion

        #region Observable Interface

        public void OnCompleted()
        {
            // Do nothing
        }

        public void OnError(Exception error)
        {
            // We are disconnected from Solace
            // Clear all symbol from our tracked list so we re-subscribe
            symbols.Clear();
        }

        public void OnNext(SolaceMessageEvent msgEvent)
        {
            // Save the message in our cache
            if (msgEvent != null)
            {
                messageCache.AddOrUpdate(msgEvent.Source, msgEvent,
                    (key, existingVal) =>
                    {
                        if (msgEvent.Source.Equals(existingVal.Source))
                            existingVal.Message = msgEvent.Message;

                        return existingVal;
                    });
            }
        }

        #endregion
    }
}
