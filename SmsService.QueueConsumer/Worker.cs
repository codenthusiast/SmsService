using Microsoft.Extensions.Hosting;
using SmsService.Core.CommandHandlers;
using SmsService.Core.Contracts;
using SmsService.Core.Dto.Commands;
using SmsService.Core.MessageHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmsService.QueueConsumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly SendSmsCommandHandler _commandHandler;
        private readonly IQueueClient _queueClient;

        public Worker(ILogger<Worker> logger, SendSmsCommandHandler commandHandler, IQueueClient queueClient)
        {
            _logger = logger;
            _commandHandler = commandHandler;
            _queueClient = queueClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.Info(string.Format("Worker running at: {0}", DateTimeOffset.Now));
                await _queueClient.HandleMessage<SendSmsCommand>((command) => _commandHandler.Handle(command));
            }
        }
    }
}
