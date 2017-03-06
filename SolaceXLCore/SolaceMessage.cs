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
    public abstract class SolaceMessage
    {
        public string Destination { get; set; }

        /// <summary>
        ///     Body contents of the message. This triggers a conversion through UTF-8 to store
        ///     as binary, and thus is slower than accessing BodyAsBytes directly.
        /// </summary>
        public abstract string Body { get; set; }

        /// <summary>
        ///     Body contents of message.
        /// </summary>
        public abstract ArraySegment<byte> BodyAsBytes { get; set; }

        /// <summary>
        ///     Look up the key in the body of the message and return it's associated value.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract string GetData(string key);

        /// <summary>
        ///     Returns a readable String of the SolaceMessage with the destination, and size of the payload.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("SolaceMessage[Destination:{0}, BodySize:{1}]", 
                Destination, BodyAsBytes.Count);
        }
    }
}
