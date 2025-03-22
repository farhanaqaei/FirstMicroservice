using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Console.WriteLine("-------- Database Connection Debug Info --------");
// Console.WriteLine($"Raw Connection String: {builder.Configuration.GetConnectionString("PlatformsConn")}");
// Console.WriteLine($"SA_PASSWORD Environment Variable: {Environment.GetEnvironmentVariable("SA_PASSWORD")}");
// Console.WriteLine("----------------------------------------------");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsProduction())
{
    Console.WriteLine("--> Using SQL Server Db");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        //opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
        opt.UseSqlServer($"Server=mssql-clusterip-srv,1433;Initial Catalog=platformsdb;User ID=sa;Password={Environment.GetEnvironmentVariable("SA_PASSWORD")};TrustServerCertificate=True;"));
}
else
{
    Console.WriteLine("--> Using InMem Db");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseInMemoryDatabase("InMem"));
}

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

Console.WriteLine("--> CommandService Endpoint: " + builder.Configuration["CommandService"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

PrepDb.PrepPopulaion(app, app.Environment.IsProduction());

app.Run();
