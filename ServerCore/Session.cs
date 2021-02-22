using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    class Session
    {
        Socket _socket;
        int _disconnected = 0;
        bool _pending = false;
        object _lock = new object();
        SocketAsyncEventArgs _sendargs = new SocketAsyncEventArgs();
        Queue<byte[]> _sendQueue= new Queue<byte[]>();
        public void Start(Socket socket)
        {
            _socket = socket;
            SocketAsyncEventArgs recvargs = new SocketAsyncEventArgs();
            recvargs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvComplete);
            recvargs.SetBuffer(new byte[1024],0,1024);


            _sendargs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendComplteted);
            RegisterRecv(recvargs);

        }
        public void Send(byte[] sendBuff)
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuff);
                if (_pending == false)
                {
                    RegisterSend();
                }
            }


        }
        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
        #region 네트워크통신
        void RegisterSend()
        {

            _pending = true;
            byte[] Buff=_sendQueue.Dequeue();
            _sendargs.SetBuffer(Buff, 0, Buff.Length);
            bool pending = _socket.SendAsync(_sendargs);
            if (pending == false)
              OnSendComplteted(null, _sendargs);
        }
        void OnSendComplteted(object sender,SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        if (_sendQueue.Count > 0)
                        {
                            RegisterSend();
                        }
                        else
                            _pending = false;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{e}");
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }
        void RegisterRecv(SocketAsyncEventArgs recvargs)
        {
           
           bool pending= _socket.ReceiveAsync(recvargs);
            if (pending == false)
                OnRecvComplete(null, recvargs);
        }
        void OnRecvComplete(object sender,SocketAsyncEventArgs args)
        {
            if(args.BytesTransferred>0&&args.SocketError==SocketError.Success)
            {
                try
                {
                   
                    string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine($"[FROM CLIENT]:{recvData}");
                    RegisterRecv(args);

                } catch(Exception e)
                {
                    Console.WriteLine($"OnRecvComplteted Failed: {e}");
                }
             }
            else
            {
                Disconnect();
            }
        }
        #endregion
    }
}
