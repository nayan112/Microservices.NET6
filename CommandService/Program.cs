using CommandService.AsyncDataService;
using CommandService.Data;
using CommandService.EventProcessor;
using CommandService.SyncDataService.Grpc;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
var _env = builder.Environment;
ConfigurationManager configuration = builder.Configuration;
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPlatformDataClient,PlatformDataClient>();
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("CommandDB"));
builder.Services.AddScoped<ICommandRepository, CommandRepository>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddHostedService<MessageBusSubscriber>();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
PreparationDb.PrePopulationDb(app);
app.Run();
