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
using Newtonsoft.Json;
using System;
using System.IO;

namespace Solace.Labs.Excel.SolaceXLCore
{
    public sealed class SolaceXLHelper
    {
        public static string ApplicationDataPath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string AppName { get; } = "SolaceXL";
        public static string ConfigFileName { get; } = "SolaceXLConfig.json";
        public static string LogFileName { get; } = "solacexl.log";

        public static string GetApplicationDirectory()
        {
            string configPath = ApplicationDataPath + "\\" + AppName;

            // Create the directory if it does not exist
            if (!Directory.Exists(configPath))
                Directory.CreateDirectory(configPath);

            return configPath;
        }

        public static string GetLogFilePath()
        {
            return GetApplicationDirectory() + "\\" + LogFileName;
        }

        public static string GetConfigFilePath()
        {
            return GetApplicationDirectory() + "\\" + ConfigFileName;
        }

        /// <summary>
        /// Saves the configuration in the application's home directory
        /// </summary>
        public static void SaveConfiguration(SolaceConfiguration config)
        {
            string filePath = GetConfigFilePath();
            using (StreamWriter file = File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, config);
            }
            // Encrypt the file using user's own encryption key so other can't see the file
            File.Encrypt(filePath);
        }

        /// <summary>
        /// Loads the configuration from the application's data directory.
        /// </summary>
        /// <returns></returns>
        public static SolaceConfiguration LoadConfiguration()
        {
            string filePath = GetConfigFilePath();
            // Decrypt the file first
            File.Decrypt(filePath);
            using (StreamReader file = File.OpenText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                var solaceConfig = (SolaceConfiguration)serializer.Deserialize(file, typeof(SolaceConfiguration));
                return solaceConfig;
            }
        }
    }
}
