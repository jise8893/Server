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

               
                Session session = new Session();
                session.Start(clientsocket);

                byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!"); 
                session.Send(sendBuff);
                Thread.Sleep(1000);
                session.Disconnect();
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
