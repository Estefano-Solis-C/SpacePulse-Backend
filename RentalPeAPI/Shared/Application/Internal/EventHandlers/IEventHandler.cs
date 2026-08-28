using Cortex.Mediator.Notifications;
using RentalPeAPI.Shared.Domain.Model.Events;

namespace RentalPeAPI.Shared.Application.Internal.EventHandlers;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IEvent
{
    
}