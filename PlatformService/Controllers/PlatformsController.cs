using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataService;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataService.Http;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IPlatformRepository platformRepository, IMapper mapper, ICommandDataClient commandDataClient, IMessageBusClient messageBusClient)
        {
            _platformRepository = platformRepository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }
      

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            var platformItems = _platformRepository.GetPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }

        [HttpGet("{Id=id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int Id)
        {
            var platformItem = _platformRepository.GetPlatform(Id);
            if (platformItem == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<PlatformReadDto>(platformItem));
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platform)
        {
            var platformmodel = _mapper.Map<Platform>(platform);
            _platformRepository.CreatePlatform(platformmodel);
            _platformRepository.SaveChanges();
            var platformReadDto = _mapper.Map<PlatformReadDto>(platformmodel);
            //Sync message
            try 
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($" could not sync :{ex.Message}");
            }
            //Async message
            try
            {
                var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
                platformPublishedDto.Event = "Platform_Published"; 
                _messageBusClient.PublishNewPlatform(platformPublishedDto);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($" could not send async message:{ex.Message}");
            }
            
            return CreatedAtRoute(nameof(GetPlatformById),new { Id = platformReadDto.Id}, platformReadDto);
        }
    }
}