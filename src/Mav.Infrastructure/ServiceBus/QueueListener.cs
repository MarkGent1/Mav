using Azure.Messaging.ServiceBus;
using Mav.Infrastructure.Exceptions;
using Mav.Infrastructure.Serlializers;
using Mav.Infrastructure.ServiceBus.Processors;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Mav.Infrastructure.ServiceBus;

public class QueueListener<T>(IMessageProcessor<T> messageProcessor,
    IServiceBusClientFactory serviceBusClientFactory,
    IServiceBusReceivedMessageSerializer<T> serializer,
    TelemetryClient telemetryClient,
    ILogger<QueueListener<T>> logger) : IHostedService
{
    private readonly IMessageProcessor<T> _messageProcessor = messageProcessor;
    private readonly IServiceBusClientFactory _serviceBusClientFactory = serviceBusClientFactory;
    private readonly IServiceBusReceivedMessageSerializer<T> _serializer = serializer;
    private readonly TelemetryClient _telemetryClient = telemetryClient;
    private readonly ILogger<QueueListener<T>> _logger = logger;

    private ServiceBusProcessor? _receiver;

    public async Task ReceiveMessageAsync(ProcessMessageEventArgs args)
    {
        if (!args.CancellationToken.IsCancellationRequested)
        {
            var message = args.Message;

            try
            {
                var messageBody = _serializer.Deserialize(args.Message);

                await _messageProcessor.ProcessMessageAsync(messageBody, args.CancellationToken);

                await args.CompleteMessageAsync(args.Message, args.CancellationToken);
            }
            catch (RetryableException ex)
            {
                _telemetryClient.TrackException(ex);

                _logger.LogError(ex, "A RetryableException thrown trying to process message from queue: {queueName}, messageId: {messageId}, exception.Message: {exceptionMessage}",
                    _messageProcessor.QueueName,
                    args.Message.MessageId,
                    ex.Message);

                await args.AbandonMessageAsync(message, GetExceptionHeaderDictionary(ex), args.CancellationToken);
            }
            catch (NonRetryableException ex)
            {
                _telemetryClient.TrackException(ex);

                _logger.LogError(ex, "A NonRetryableException thrown trying to process message from queue: {queueName}, messageId: {messageId}, exception.Message: {exceptionMessage}",
                    _messageProcessor.QueueName,
                    args.Message.MessageId,
                    ex.Message);

                await args.DeadLetterMessageAsync(message, GetExceptionHeaderDictionary(ex), args.CancellationToken);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);

                _logger.LogError(ex, "A Exception thrown trying to process message from queue: {queueName}, messageId: {messageId}, exception.Message: {exceptionMessage}",
                    _messageProcessor.QueueName,
                    args.Message.MessageId,
                    ex.Message);

                await args.AbandonMessageAsync(message, GetExceptionHeaderDictionary(ex), args.CancellationToken);
            }
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        try
        {
            _receiver = _serviceBusClientFactory.CreateReceiver(_messageProcessor.QueueName, ReceiveMessageAsync);

            _receiver.ProcessMessageAsync += ReceiveMessageAsync;
            _receiver.ProcessErrorAsync += ExceptionReceivedHandler;

            await _receiver.StartProcessingAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _telemetryClient.TrackException(ex);

            _logger.LogError(ex, "Error in StartAsync: {Message}", ex.Message);

            throw;
        }
    }

    private Task ExceptionReceivedHandler(ProcessErrorEventArgs exceptionReceivedEventArgs)
    {
        _telemetryClient.TrackException(exceptionReceivedEventArgs.Exception);

        _logger.LogError(exceptionReceivedEventArgs.Exception, "Message handler encountered an exception: {Exception}. Entity Path: {EntityPath}.",
            exceptionReceivedEventArgs.Exception, exceptionReceivedEventArgs.EntityPath);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (!_receiver?.IsClosed ?? false)
        {
            await _receiver!.CloseAsync(cancellationToken);
        }
    }

    private IDictionary<string, object> GetExceptionHeaderDictionary(Exception exception)
    {
        var baseException = exception.GetBaseException();
        var (stackTrace, errorMessage) = GetRecursiveExceptionInfo(exception);

        return new Dictionary<string, object> {
            { "Reason", "fault" },
            { "ExceptionType", baseException.GetType().Name },
            { "ExceptionMessage", errorMessage },
            { "ExceptionStackTrace", stackTrace },
            { "Data", JsonSerializer.Serialize(exception.Data) }
        };
    }

    private static (object stackTrace, object errorMessage) GetRecursiveExceptionInfo(Exception exception)
    {
        var currentException = exception;
        var stackTraceStringBuilder = new StringBuilder();
        var errorMessageStringBuilder = new StringBuilder();

        while (currentException != null) 
        {
            if (currentException.StackTrace != null)
            {
                stackTraceStringBuilder.AppendLine(currentException.StackTrace);
            }

            errorMessageStringBuilder.Append($" => {currentException.Message}");
            currentException = currentException.InnerException;
        }

        return (stackTraceStringBuilder.ToString(), errorMessageStringBuilder.ToString());
    }
}
