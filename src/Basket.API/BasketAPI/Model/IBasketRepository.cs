using System.Collections.Generic;
using System.Threading.Tasks;

namespace BasketAPI.Model
{
    public interface IBasketRepository
    {
        Task<CustomerBasket> GetBasketAsync(string customerId);

        IEnumerable<string> GetUsers();

        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);

        Task<CustomerBasket> SaveEventBasketAsync(int productId, decimal newPrice, decimal oldPrice);

        Task<bool> DeleteBasketAsync(string id);
    }
}
