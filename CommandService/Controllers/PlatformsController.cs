using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController:ControllerBase
    {
        private readonly ICommandRepository _repository;
        private readonly IMapper _mapper;

        public PlatformsController(ICommandRepository repository,IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetAll()
        {
            Console.WriteLine("-->Getting platforms from command Service");
            var platformItems = _repository.GetPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }

        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine(" --> Sync TestInboundConnection working..");
            return Ok("PlatformsController.TestInboundConnection");
        }
    }
}