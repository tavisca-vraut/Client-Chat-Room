using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleClient
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            IPAddress ip = IPAddress.Parse("172.16.5.199");
            int port = 5000;
            TcpClient client = new TcpClient();

            Console.WriteLine("Establishing connection...");
            client.Connect(ip, port);
            Console.WriteLine("Connected to the server..\n");

            NetworkStream ns = client.GetStream();

            Console.WriteLine("Enter your name: ");
            byte[] buffer = Encoding.ASCII.GetBytes(Console.ReadLine());
            ns.Write(buffer, 0, buffer.Length);

            Console.WriteLine("Connected to the Chat Room. Type and hit Enter to broadcast your message.\n");
            
            var clientTask = Task.Factory.StartNew(() => ReceiveData(client));

            string s;
            while (true)
            {
                s = Console.ReadLine();
                if (s == "") continue;
                if (s.ToLower() == "exit") break;

                buffer = Encoding.ASCII.GetBytes(s);
                ns.Write(buffer, 0, buffer.Length);
            }

            buffer = Encoding.ASCII.GetBytes(s);
            ns.Write(buffer, 0, buffer.Length);

            client.Client.Shutdown(SocketShutdown.Send);
            clientTask.Wait();
            ns.Close();
            client.Close();
            Console.WriteLine("disconnect from server!!");
            Console.ReadKey();
        }

        static void ReceiveData(TcpClient client)
        {
            NetworkStream ns = client.GetStream();
            byte[] receivedBytes = new byte[1024];
            int byte_count;

            while ((byte_count = ns.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
            {
                Console.Write(Encoding.ASCII.GetString(receivedBytes, 0, byte_count));
            }
        }
    }
}

