using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataService.Grpc
{
    public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IMapper _mapper;

        public GrpcPlatformService(IPlatformRepository platformRepository, IMapper mapper)
        {
            _platformRepository = platformRepository;
            _mapper = mapper;
        }

        public override Task<PlatformResponse> GetPlatforms(GetAllRequest request, ServerCallContext context)
        {
            var platforms =  _platformRepository.GetPlatforms();
            var response = new PlatformResponse();
            foreach (var plat in platforms)
            {
                response.Platform.Add(_mapper.Map<GrpcPlatformModel>(plat));
            }
            return Task.FromResult(response);
        }
    }
}