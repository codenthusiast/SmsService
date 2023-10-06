using SmsService.Core.AppExceptions;
using SmsService.Core.Contracts;
using SmsService.Core.Dto.Commands;
using SmsService.Core.Events;
using SmsService.Core.MessageHandlers;
using SmsService.Core.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmsService.Core.CommandHandlers
{
    public class SendSmsCommandHandler : ICommandHandler<SendSmsCommand>
    {
        private readonly ISmsRepository _repository;
        private readonly IEventBus _eventBus;
        private readonly ISmsGateway _smsGateway;
        private readonly ILogger<SendSmsCommandHandler> _logger;

        public SendSmsCommandHandler(ISmsRepository repository, IEventBus eventBus, ISmsGateway smsGateway, ILogger<SendSmsCommandHandler> logger)
        {
            _repository = repository;
            _eventBus = eventBus;
            _smsGateway = smsGateway;
            _logger = logger;
        }

        public async Task<bool> Handle(SendSmsCommand command)
        {
            if(command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if(await _repository.Exists(command.SessionId))
            {
                _logger.Info(string.Format("Duplicate request {0}", command.SessionId));

                return true;
            }

            try
            {
                _logger.Info(string.Format("Sending message to Gateway. Session ID {0}", command.SessionId));
                var gatewayResponse = await _smsGateway.SendAsync(command);

                if (gatewayResponse.IsSuccessful)
                {
                    _logger.Info(string.Format("Message sent to Gateway successfully. Session ID {0}. Saving to DB", command.SessionId));
                    await _repository.Insert(command);
                    _logger.Info(string.Format("Message saved to DB. Session ID {0}. Publishing to event bus", command.SessionId));

                    var @event = new SmsSentEvent(command.PhoneNumber, command.SessionId);
                    await _eventBus.Publish(@event);
                    _logger.Info(string.Format("Event published successfully. Session ID {0}.", command.SessionId));
                    return true;
                }
                _logger.Error(string.Format("Unable to send SMS. Gateway response: {0}. session ID: {1}", gatewayResponse.Message, command.SessionId));
                return false;
            }
            catch(GatewayException ex)
            {
                _logger.Error(ex, string.Format("Error communicating with SMS gateway. Session ID {0}", command.SessionId));
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, string.Format("An unexpected error occurred. Session ID {0}", command.SessionId));
                throw;
            }
            
        }
    }
}
