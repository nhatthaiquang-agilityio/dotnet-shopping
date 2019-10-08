using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GRPCPackage;
using Grpc.Core;

namespace dotnet_gRPC.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        public GreeterService(ILogger<GreeterService> logger)
        {
        }

        public override Task<HelloReply> SayHello(
            HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        //public override Task<HelloReply> SayHello(
        //    HelloRequest request, ServerCallContext context)
        //{
        //    var httpContext = context.GetHttpContext();
        //    var clientCertificate = httpContext.Connection.ClientCertificate;

        //    return Task.FromResult(new HelloReply
        //    {
        //        Message = "Hello " + request.Name + " from " + clientCertificate.Issuer
        //    });
        //}
    }
}