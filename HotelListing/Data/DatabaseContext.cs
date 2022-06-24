using HotelListing.Configurations.Entities;
using HotelListing.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class DatabaseContext : IdentityDbContext<ApiUser>
{
    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<Country> Countries { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //builder.Entity<Country>()
        //.HasMany(c => c.Hotels)
        //.WithOne()
        //.HasForeignKey(c => c.CountryId)
        //.OnDelete(DeleteBehavior.Restrict);

        builder.ApplyConfiguration(new RoleConfiguration());
        builder.ApplyConfiguration(new HotelConfiguartion());
        builder.ApplyConfiguration(new CountryConfiguration());
    }
}