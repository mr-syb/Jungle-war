using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Tcp服务端
{
    class Program
    {
        static void Main(string[] args)
        {
            StartServerAsync();
            Console.ReadKey();
        }

        static void StartServerSync() {           
                
            /*同步的方式进行交互*/

            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //本机 127.0.0.1
            //IpAddress xxx.xx.xx.xx  ip地址
            //IpEndPoint xxx.xx.xx:port 端口号
            //方式一：IPAddress iPAddress = new IPAddress(new byte[]{127, 0, 0, 1});
            IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, 88);
            serverSocket.Bind(iPEndPoint);//绑定端口号
            serverSocket.Listen(0);
            Socket clientSocket = serverSocket.Accept();//接受一个客户端链接

            //向客户端发送一条消息
            string msg = "Hello client!你好。。。。";
            byte[] data = System.Text.Encoding.UTF8.GetBytes(msg);
            clientSocket.Send(data);

            //接受客户端的一条消息--同步
            byte[] dataBuffer = new byte[1024];
            int count = clientSocket.Receive(dataBuffer);
            string msgReceive = System.Text.Encoding.UTF8.GetString(dataBuffer, 0, count);
            Console.WriteLine(msgReceive);

            Console.ReadKey();

            clientSocket.Close();
            serverSocket.Close();
        }

        static byte[] dataBuffer = new byte[1024];

        static void StartServerAsync() {
            /*异步的方式进行交互*/

            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //本机 127.0.0.1
            //IpAddress xxx.xx.xx.xx  ip地址
            //IpEndPoint xxx.xx.xx:port 端口号
            //方式一：IPAddress iPAddress = new IPAddress(new byte[]{127, 0, 0, 1});
            IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, 88);
            serverSocket.Bind(iPEndPoint);//绑定端口号
            serverSocket.Listen(0);//开始监听

            //Socket clientSocket = serverSocket.Accept();//接受一个客户端链接----同步的方式
            serverSocket.BeginAccept(AcceptCollBack, serverSocket); //最后一个参数目的是将将serverSocket当做参数传递到callback函数里

        }

        static void AcceptCollBack(IAsyncResult ar) {

            Socket serverSocket = ar.AsyncState as Socket;
            Socket clientSocket = serverSocket.EndAccept(ar);
            //向客户端发送一条消息
            string msg = "Hello client!你好。。。。";
            byte[] data = Encoding.UTF8.GetBytes(msg);
            clientSocket.Send(data);

            //接受客户端的一条消息——异步
            clientSocket.BeginReceive(dataBuffer, 0, 1024, SocketFlags.None, ReceiveCallBack, clientSocket);//最后一位表示回调函数的参数，可在回调函数中由 ar.AsyncState接收
            //clientSocket.BeginReceive异步函数不会影响到下面的语句执行

            //循环处理客户端的连接
            serverSocket.BeginAccept(AcceptCollBack, serverSocket);//最后一个参数目的是将将serverSocket当做参数传递到callback函数里
        }

        static void ReceiveCallBack(IAsyncResult ar)
        {
            Socket clientSocket  = null;
            //对客户端的非正常断开做异常处理
            try
            {
                clientSocket = ar.AsyncState as Socket;
                int count = clientSocket.EndReceive(ar);//与之前的clientSocket.BeginReceive（）对应,返回值表示接收到的字节数
                if (count == 0)
                {
                    clientSocket.Close();
                    return;
                }
                string msg = Encoding.UTF8.GetString(dataBuffer, 0, count);
                Console.WriteLine("从客户端接收到数据：" + msg);

                //循环接收
                clientSocket.BeginReceive(dataBuffer, 0, 1024, SocketFlags.None, ReceiveCallBack, clientSocket);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //出现异常时断开连接
                if (clientSocket != null)
                {
                    clientSocket.Close();
                }
            }
            //finally
            //{
            //}
                
        }
    }
}


