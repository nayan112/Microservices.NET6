using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;
using PlatformService;

namespace CommandService.Profiles
{
    public class CommandProfile : Profile
    {
        public CommandProfile()
        {
            //Source -> Target
            CreateMap<Command, CommandCreateDto>();
            CreateMap<CommandCreateDto,Command>();
            CreateMap<Command, CommandReadDto>();
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<PlatformPublishedDto, Platform>()
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));
            CreateMap<GrpcPlatformModel, Platform>()
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.PlatformId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Commands, opt => opt.Ignore());
        }
    }
}