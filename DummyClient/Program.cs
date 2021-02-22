using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //DNS
            string host = Dns.GetHostName();
            IPHostEntry iphost = Dns.GetHostEntry(host);
            IPAddress ipaddr = iphost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipaddr, 7777);
            while (true)
            {
                //설정
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    //연결
                    socket.Connect(endPoint);

                    Console.WriteLine($"Connected To {socket.RemoteEndPoint.ToString()}");

                    //보내기
                    byte[] sendBuff = Encoding.UTF8.GetBytes("Hello World");
                    int sendbytes = socket.Send(sendBuff);

                    //받기
                    byte[] recvBuff = new byte[1024];
                    int recvbytes = socket.Receive(recvBuff);
                    string recvdata = Encoding.UTF8.GetString(recvBuff);
                    Console.WriteLine($"From Server:{recvdata}");

                    //socket exit
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                Thread.Sleep(100);
            }
          
        }
    }
}
