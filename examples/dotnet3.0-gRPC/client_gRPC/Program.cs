using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using GRPCPackage;

namespace client_gRPC
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting GRPC Client...");
            // https://docs.microsoft.com/en-us/aspnet/core/grpc/troubleshoot?view=aspnetcore-3.0
            // This switch must be set before creating the GrpcChannel/HttpClient.
            AppContext.SetSwitch(
                "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var greeterClient = new Greeter.GreeterClient(channel);

            try
            {
                var response = await greeterClient.SayHelloAsync(
                    new HelloRequest { Name = "World" });
                Console.WriteLine(response.Message);
            }
            catch (RpcException ex)
            {
                Console.WriteLine("Error:");
                Console.WriteLine(ex.Status.Detail);
            }

            Console.WriteLine("Shutting down");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

    }
}
