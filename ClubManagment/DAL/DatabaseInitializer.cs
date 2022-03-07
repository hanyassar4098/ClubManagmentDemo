using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Core.Interfaces;

namespace DAL
{
    public interface IDatabaseInitializer
    {
        Task SeedAsync();
    }


    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountManager _accountManager;
        private readonly ILogger _logger;

        public DatabaseInitializer(ApplicationDbContext context, IAccountManager accountManager, ILogger<DatabaseInitializer> logger)
        {
            _accountManager = accountManager;
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync().ConfigureAwait(false);

           if ( !await _context.Service.AnyAsync())
            {
                _logger.LogInformation("Seeding initial data");
               
                Service serv_1 = new Service
                {
                    Name = "Foot Ball",
                    Description = "Yet another masterpiece from the world's best car manufacturer",
                    Price = 114234,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                };

                Service serv_2 = new Service
                {
                    Name = "Basket Ball",
                    Description = "A true man's choice",
                    //BuyingPrice = 78990,
                    Price = 86990,
                    //UnitsInStock = 4,
                    //IsActive = true,
                    //ProductCategory = servCat_1,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                };


                _context.Service.Add(serv_1);
                _context.Service.Add(serv_2);               

                await _context.SaveChangesAsync();

                _logger.LogInformation("Seeding initial data completed");
            }

            if (!await _context.Users.AnyAsync())
            {
                _logger.LogInformation("Generating inbuilt accounts");

                const string adminRoleName = "administrator";
                const string userRoleName = "user";

                const string adminservice = "Foot Ball";
                const string userservice = "Basket Ball";

                await EnsureRoleAsync(adminRoleName, "Default administrator", ApplicationPermissions.GetAllPermissionValues());
                await EnsureRoleAsync(userRoleName, "Default user", new string[] { });

                await CreateUserAsync("admin", "tempP@ss123", "Inbuilt Administrator", "admin@suez.com", "+1 (123) 000-0000", new string[] { adminRoleName }, new string[] { adminservice });
                await CreateUserAsync("user", "tempP@ss123", "Inbuilt Standard User", "user@suez.com", "+1 (123) 000-0001", new string[] { userRoleName }, new string[] { userservice });

                _logger.LogInformation("Inbuilt account generation completed");
            }

            //if (!await _context.Service.AnyAsync())
            //{
            //    _logger.LogInformation("Seeding initial data");


            //    Subscribtion subscrip_1 = new Subscribtion
            //    {
            //        Subscriber = await _context.Users.FirstAsync(),
            //        DateCreated = DateTime.UtcNow,
            //        DateModified = DateTime.UtcNow,
            //        Service = serv_1,
            //        //SubscribtionDetail = new List<SubscribtionDetail>()
            //        //{
            //        //    new SubscribtionDetail() {UnitPrice = serv_1.SellingPrice, Quantity=1, Service = serv_1 },
            //        //    new SubscribtionDetail() {UnitPrice = serv_2.SellingPrice, Quantity=1, Service = serv_2 },
            //        //}
            //    };

            //    Subscribtion subscrip_2 = new Subscribtion
            //    {
            //        Subscriber = await _context.Users.FirstAsync(),
            //        //Customer = cust_2,
            //        DateCreated = DateTime.UtcNow,
            //        DateModified = DateTime.UtcNow,
            //        Service = serv_2,
            //        //SubscribtionDetail = new List<SubscribtionDetail>()
            //        //{
            //        //    new SubscribtionDetail() {UnitPrice = serv_2.SellingPrice, Quantity=1, Service = serv_2 },
            //        //}
            //    };


            //    //_context.Customers.Add(cust_1);
            //    //_context.Customers.Add(cust_2);
            //    //_context.Customers.Add(cust_3);
            //    //_context.Customers.Add(cust_4);

            //    _context.Service.Add(serv_1);
            //    _context.Service.Add(serv_2);

            //    _context.Subscribtion.Add(subscrip_1);
            //    _context.Subscribtion.Add(subscrip_2);

            //    await _context.SaveChangesAsync();

            //    _logger.LogInformation("Seeding initial data completed");
            //}

        }



        private async Task EnsureRoleAsync(string roleName, string description, string[] claims)
        {
            if ((await _accountManager.GetRoleByNameAsync(roleName)) == null)
            {
                ApplicationRole applicationRole = new ApplicationRole(roleName, description);

                var result = await this._accountManager.CreateRoleAsync(applicationRole, claims);

                if (!result.Succeeded)
                    throw new Exception($"Seeding \"{description}\" role failed. Errors: {string.Join(Environment.NewLine, result.Errors)}");
            }
        }

        private async Task<ApplicationUser> CreateUserAsync(string userName, string password, string fullName, string email, string phoneNumber, string[] roles, string[] services)
        {
            ApplicationUser applicationUser = new ApplicationUser
            {
                UserName = userName,
                FullName = fullName,
                Email = email,
                PhoneNumber = phoneNumber,
                EmailConfirmed = true,
                IsEnabled = true
            };

            var result = await _accountManager.CreateUserAsync(applicationUser, roles , services , password);

            if (!result.Succeeded)
                throw new Exception($"Seeding \"{userName}\" user failed. Errors: {string.Join(Environment.NewLine, result.Errors)}");


            return applicationUser;
        }
    }
}
