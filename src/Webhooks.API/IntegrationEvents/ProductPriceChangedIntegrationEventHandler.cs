using BuildingBlocks.EventBus.Abstractions;
using System.Threading.Tasks;

namespace Webhooks.API.IntegrationEvents
{
    public class ProductPriceChangedIntegrationEventHandler : IIntegrationEventHandler<ProductPriceChangedIntegrationEvent>
    {
        public async Task Handle(ProductPriceChangedIntegrationEvent @event)
        {
            await Task.Delay(1);
        }
    }
}
