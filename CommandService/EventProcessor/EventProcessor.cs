using System.Text.Json;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.EventProcessor
{
    public class EventProcessor: IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;
            public EventProcessor(IServiceScopeFactory scopeFactory,IMapper mapper)
            {
                _scopeFactory = scopeFactory;
                _mapper = mapper;
            }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);
            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
        }
        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("  --> Determining Event..");
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
            switch (eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("  --> Platform_Published event detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("  --> Unknown event detected");
                    return EventType.Undetermined;
            }
        }
        private void AddPlatform( string PlatformPublishedMessage )
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(PlatformPublishedMessage);
                try
                {
                    var platform = _mapper.Map<Platform>(platformPublishedDto);
                    if (!repo.ExternalPlatformExists(platform.ExternalId))
                    {
                        repo.CreatePlatform(platform);
                        repo.SaveChanges();
                    }
                   
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"  --> Error adding platform: {ex.Message}");
                }
            }        
        }
    }
    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}