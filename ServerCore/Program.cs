using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
   
    class Program
    {
        static Listener _listener = new Listener();
        static void OnAcceptHandler(Socket clientsocket)
        {
            try
            {

                //받기
                byte[] recvBuff = new byte[1024];
                int recvbyte = clientsocket.Receive(recvBuff);
                string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvbyte);  //버퍼,시작주소인덱스,받은 바이트수
                Console.WriteLine($"[FROM CLIENT]{recvData}");

                //보낸다
                byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
                clientsocket.Send(sendBuff);

                //연결종료
                clientsocket.Shutdown(SocketShutdown.Both);
                clientsocket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        static void Main(string[] args)
        {
         
            //DNS
            string host = Dns.GetHostName();
            IPHostEntry iphost=Dns.GetHostEntry(host);
            IPAddress ipaddr=iphost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipaddr,7777);


            _listener.Init(endPoint, OnAcceptHandler);
            Console.WriteLine("Listening...");
            while (true)
            {

            }

           
        }
    }
}
