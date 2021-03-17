using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrdersHandler.DataAccess
{
    public interface IDbDataAccess
    {
        Task<IList<T>> LoadDataAsync<T>(string sql);
        Task<T> LoadDataAsync<T>(string sql, int orderId);
        Task UpdateDataAsync<T>(string sql, T order);
        Task<int> InsertDataAsync<T>(string sql, T order);
    }
}