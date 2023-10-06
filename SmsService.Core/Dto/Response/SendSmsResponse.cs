using System;
using System.Collections.Generic;
using System.Text;

namespace SmsService.Core.Dto.Response
{
    public class SendSmsResponse
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
    }
}
