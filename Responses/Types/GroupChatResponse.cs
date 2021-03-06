﻿using TeleBotDotNet.Json;

namespace TeleBotDotNet.Responses.Types
{
    public class GroupChatResponse
    {
        public int Id { get; private set; }
        public string Title { get; private set; }

        internal static GroupChatResponse Parse(JsonData data)
        {
            if (data == null || !data.Has("id") || !data.Has("title"))
            {
                return null;
            }

            return new GroupChatResponse
            {
                Id = data.Get<int>("id"),
                Title = data.Get<string>("title")
            };
        }
    }
}