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

using System;

namespace Solace.Labs.Excel.SolaceXLCore
{
    public class SolaceMessageEvent : EventArgs
    {
        public string Source { get; set; }

        public SolaceMessage Message { get; set; }

        /// <summary>
        ///     Converts the SolaceMessageEvent objects to a readable and printable format.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[on {0}, msg:{1}]", Source, Message);
        }
    }
}
