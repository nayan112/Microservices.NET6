using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataService;
using PlatformService.Data;
using PlatformService.SyncDataService.Grpc;
using PlatformService.SyncDataService.Http;

var builder = WebApplication.CreateBuilder(args);
var _env = builder.Environment;
ConfigurationManager configuration = builder.Configuration;
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddGrpc();
if(_env.IsProduction())
{
    Console.WriteLine("Using Production SQL Server");
    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("PlatformConnection")));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("PlatformService"));
}
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

PrepDB.PrepPopulation(app,_env.IsProduction());
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<GrpcPlatformService>();
app.MapGet("/protos/platforms.protos", async context =>
{
    await context.Response.WriteAsync(File.ReadAllText("protos/platforms.protos"));
});
Console.WriteLine($" --> CommandService endpoint: {configuration["CommandService"]}");
app.Run();
 