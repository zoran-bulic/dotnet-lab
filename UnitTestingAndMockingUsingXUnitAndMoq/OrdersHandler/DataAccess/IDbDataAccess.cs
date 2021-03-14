using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrdersHandler.DataAccess
{
    public interface IDbDataAccess
    {
        List<T> LoadData<T>(string sql);
        T LoadData<T>(string sql, int orderId);
        void SaveData<T>(string sql, T order);
        void UpdateData<T>(string sql, T order);

        Task<T> LoadDataAsync<T>(string sql, int orderId);
        Task UpdateDataAsync<T>(string sql, T order);
        Task<int> InsertDataAsync<T>(string sql, T order);
    }
}