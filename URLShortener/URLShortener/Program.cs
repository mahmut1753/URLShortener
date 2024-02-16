using Microsoft.EntityFrameworkCore;
using URLShortener.Context;
using URLShortener.Entities;
using URLShortener.Extensions;
using URLShortener.Models;
using URLShortener.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<ContextBase>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddScoped<UrlShorteningService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.Migrations();
}

app.MapPost("api/shorten", async (
    UrlShortenRequest request,
    UrlShorteningService urlShorteningService,
    ContextBase contextBase,
    HttpContext httpContext
    ) =>
{
    if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
        return Results.BadRequest("The specified URL is invalid");

    var code = await urlShorteningService.GenerateShortUrlCode();

    ShortenedUrl shortUrl = new()
    {
        Id = Guid.NewGuid(),
        LongUrl = request.Url,
        Code = code,
        ShortUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/api/{code}",
        CreatedOnUtc = DateTime.UtcNow
    };

    contextBase.ShortenedUrls.Add(shortUrl);
    await contextBase.SaveChangesAsync();

    return Results.Ok(shortUrl.ShortUrl);
});

app.MapGet("api/{code}", async (string code, ContextBase contextBase) =>
{
    ShortenedUrl shortenedUrl = await contextBase.ShortenedUrls.FirstOrDefaultAsync(x => x.Code == code);

    if (shortenedUrl is null) return Results.NotFound();

    return Results.Redirect(shortenedUrl.LongUrl);
});

app.Run();
