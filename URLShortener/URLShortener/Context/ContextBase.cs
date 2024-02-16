using Microsoft.EntityFrameworkCore;
using URLShortener.Entities;
using URLShortener.Services;

namespace URLShortener.Context;

public class ContextBase : DbContext
{
    public ContextBase(DbContextOptions options): base(options)
    {        
    }

    public DbSet<ShortenedUrl> ShortenedUrls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShortenedUrl>(builder =>
        {
            builder.Property(s => s.Code).HasMaxLength(UrlShorteningService.NumberOfCharsInShortLink);
            builder.HasIndex(s => s.Code).IsUnique();
        });

        base.OnModelCreating(modelBuilder); 
    }
}
