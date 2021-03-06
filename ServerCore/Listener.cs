﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Listener
    {
        Socket _listenSocket;
        Action<Socket> _onAcceptHandler; //연결성공시 콜백함수

        public void Init(IPEndPoint endPoint, Action<Socket> onAcceptHandler)
        {
            //LISTEN SOCKET
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _onAcceptHandler += onAcceptHandler;
            //bind
            _listenSocket.Bind(endPoint);

            //최대 대기수
            _listenSocket.Listen(10);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed +=new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            RegisterAccept(args);
        }
        void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            bool pending=_listenSocket.AcceptAsync(args);
            if (pending == false)
                OnAcceptCompleted(null,args);
        }
        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                _onAcceptHandler.Invoke(args.AcceptSocket);
            }else
                Console.WriteLine(args.SocketError.ToString());
            RegisterAccept(args);
        }
       
    }
}
