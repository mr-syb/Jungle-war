using System;
using System.Net;
using System.Net.Sockets;

namespace Tcp服务端
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //本机 127.0.0.1
            //IpAddress xxx.xx.xx.xx  ip地址
            //IpEndPoint xxx.xx.xx:port 端口号
            //方式一：IPAddress iPAddress = new IPAddress(new byte[]{127, 0, 0, 1});
            IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress,88);
            serverSocket.Bind(iPEndPoint);//绑定端口号
            serverSocket.Listen(0);
            Socket clientSocket = serverSocket.Accept();//接受一个客户端链接

            //向客户端发送一条消息
            string msg = "Hello client!你好。。。。";
            byte[] data = System.Text.Encoding.UTF8.GetBytes(msg);
            clientSocket.Send(data);

            //接受客户端的一条消息
            byte[] dataBuffer = new byte[1024];
            int count = clientSocket.Receive(dataBuffer);
            string msgReceive = System.Text.Encoding.UTF8.GetString(dataBuffer,0, count);
            Console.WriteLine(msgReceive);

            clientSocket.Close();
            serverSocket.Close();
        }
    }
}
