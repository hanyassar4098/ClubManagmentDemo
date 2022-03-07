using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Repositories;
using DAL.Repositories.Interfaces;

namespace DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly ApplicationDbContext _context;

        IServiceRepository _services;
        ISubscribtionsRepository _subscriptions;
                

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }


        public IServiceRepository Services
        {
            get
            {
                if (_services == null)
                    _services = new ServiceRepository(_context);

                return _services;
            }
        }



        public ISubscribtionsRepository Subscribtions
        {
            get
            {
                if (_subscriptions == null)
                    _subscriptions = new SubscribtionRepository(_context);

                return _subscriptions;
            }
        }

       

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
