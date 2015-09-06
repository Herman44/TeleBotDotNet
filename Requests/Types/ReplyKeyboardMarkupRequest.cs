﻿using System.Collections.Generic;
using TeleBotDotNet.Http;
using TeleBotDotNet.Json;
using TeleBotDotNet.Requests.Types.Bases;

namespace TeleBotDotNet.Requests.Types
{
    public class ReplyKeyboardMarkupRequest : BaseTypeRequest
    {
        public List<List<string>> Keyboard { get; set; }
        public bool ResizeKeyboard { get; set; }
        public bool OneTimeKeyboard { get; set; }
        public bool Selective { get; set; }

        internal override void Parse(HttpData httpData, string key)
        {
            httpData.Parameters.Add(key, JsonData.Serialize(new
            {
                keyboard = Keyboard,
                resize_keyboard = ResizeKeyboard,
                one_time_keyboard = OneTimeKeyboard,
                selective = Selective
            }));
        }
    }
}