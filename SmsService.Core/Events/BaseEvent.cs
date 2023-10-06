using System;
using System.Collections.Generic;
using System.Text;

namespace SmsService.Core.Events
{
    public abstract class BaseEvent
    {
        public BaseEvent(string id, string eventType)
        {
            Id = id;
            Sent = DateTimeOffset.Now;
            EventType = eventType;
        }
        public string Id { get; set; }
        public DateTimeOffset? Sent { get; private set; }

        public string EventType { get; private set; }
    }
}
