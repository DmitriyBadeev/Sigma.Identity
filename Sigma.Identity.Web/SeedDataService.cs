using System;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Sigma.Identity.Web.Data;
using Sigma.Identity.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Sigma.Identity.Web
{
    public class SeedDataService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly PersistedGrantDbContext _persistedGrantDbContext;
        private readonly ConfigurationDbContext _configurationDbContext;

        public SeedDataService(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext, 
            PersistedGrantDbContext persistedGrantDbContext, ConfigurationDbContext configurationDbContext)
        {
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
            _persistedGrantDbContext = persistedGrantDbContext;
            _configurationDbContext = configurationDbContext;
        }
        
        public void EnsureSeedData()
        {
            if (_applicationDbContext.Database.GetPendingMigrations().Any())
            {
                Log.Information("Migrating identity database");
                _applicationDbContext.Database.Migrate();
                Log.Information("Database identity has migrated successfully");
            }

            if (_persistedGrantDbContext.Database.GetPendingMigrations().Any())
            {
                Log.Information("Migrating persisted grant database");
                _persistedGrantDbContext.Database.Migrate();
                Log.Information("Database persisted grant has migrated successfully");
            }

            if (_configurationDbContext.Database.GetPendingMigrations().Any())
            {
                Log.Information("Migrating configuration database");
                _configurationDbContext.Database.Migrate();
                Log.Information("Database configuration has migrated successfully");
            }
            
            var testUser = _userManager.FindByNameAsync("test").Result;
            if (testUser == null)
            {
                Log.Debug("Creating test user...");
                
                testUser = new ApplicationUser
                {
                    UserName = "test",
                    Email = "test@gmail.com",
                    EmailConfirmed = true,
                };
                
                var result = _userManager.CreateAsync(testUser, "12345").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                
                Log.Debug("Test user created");
            }
            else
            {
                Log.Debug("Test user already exists");
            }
            
            if (!_configurationDbContext.Clients.Any())
            {
                Log.Information("Adding clients...");
                foreach (var client in Config.GetSpaClient())
                {
                    _configurationDbContext.Clients.Add(client.ToEntity());
                }
                _configurationDbContext.SaveChanges();
                Log.Information("Clients added");
            }

            if (!_configurationDbContext.IdentityResources.Any())
            {
                Log.Information("Adding IdentityResources...");
                foreach (var resource in Config.GetIdentityResources())
                {
                    _configurationDbContext.IdentityResources.Add(resource.ToEntity());
                }
                _configurationDbContext.SaveChanges();
                Log.Information("IdentityResources added");

            }
            
            if (!_configurationDbContext.ApiScopes.Any())
            {
                Log.Information("Adding ApiScopes...");
                foreach (var resource in Config.GetApiScope())
                {
                    _configurationDbContext.ApiScopes.Add(resource.ToEntity());
                }
                _configurationDbContext.SaveChanges();
                
                Log.Information("ApiScopes added");
            }
            
            if (!_configurationDbContext.ApiResources.Any())
            {
                Log.Information("Adding ApiResources...");
                foreach (var resource in Config.GetApiResource())
                {
                    _configurationDbContext.ApiResources.Add(resource.ToEntity());
                }
                _configurationDbContext.SaveChanges();
                
                Log.Information("ApiResources added");
            }
        }
    }
}
