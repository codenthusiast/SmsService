using SmsService.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmsService.Core.Contracts
{
    public interface IEventBus
    {
        Task Publish<T>(T @event) where T : BaseEvent;
    }
}
