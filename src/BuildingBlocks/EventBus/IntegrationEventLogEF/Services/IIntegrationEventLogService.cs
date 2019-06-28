using System;
using System.Data.Common;
using System.Threading.Tasks;
using BuildingBlocks.EventBus.Events;
using Microsoft.EntityFrameworkCore.Storage;

namespace BuildingBlocks.IntegrationEventLogEF.Services
{
    public interface IIntegrationEventLogService
    {
        Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction);
        Task MarkEventAsPublishedAsync(Guid eventId);
        Task MarkEventAsInProgressAsync(Guid eventId);
        Task MarkEventAsFailedAsync(Guid eventId);
    }
}
