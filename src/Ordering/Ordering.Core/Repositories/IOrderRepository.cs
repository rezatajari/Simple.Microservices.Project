using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ordering.Core.Entities;
using Ordering.Core.Repositories.Base;

namespace Ordering.Core.Repositories
{
    public interface IOrderRepository:IRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrderByUserName(string userName);
    }
}
