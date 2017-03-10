using System;
using System.Collections.Generic;
using System.IO;
using ConsoleApplication;
using Grpc.Core;
using static ConsoleApplication.Messages.UserService;

namespace Grpc.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            const int port = 9000;
            var cacert = File.ReadAllText("Keys/ca.crt");
            var cert = File.ReadAllText("Keys/server.crt");
            var key = File.ReadAllText("Keys/server.key");

            var keypair = new KeyCertificatePair(cert, key);
            var sslCreds = new SslServerCredentials(new List<KeyCertificatePair>
            {
                keypair
            }, cacert, false);

            Grpc.Core.Server server = new Grpc.Core.Server
            {
                Ports = { new ServerPort("0.0.0.0", port, sslCreds) },
                Services = { BindService(new UsersService()) }
            };

            server.Start();

            Console.WriteLine("Starting server on port " + port);
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
        }
    }
}
