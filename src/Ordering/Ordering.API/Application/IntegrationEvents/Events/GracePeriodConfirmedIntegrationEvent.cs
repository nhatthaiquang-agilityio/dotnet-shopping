namespace Ordering.API.Application.IntegrationEvents.Events
{
    using BuildingBlocks.EventBus.Events;

    public class GracePeriodConfirmedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }

        public GracePeriodConfirmedIntegrationEvent(int orderId) =>
            OrderId = orderId;
    }
}
