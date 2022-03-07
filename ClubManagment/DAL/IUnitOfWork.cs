using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IUnitOfWork
    {
        //ICustomerRepository Customers { get; }
        IServiceRepository Services { get; }
        ISubscribtionsRepository Subscribtions { get; }
        
        int SaveChanges();
    }
}
