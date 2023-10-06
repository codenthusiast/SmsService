using SmsService.Core.Dto.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmsService.Core.Repository
{
    public interface ISmsRepository
    {
        Task Insert(SendSmsCommand command);
        Task<SendSmsCommand> GetBySessionId(string sessionId);
        Task<bool> Exists(string sessionId);
    }
}
