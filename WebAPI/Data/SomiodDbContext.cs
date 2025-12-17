using System.Data.Entity;
using WebAPI.Models;

namespace WebAPI.Data
{
    public class SomiodDbContext : DbContext
    {
        public SomiodDbContext() : base("SomiodConnectionString") { }

        public DbSet<Application> Applications { get; set; }
        public DbSet<Container> Containers { get; set; }
        public DbSet<ContentInstance> ContentInstances { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
    }
}
