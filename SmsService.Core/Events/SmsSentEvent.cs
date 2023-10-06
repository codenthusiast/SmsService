using System;
using System.Collections.Generic;
using System.Text;

namespace SmsService.Core.Events
{
    public class SmsSentEvent : BaseEvent
    {
        public SmsSentEvent(string phoneNumber, string sessionId) : base(sessionId, "SmsSent")
        {
            PhoneNumber = phoneNumber;
        }
        public string PhoneNumber { get; set; }

    }
}
