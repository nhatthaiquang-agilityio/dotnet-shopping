using BuildingBlocks.EventBus.Abstractions;
using BuildingBlocks.EventBus.Events;
using Catalog.API.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;


namespace Catalog.API.IntegrationEvents
{
    public class CatalogIntegrationEventService : ICatalogIntegrationEventService
    {
        private readonly IEventBus _eventBus;
        private readonly CatalogContext _catalogContext;
        private readonly ILogger<CatalogIntegrationEventService> _logger;

        public CatalogIntegrationEventService(
            ILogger<CatalogIntegrationEventService> logger,
            IEventBus eventBus,
            CatalogContext catalogContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _catalogContext = catalogContext ?? throw new ArgumentNullException(nameof(catalogContext));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            try
            {
                _logger.LogInformation(
                    "----- Publishing integration event: {IntegrationEventId_published} from {AppName} - ({@IntegrationEvent})",
                    evt.Id, Program.AppName, evt);

                _eventBus.Publish(evt);

                //TODO: save data log
                await Task.Delay(1);

            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "ERROR Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})",
                    evt.Id, Program.AppName, evt);
            }
        }

        public async Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent evt)
        {
            _logger.LogInformation(
                "----- CatalogIntegrationEventService - Saving changes and integrationEvent: {IntegrationEventId}", evt.Id);
            //TODO: save data log
            // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
            await _catalogContext.SaveChangesAsync();
        }
    }
}
