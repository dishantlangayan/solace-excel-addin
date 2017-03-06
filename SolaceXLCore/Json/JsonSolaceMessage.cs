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
using System.Text;

namespace Solace.Labs.Excel.SolaceXLCore.Json
{
    public class JsonSolaceMessage : SolaceMessage
    {
        private ArraySegment<byte> _body;

        public override string Body
        {
            get
            {
                if (_body.Count == 0)
                    return null;
                else
                    return Encoding.UTF8.GetString(_body.Array, _body.Offset, _body.Count);
            }
            set
            {
                if (value == null)
                    _body = new ArraySegment<byte>();
                else
                    _body = new ArraySegment<byte>(Encoding.UTF8.GetBytes(value));
            }
        }

        public override ArraySegment<byte> BodyAsBytes
        {
            get
            {
                return _body;
            }
            set
            {
                _body = value;
            }
        }

        public dynamic BodyAsJson { get; set; }

        /// <summary>
        /// Looks up the key in the Json body and return's the corresponding value.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override string GetData(string key)
        {
            if (BodyAsJson != null)
            {
                var jsonValue = BodyAsJson[key];
                if (jsonValue != null)
                {
                    return jsonValue.ToString();
                }
            }

            return null;
        }
    }
}
