using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Repositories.Interfaces;

namespace DAL.Repositories
{
    public class SubscribtionRepository : Repository<Subscribtion>, ISubscribtionsRepository
    {
        public SubscribtionRepository(DbContext context) : base(context)
        { }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;

        public Task<List<string>> GetServicesAsync(ApplicationUser user)
        {
            var result = from b in _appContext.Service
                         from sub in _appContext.Subscribtion
                         where sub.ServiceId == b.Id && sub.SubscriberId == user.Id
                         select b.Name;
            return result.ToListAsync();           

        }

        public async Task<bool> SubscribtionsAsync(ApplicationUser user, IEnumerable<string> services)
        {
            var subscribtions = services.Join(_appContext.Service,
                 s => s,
                 sa => sa.Name,
                 (s, sa) => new Subscribtion
                 {
                     ServiceId = sa.Id,
                     SubscriberId = user.Id,
                     DateCreated = DateTime.UtcNow,
                     DateModified = DateTime.UtcNow
                      }).ToList();

            _appContext.Subscribtion.AddRange(subscribtions);

            _appContext.SaveChanges();

            return true;

        }
        public async Task<bool> SubscribtionsdeleteAsync(ApplicationUser user, IEnumerable<string> services)
        {

            var servicesIDs = await _appContext.Service
                .Where(r => services.Contains(r.Name))
                .Select(r => r.Id)
                .ToArrayAsync();

           
                var list = _appContext.Subscribtion.Where(c => servicesIDs.Contains(c.ServiceId) && c.SubscriberId == user.Id).ToList();
                   
                    _appContext.Subscribtion.RemoveRange(list);
                    _appContext.SaveChanges();
            

            //var toRemoveList = (from a in services
            //        from b in _appContext.Service
            //        from sub in _appContext.Subscribtion
            //        where a == b.Name && sub.ServiceId == b.Id && sub.SubscriberId == user.Id
            //        select new Subscribtion { }).ToList();


            //_appContext.Subscribtion.RemoveRange(toRemoveList);

            //_appContext.SaveChanges();

            return true;

           

        }
        public async Task<List<Service>> GetServicesLoadRelatedAsync(int page, int pageSize)
        {
            IQueryable<Service> servicesQuery = _appContext.Service
                .OrderBy(r => r.Name);

            if (page != -1)
                servicesQuery = servicesQuery.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                servicesQuery = servicesQuery.Take(pageSize);

            var services = await servicesQuery.ToListAsync();

            return services;
        }

        public async Task<(bool Succeeded, string[] Errors)> ServiceAddAsync(Service newservice)
        {

            await _appContext.Set<Service>().AddAsync(newservice);
            _appContext.SaveChanges();
            return (true, new string[] { });

        }

        public async Task<Service> GetserviceRelatedAsync(string serviceName)
        {
            var service = await _appContext.Service                
                .Where(r => r.Name == serviceName)
                .SingleOrDefaultAsync();

            return service;
        }


    }
}
