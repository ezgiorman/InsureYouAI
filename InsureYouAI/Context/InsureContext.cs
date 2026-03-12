using Microsoft.EntityFrameworkCore;

using InsureYouAI.Entities;
namespace InsureYouAI.Context;

public class InsureContext:DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=DESKTOP-TSGOESU;initial catalog=InsureDb;integrated security=true;trust server certificate=true");
    }

    public DbSet<About> Abouts  { get; set; }
    public DbSet<AboutItem> AboutItems { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<PricingPlan> PricingPlans { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<Testimonial> Testimonials { get; set; }
    public DbSet<TrailerVideo> TrailerVideos { get; set; }
}
