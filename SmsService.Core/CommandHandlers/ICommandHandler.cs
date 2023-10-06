using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmsService.Core.MessageHandlers
{
    public interface ICommandHandler<T> where T : class
    {
        Task<bool> Handle(T command);
    }
}
