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

namespace Solace.Labs.Excel.SolaceXLCore
{
    public class SolaceConfiguration
    {
        public string Host { get; set; } = "localhost";
        public string MsgVpn { get; set; } = "default";
        public string Username { get; set; } = "default";
        public string Password { get; set; }
        public string ClientName { get; set; }
        public int ReconnectRetries { get; set; } = 5;
        public int ReconnectRetriesWaitInMsecs { get; set; } = 3000; // 3 secs
        public int ConnectRetries { get; set; } = 1;
        public int ConnectRetriesPerHost { get; set; } = 20;

        // For logging
        public LogLevel AppLogLevel { get; set; } = LogLevel.Info; // Equal to Solace->Notice
        public bool EnableLogging { get; set; } = true;

        // AddIn configurable settings
        public string TopicPrefix { get; set; }
    }
}
