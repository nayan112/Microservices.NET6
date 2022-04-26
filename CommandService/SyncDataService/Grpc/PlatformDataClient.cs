using AutoMapper;
using CommandService.Models;
using Grpc.Core;
using Grpc.Net.Client;
using PlatformService;

namespace CommandService.SyncDataService.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public PlatformDataClient(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }
        public Task<IEnumerable<Platform>> ReturnAllPlatforms()
        {
            Console.WriteLine($" --> calling Grpc {_configuration["GrpcPlatform"]}");
            var channel = GrpcChannel.ForAddress(_configuration["GrpcPlatform"]);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();
            try
            {
                var response = client.GetPlatforms(request);
                var platforms = _mapper.Map<IEnumerable<Platform>>(response.Platform);
                return Task.FromResult(platforms);
            }
            catch (RpcException ex)
            {
                Console.WriteLine($" --> Error calling Grpc {_configuration["GrpcPlatform"]}");
                Console.WriteLine($" --> {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($" --> Error calling Grpc {_configuration["GrpcPlatform"]}. Error: {ex.Message}");
                return null;
            }
        }
    }
}