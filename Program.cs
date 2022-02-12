using AspNetCoreRateLimit;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PhotoAPI.Data;
using PhotoAPI.Interfaces;
using PhotoAPI.Profiles;
using PhotoAPI.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Returning Base64 string is high payload operation. Due to this fact, client should retrieve the data with pagination.=>
//Using OData for pagination and querying. =>
builder.Services.AddControllers().AddOData(options=>options.SetMaxTop(10).Expand().Filter().OrderBy().SkipToken());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Photos API",
        Description = ".NET 6 Web API for IIASA Full-Stack Developer Challenge",
        Contact = new OpenApiContact
        {
            Name = "Umur Yuksel",
            Email = "dev.umuryksl@gmail.com",
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

//builder.Services.AddDbContext<PhotoDbContext>(options => options.UseSqlServer(@"Data Source=host.docker.internal,1401;User ID=sa;Password=TestP@ssw0rd;"));
builder.Services.AddDbContext<PhotoDbContext>(c => c.UseSqlServer(builder.Configuration.GetConnectionString("PhotosDbConnection")), ServiceLifetime.Singleton);

builder.Services.AddScoped<IPhoto, PhotoService>();
builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>((options) =>
{
    options.GeneralRules = new List<RateLimitRule>()
    {
        new RateLimitRule()
        {
            Endpoint= "*",
            Limit=10,
            Period="5m"
        }
    };
});
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My service");
    c.RoutePrefix = string.Empty;  // Set Swagger UI at apps root
});

app.UseIpRateLimiting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//Applying auto migration.
using (var scope = app.Services.CreateScope())
{
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<PhotoDbContext>();
        context.Database.Migrate();
}

app.Run();
