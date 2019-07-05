using BasketAPI.IntegrationEvents.Events;
using BasketAPI.Model;
using BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BasketAPI.IntegrationEvents.EventHandling
{
    public class ProductPriceChangedIntegrationEventHandler : IIntegrationEventHandler<ProductPriceChangedIntegrationEvent>
    {
        private readonly ILogger<ProductPriceChangedIntegrationEventHandler> _logger;
        private readonly IBasketRepository _repository;

        public ProductPriceChangedIntegrationEventHandler(
            ILogger<ProductPriceChangedIntegrationEventHandler> logger,
            IBasketRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task Handle(ProductPriceChangedIntegrationEvent @event)
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

            var userIds = _repository.GetUsers();

            if (userIds.Count<string>() > 0)
            {
                foreach (var id in userIds)
                {
                    var basket = await _repository.GetBasketAsync(id);

                    await UpdatePriceInBasketItems(@event.ProductId, @event.NewPrice, @event.OldPrice, basket);
                }
            }
            else
            {
                _logger.LogInformation("----- SaveEventBasketAsync");
                await _repository.SaveEventBasketAsync(@event.ProductId, @event.NewPrice, @event.OldPrice);
            }

        }

        private async Task UpdatePriceInBasketItems(int productId, decimal newPrice, decimal oldPrice, CustomerBasket basket)
        {
            string match = productId.ToString();
            var itemsToUpdate = basket?.Items?.Where(x => x.ProductId == match).ToList();

            if (itemsToUpdate != null)
            {
                _logger.LogInformation(
                    "----- ProductPriceChangedIntegrationEventHandler - Updating items in basket for user: {BuyerId} ({@Items})",
                    basket.BuyerId, itemsToUpdate);

                foreach (var item in itemsToUpdate)
                {
                    if (item.UnitPrice == oldPrice)
                    {
                        var originalPrice = item.UnitPrice;
                        item.UnitPrice = newPrice;
                        item.OldUnitPrice = originalPrice;
                    }
                }
                await _repository.UpdateBasketAsync(basket);
            } 
        }
    }
}
