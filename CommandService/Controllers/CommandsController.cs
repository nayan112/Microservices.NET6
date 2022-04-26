using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
   [ApiController]
   [Route("api/c/platforms/{platformId}/[controller]")]
   public class CommandsController : ControllerBase
   {
      private readonly ICommandRepository _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepository repository,IMapper mapper)
      {
         _repository = repository;
         _mapper = mapper;
      }
      

      [HttpGet]
      public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
      {
         Console.WriteLine($"--> Getting CommandsForPlatform: {platformId}");
         if (!_repository.PlatformExists(platformId))
         {
            return NotFound();
         }
         var commands = _repository.GetCommandsForPlatform(platformId);
         return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
      }

      [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
      public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId,int commandId)
      {
         Console.WriteLine($"--> Getting CommandForPlatform, command id: {commandId}, platform id: {platformId}");
         if (!_repository.PlatformExists(platformId))
         {
            return NotFound();
         }
         var commandItem = _repository.GetCommand(platformId, commandId);
         if (commandItem != null)
         {
            return Ok(commandItem);
         }
         return NotFound();
      }

      [HttpPost]
      public ActionResult<CommandReadDto> CreateCommandforPlatform(int platformId, CommandCreateDto commandCreateDto)
      {
         Console.WriteLine($"--> Getting CreateCommandforPlatform, platform id: {platformId}");
         if (!_repository.PlatformExists(platformId))
         {
            return NotFound();
         }
         var command = _mapper.Map<Command>(commandCreateDto);
         try
         {
            _repository.CreateCommand(platformId, command);
            _repository.SaveChanges();
         }
         catch (System.Exception ex)
         {
            throw ex;
         }
         var commandReadDto = _mapper.Map<CommandReadDto>(command);
         return CreatedAtRoute(nameof(GetCommandForPlatform), new { platformId = platformId, commandId = commandReadDto.Id },commandReadDto);
      }

      // [HttpPut("{id}")]
      // public ActionResult Update(int id, CommandUpdateDto commandUpdateDto)
      // {
      //    var commandModel = _repository.GetById(id);
      //    if (commandModel == null)
      //    {
      //       return NotFound();
      //    }
      //    _repository.Update(id, commandUpdateDto);
      //    return NoContent();
      // }

      // [HttpDelete("{id}")]
      // public ActionResult Delete(int id)
      // {
      //    var commandModel = _repository.GetById(id);
      //    if (commandModel == null)
      //    {
      //       return NotFound();
      //    }
      //    _repository.Delete(id);
      //    return NoContent();
      // }
   } 
}