using Web.Shopping.HttpAggregator.Models;
using System.Threading.Tasks;

namespace Web.Shopping.HttpAggregator.Services
{
    public interface IOrderApiClient
    {
        Task<OrderData> GetOrderDraftFromBasketAsync(BasketData basket);
    }
}
