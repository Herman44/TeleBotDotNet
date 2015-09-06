﻿using System;
using TeleBotDotNet.Http;
using TeleBotDotNet.Requests.Methods.Bases;
using TeleBotDotNet.Requests.Types;

namespace TeleBotDotNet.Requests.Methods
{
    public class SendVoiceRequest : BaseMethodRequest
    {
        public int ChatId { get; set; }
        public InputFileRequest Voice { get; set; }
        public int? Duration { get; set; }
        public int? ReplyToMessageId { get; set; }
        public ReplyMarkupRequest ReplyMarkup { get; set; }

        internal override string MethodName { get; } = "sendVoice";

        internal override HttpData Parse()
        {
            var httpData = new HttpData
            {
                Parameters = new HttpParameterList
                {
                    {"chat_id", ChatId},
                    {"duration", Duration},
                    {"reply_to_message_id", ReplyToMessageId}
                }
            };

            if (Voice != null)
            {
                Voice.Parse(httpData, "voice");
            }
            if (ReplyMarkup != null)
            {
                ReplyMarkup.Parse(httpData, "reply_markup");
            }

            return httpData;
        }
    }
}
