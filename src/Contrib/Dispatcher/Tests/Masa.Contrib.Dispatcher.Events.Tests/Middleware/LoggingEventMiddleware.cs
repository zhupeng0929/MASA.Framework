// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.Middleware;

public class LoggingEventMiddleware<TEvent> : EventMiddleware<TEvent> where TEvent : IEvent
{
    private readonly ILogger<LoggingEventMiddleware<TEvent>>? _logger;
    public LoggingEventMiddleware(ILogger<LoggingEventMiddleware<TEvent>>? logger = null) => _logger = logger;

    public override async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        var eventType = @event.GetType();
        _logger?.LogInformation("----- Handling command {FullName} ({event})", eventType.FullName, @event);
        await next();
    }
}
