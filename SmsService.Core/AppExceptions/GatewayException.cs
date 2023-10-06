using System;
using System.Collections.Generic;
using System.Text;

namespace SmsService.Core.AppExceptions
{
    public class GatewayException : Exception
    {
        public GatewayException() : base ()
        {

        }

        public GatewayException(string message) : base(message)
        {

        }

        public GatewayException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
