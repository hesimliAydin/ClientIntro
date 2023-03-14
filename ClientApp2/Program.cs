using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp2
{
    public class Program
    {
        static void Main(string[] args)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var ipAddress = IPAddress.Parse("192.168.100.155");
            var port = 27001;

            var endPoint = new IPEndPoint(ipAddress, port);

            try
            {
                socket.Connect(endPoint);
                if (socket.Connected)
                {
                    Console.WriteLine("Connected to the server");
                    var sender = Task.Run(() =>
                    {
                        while (true)
                        {
                            var msg = Console.ReadLine();
                            var bytes = Encoding.UTF8.GetBytes(msg);
                            socket.Send(bytes);
                        }
                    });

                    var receiver = Task.Run(() =>
                    {
                        var length = 0;
                        var bytes = new byte[1024];
                        do
                        {
                            length = socket.Receive(bytes);
                            if (length > 0)
                            {
                                var msg = Encoding.UTF8.GetString(bytes, 0, length);
                                Console.WriteLine(msg);
                            }
                        }
                        while (true);
                    });

                    Task.WaitAll(receiver, sender);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Can not connect to the server");
            }
        }
    }

}
