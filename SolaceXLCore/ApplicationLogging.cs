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
using NLog.Config;
using NLog.Targets;

namespace Solace.Labs.Excel.SolaceXLCore
{
    public static class ApplicationLogging
    {
        public static Logger CreateLogger<T>() => LogManager.GetLogger(typeof(T).Name);

        static ApplicationLogging()
        {
            // Create configuration object 
            var config = new LoggingConfiguration();

            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            // Set target properties 
            fileTarget.FileName = SolaceXLHelper.GetLogFilePath();
            fileTarget.Layout = @"${longdate} ${level:uppercase=true} [${logger}] : ${message}";
            fileTarget.ArchiveNumbering = ArchiveNumberingMode.Rolling;
            fileTarget.ArchiveAboveSize = 10485760; // 10 MB
            fileTarget.MaxArchiveFiles = 20;

            // Define rules
            var rule = new LoggingRule("*", LogLevel.Info, fileTarget);
            config.LoggingRules.Add(rule);

            // Activate the configuration
            LogManager.Configuration = config;
        }

        public static void UpdateLogging(LogLevel newLevel, bool disableLogging)
        {
            foreach (var rule in LogManager.Configuration.LoggingRules)
            {
                rule.EnableLoggingForLevel(newLevel);
            }

            // Call to update existing Loggers created with GetLogger() or 
            // GetCurrentClassLogger()
            LogManager.ReconfigExistingLoggers();
        }
    }
}
