using System;
using System.Collections.Generic;
using System.Text;

namespace SmsService.Core.Contracts
{
    public interface ILogger<T> where T : class
    {
        void Info(string message);
        void Error(Exception ex, string message);
        void Error(string message);
        void Debug(string message);
        void Warn(string message);
    }
}
