using System;
using System.Linq;
using CollegeEventsApi.Data;
using CollegeEventsApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CollegeEventsApi.Tests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
        
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));

            _connection = new SqliteConnection("Data Source=:memory:;Cache=Shared");
            _connection.Open();

            services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlite(_connection);
            });

           
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });

   
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

          
            db.Database.EnsureCreated();

            Seed(db);
        });
    }

    private static void Seed(AppDbContext db)
    {
  
        if (!db.Categories.Any())
        {
            db.Categories.Add(new Category { Id = 1, Name = "Seed Category" });
            db.SaveChanges();
        }

      
        if (!db.Venues.Any())
        {
            db.Venues.Add(new Venue { Id = 1, Name = "Seed Venue", Address = "DKIT" });
            db.SaveChanges();
        }

      
        if (!db.Students.Any())
        {
            db.Students.Add(new Student
            {
                Id = 1,
                StudentNumber = "STUDENT001",
                FullName = "Seed Student",
                Email = "seed@student.test",
                Role = "Admin",
                PasswordHash = new byte[] { 1, 2, 3 },
                PasswordSalt = new byte[] { 9, 8, 7 }
            });
            db.SaveChanges();
        }

  
        if (!db.Events.Any())
        {
            db.Events.Add(new Event
            {
                Id = 1,
                Title = "Seed Event",
                Description = "Seeded for tests",
                StartTime = DateTime.UtcNow.AddDays(7),
                EndTime = DateTime.UtcNow.AddDays(7).AddHours(2),
                Capacity = 50,
                CategoryId = 1,
                VenueId = 1,
                RegistrationCount = 0
            });
            db.SaveChanges();
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection?.Dispose();
            _connection = null;
        }
    }
}