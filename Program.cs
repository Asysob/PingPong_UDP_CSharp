using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace PingPong_UDP_CSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                Server();
            else
                Client(args[0]);
        }

        private const int server_port = 4242;
        private const int buffer_length = 1024;

        public static void Server()
        {
            UdpClient sock = new UdpClient(server_port);
            bool running = true;
            while (running)
            {
                IPEndPoint client = new IPEndPoint(IPAddress.Any,0);
                Byte[] buffer = sock.Receive(ref client);
                string content = Encoding.ASCII.GetString(buffer);
                Console.WriteLine("Received: {0}", content);
                if (content.Equals("END"))
                    running = false;
                content += " BACK";
                buffer = Encoding.ASCII.GetBytes(content);
                sock.Send(buffer, Encoding.ASCII.GetByteCount(content),client);
            }
            sock.Close();
        }

        public static IPAddress GetIPAddress ( string name ) {
            IPHostEntry host_entry = Dns.GetHostEntry(name);
            IPAddress host_addr = null;
            if (host_entry == null)
                return null;
            foreach (IPAddress addr in host_entry.AddressList) {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {
                    host_addr = addr;
                    break;
                }
            }
            return host_addr;
        }

        public static void Client(string server_host)
        {
            Console.WriteLine("Connecting to host {0}", server_host);
            UdpClient sock = new UdpClient();
            IPAddress server_addr = GetIPAddress(server_host);
            IPEndPoint server = new IPEndPoint(server_addr, server_port);
            string message;
            Byte[] buffer;
            for (int m = 0; m < 10; m++)
            {
                message = "BALL " + m;
                buffer = Encoding.ASCII.GetBytes(message);
                sock.Send(buffer, Encoding.ASCII.GetByteCount(message), server);
                IPEndPoint dummy = null;
                buffer = sock.Receive(ref dummy);
                string content = Encoding.ASCII.GetString(buffer);
                Console.WriteLine("Received: {0}", content);
                Thread.Sleep(1000);
            }
            message = "END";
            buffer = Encoding.ASCII.GetBytes(message);
            sock.Send(buffer, Encoding.ASCII.GetByteCount(message), server);
            sock.Close();
        }
    }
}
