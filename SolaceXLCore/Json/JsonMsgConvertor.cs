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
using SolaceSystems.Solclient.Messaging;
using Newtonsoft.Json;
using NLog;

namespace Solace.Labs.Excel.SolaceXLCore.Json
{
    public class JsonMsgConvertor : MsgConvertor
    {
        private Logger logger { get; } = ApplicationLogging.CreateLogger<JsonMsgConvertor>();

        public override SolaceMessage ConvertMessage(IMessage message)
        {
            var jsonMsg = new JsonSolaceMessage();

            // Destination
            jsonMsg.Destination = GetDestination(message);

            // Copy binary contents
            sbyte[] bodyBytes = message.GetBinaryAttachment();
            if (bodyBytes != null)
                jsonMsg.BodyAsBytes = new ArraySegment<byte>((byte[])((Array)bodyBytes));

            // Try to convert binary contents to Json
            try
            {
                string bodyAsString = jsonMsg.Body;
                if (!string.IsNullOrEmpty(bodyAsString))
                    jsonMsg.BodyAsJson = JsonConvert.DeserializeObject<dynamic>(bodyAsString);
            }
            catch (Exception e)
            {
                // Probably not a json message or well formated json
                logger.Error(e, "Unable to convert received message to a Json Message.");
            }

            return jsonMsg;
        }
    }
}
