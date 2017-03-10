using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApplication.Messages;
using Google.Protobuf;
using Grpc.Core;
using static ConsoleApplication.Messages.UserService;

namespace Grpc.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const int port = 9000;
            var cacert = File.ReadAllText("Keys/ca.crt");
            var cert = File.ReadAllText("Keys/client.crt");
            var key = File.ReadAllText("Keys/client.key");

            var keypair = new KeyCertificatePair(cert, key);
            var sslCreds = new SslCredentials(cacert, keypair);

            var channel = new Channel("Alexiss-MacBook-Pro.local", port, sslCreds);

            var client = new UserServiceClient(channel);

            Console.WriteLine("--- UNARY CALL ---------------------");
            GetByUserIdAsync(client).Wait();
            Console.WriteLine("------------------------------------");
            Console.WriteLine("--- SERVER STREAMING CALL ----------");
            GetAllAsync(client).Wait();
            Console.WriteLine("------------------------------------");
            Console.WriteLine("--- CLIENT STREAMING CALL ----------");
            AddImageAsync(client).Wait();
            Console.WriteLine("------------------------------------");
            Console.WriteLine("--- BIDIRECTIONAL STREAMING CALL ---");
            SaveAllAsync(client).Wait();
            Console.WriteLine("------------------------------------");
        }

        private static async Task GetByUserIdAsync(UserServiceClient client)
        {
            var md = new Metadata();

            md.Add("Username", "Alexis");
            md.Add("password", "12345");

            var res = await client.GetByUserIdAsync(new GetByUserIdRequest()
            {
                UserId = 1
            }, md);

            Console.WriteLine(res.User);
        }

        private static async Task GetAllAsync(UserServiceClient client)
        {
            using (var call = client.GetAll(new GetAllRequest()))
            {
                var resStream = call.ResponseStream;
                while (await resStream.MoveNext(CancellationToken.None))
                {
                    Console.WriteLine(resStream.Current.User);
                }
            }
        }

        private static async Task AddImageAsync(UserServiceClient client)
        {
            var md = new Metadata();
            md.Add("UserId", "1");

            FileStream fs = File.OpenRead("Images/NodeMcu.jpg");

            using (var call = client.AddImage(md))
            {
                var stream = call.RequestStream;

                while (true)
                {
                    byte[] buffer = new byte[64 * 1024];
                    int numRead = await fs.ReadAsync(buffer, 0, buffer.Length);

                    if (numRead == 0)
                        break;

                    if (numRead < buffer.Length)
                        Array.Resize(ref buffer, numRead);

                    await stream.WriteAsync(new AddImageRequest()
                    {
                        Data = ByteString.CopyFrom(buffer)
                    });
                }
                await stream.CompleteAsync();

                var res = await call.ResponseAsync;
                Console.WriteLine(res.IsOk);
            }
        }

        private static async Task SaveAllAsync(UserServiceClient client)
        {
            var u1 = new User
            {
                Id = 4,
                FirstName = "Luke",
                LastName = "Skywalker",
                Birthday = new DateTime(1977, 11, 22).Ticks
            };
            u1.Vehicles.Add(new Vehicle { Id = 7, RegNumber = "SJDKJSDKJI200" });
            u1.Vehicles.Add(new Vehicle { Id = 8, RegNumber = "WIUDSBDJK9328" });

            var users = new List<User> { u1 };

            using (var call = client.SaveAll())
            {
                var reqStream = call.RequestStream; var resStream = call.ResponseStream;

                var resTask = Task.Run(async () =>
                {
                    while (await resStream.MoveNext(CancellationToken.None))
                        Console.WriteLine($"Saved: {resStream.Current.User}");
                });

                foreach (var u in users)
                    await reqStream.WriteAsync(new UserRequest
                    {
                        User = u
                    });

                await call.RequestStream.CompleteAsync();
                await resTask;
            }
        }
    }
}
