using SmsService.Core.Dto.Commands;
using SmsService.Core.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmsService.Core.Contracts
{
    public interface ISmsGateway
    {
        Task<SendSmsResponse> SendAsync(SendSmsCommand request);
    }
}
