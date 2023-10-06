using Moq;
using SmsService.Core.AppExceptions;
using SmsService.Core.CommandHandlers;
using SmsService.Core.Contracts;
using SmsService.Core.Dto.Commands;
using SmsService.Core.Dto.Response;
using SmsService.Core.Events;
using SmsService.Core.Repository;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SmsService.Test.CommandHandlers
{
    public class SendSmsCommandHandlerTest : IDisposable
    {

        private readonly Mock<ISmsRepository> _smsRepository;
        private readonly Mock<IEventBus> _eventBus;
        private readonly Mock<ISmsGateway> _smsGateway;
        private readonly Mock<ILogger<SendSmsCommandHandler>> _logger;
        private readonly SendSmsCommandHandler _sut;
        private readonly SendSmsCommand _command;

        public SendSmsCommandHandlerTest()
        {
            _smsRepository = new Mock<ISmsRepository>();
            _eventBus = new Mock<IEventBus>();
            _smsGateway = new Mock<ISmsGateway>();
            _logger = new Mock<ILogger<SendSmsCommandHandler>>();
            _sut = new SendSmsCommandHandler(_smsRepository.Object, _eventBus.Object, _smsGateway.Object, _logger.Object);
            _command = new SendSmsCommand
            {
                PhoneNumber = "08033333333",
                SmsText = "Payment made successfully",
                SessionId = Guid.NewGuid().ToString()
            };

        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidArgumentException_SendSmsCommandIsNull()
        {
            SendSmsCommand command = null;
            
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.Handle(command));
        }


        [Fact]
        public async Task Handle_ShouldThrowGatewayException_SmsGatewaySendAsyncFailed()
        {

            _smsRepository.Setup(x => x.Exists(_command.SessionId)).ReturnsAsync(false);
            var sendSmsResponse = new SendSmsResponse
            {
                IsSuccessful = true
            };
            _smsGateway.Setup(x => x.SendAsync(_command)).Throws<GatewayException>();

            await Assert.ThrowsAsync<GatewayException>(() => _sut.Handle(_command));
            _logger.Verify(x => x.Error(It.IsAny<GatewayException>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_UncaughtExceptionOccurr()
        {

            var sendSmsResponse = new SendSmsResponse
            {
                IsSuccessful = true
            };

            _smsGateway.Setup(x => x.SendAsync(_command)).ReturnsAsync(sendSmsResponse);
            _smsRepository.Setup(x => x.Exists(_command.SessionId)).ReturnsAsync(false);
            _smsRepository.Setup(x => x.Insert(_command)).Throws<Exception>();


            await Assert.ThrowsAsync<Exception>(() => _sut.Handle(_command));
            _logger.Verify(x => x.Error(It.IsAny<Exception>(), string.Format("An unexpected error occurred. Session ID {0}", _command.SessionId)), Times.Once);
        }


        [Fact]
        public async Task Handle_ShouldReturnTrue_SessionIdExists()
        {

            _smsRepository.Setup(x => x.Exists(_command.SessionId)).ReturnsAsync(true);

            var result = await _sut.Handle(_command);

            Assert.True(result);
        }

        [Fact]
        public async Task Handle_ShouldNeverCallEventBusPublish_SessionIdExists()
        {

            _smsRepository.Setup(x => x.Exists(_command.SessionId)).ReturnsAsync(true);

            var result = await _sut.Handle(_command);

            _logger.Verify(x => x.Info(string.Format("Duplicate request {0}", _command.SessionId)));
            _eventBus.Verify(_ => _.Publish(It.IsAny<SmsSentEvent>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnTrue_SessionIdDoesNotExist()
        {

            _smsRepository.Setup(x => x.Exists(_command.SessionId)).ReturnsAsync(false);
            var sendSmsResponse = new SendSmsResponse
            {
                IsSuccessful = true
            };
            _smsGateway.Setup(x => x.SendAsync(_command)).ReturnsAsync(sendSmsResponse);

            var result = await _sut.Handle(_command);

            Assert.True(result);
            _logger.Verify(x => x.Info(It.IsAny<string>()), Times.Exactly(4));
        }

        [Fact]
        public async Task Handle_ShouldCallEventBusPublishOnce_SessionIdDoesNotExist()
        {

            _smsRepository.Setup(x => x.Exists(_command.SessionId)).ReturnsAsync(false);
            var sendSmsResponse = new SendSmsResponse
            {
                IsSuccessful = true
            };
            _smsGateway.Setup(x => x.SendAsync(_command)).ReturnsAsync(sendSmsResponse);

            var result = await _sut.Handle(_command);

            _eventBus.Verify(_ => _.Publish(It.IsAny<SmsSentEvent>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_SmsGatewayNotSuccessful()
        {

            _smsRepository.Setup(x => x.Exists(_command.SessionId)).ReturnsAsync(false);
            var sendSmsResponse = new SendSmsResponse
            {
                IsSuccessful = false,
                Message = "An error occurred"
            };
            _smsGateway.Setup(x => x.SendAsync(_command)).ReturnsAsync(sendSmsResponse);

            var result = await _sut.Handle(_command);
            _logger.Verify(x => x.Error(string.Format("Unable to send SMS. Gateway response: {0}. session ID: {1}", sendSmsResponse.Message, _command.SessionId)), Times.Once);
            Assert.False(result);
        }


        [Fact]
        public async Task Handle_ShouldNeverCallPublish_SmsGatewayNotSuccessful()
        {

            _smsRepository.Setup(x => x.Exists(_command.SessionId)).ReturnsAsync(false);
            var sendSmsResponse = new SendSmsResponse
            {
                IsSuccessful = false,
                Message = "An error occurred"
            };
            _smsGateway.Setup(x => x.SendAsync(_command)).ReturnsAsync(sendSmsResponse);

            var result = await _sut.Handle(_command);

            _eventBus.Verify(_ => _.Publish(It.IsAny<SmsSentEvent>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCallPublishOnce_SessionIdDoesNotExist()
        {

            _smsRepository.Setup(x => x.Exists(_command.SessionId)).ReturnsAsync(false);
            var sendSmsResponse = new SendSmsResponse
            {
                IsSuccessful = true
            };
            _smsGateway.Setup(x => x.SendAsync(_command)).ReturnsAsync(sendSmsResponse);

            var result = await _sut.Handle(_command);

            _eventBus.Verify(_ => _.Publish(It.IsAny<SmsSentEvent>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldCallInsertOnce_SessionIdDoesNotExist()
        {
            _smsRepository.Setup(x => x.Exists(_command.SessionId)).ReturnsAsync(false);
            var sendSmsResponse = new SendSmsResponse
            {
                IsSuccessful = true
            };
            _smsGateway.Setup(x => x.SendAsync(_command)).ReturnsAsync(sendSmsResponse);

            var result = await _sut.Handle(_command);

            _smsRepository.Verify(_ => _.Insert(It.IsAny<SendSmsCommand>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldNeverCallInsert_SessionIdExists()
        {

            _smsRepository.Setup(x => x.Exists(_command.SessionId)).ReturnsAsync(true);
            var sendSmsResponse = new SendSmsResponse
            {
                IsSuccessful = true
            };
            _smsGateway.Setup(x => x.SendAsync(_command)).ReturnsAsync(sendSmsResponse);

            var result = await _sut.Handle(_command);

            _smsRepository.Verify(_ => _.Insert(It.IsAny<SendSmsCommand>()), Times.Never);
        }

        public void Dispose()
        {
        }
    }
}
