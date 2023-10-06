using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmsService.Core.Contracts
{
    public interface IQueueClient
    {
        Task HandleMessage<T>(Func<T, Task<bool>> messageHandler) where T : class;
    }
}
