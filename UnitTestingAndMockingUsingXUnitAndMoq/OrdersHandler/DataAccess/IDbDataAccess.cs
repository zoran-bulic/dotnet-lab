using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersHandler.DataAccess
{
    public interface IDbDataAccess
    {
        List<T> LoadData<T>(string sql);
        T LoadData<T>(string sql, int orderId);
        void SaveData<T>(string sql, T order);
        void UpdateData<T>(string sql, T order);
    }
}
