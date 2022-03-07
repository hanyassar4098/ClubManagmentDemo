using DAL.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface ISubscribtionsRepository : IRepository<Subscribtion>
    {
        Task<bool> SubscribtionsAsync(ApplicationUser user, IEnumerable<string> services);
        Task<bool> SubscribtionsdeleteAsync(ApplicationUser user, IEnumerable<string> services);
        Task<List<string>> GetServicesAsync(ApplicationUser user);
        Task<List<Service>> GetServicesLoadRelatedAsync(int page, int pageSize);

       
        Task<(bool Succeeded, string[] Errors)> ServiceAddAsync(Service newservice);
        Task<Service> GetserviceRelatedAsync(string serviceName);

    }
}
