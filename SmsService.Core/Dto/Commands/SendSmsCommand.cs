using System;
using System.Collections.Generic;
using System.Text;

namespace SmsService.Core.Dto.Commands
{
    public class SendSmsCommand
    {
        public string PhoneNumber { get; set; }
        public string SmsText { get; set; }
        public string SessionId { get; set; }
    }
}
