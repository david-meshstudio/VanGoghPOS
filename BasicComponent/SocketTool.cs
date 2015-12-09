using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace COM.MeshStudio.Lib.BasicComponent
{
    public class SocketTool
    {
        public static MServerSocket FactoryGenerateServerTCPSocket()
        {
            MServerSocket result = new MServerSocket();
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            result.socket = socket;
            return result;
        }

        public static MClientSocket FactoryGenerateClientTCPSocket(string clientid)
        {
            MClientSocket result = new MClientSocket();
            result.clientid = clientid;
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            result.socket = socket;
            return result;
        }

        public class MServerSocket
        {
            public Socket socket;
            public delegate void ReceiveCallBack(string message);
            public ReceiveCallBack receiveCallBack;
            private bool Running = true;
            private Dictionary<string, Socket> clientList = new Dictionary<string, Socket>();

            public void Listen(string ip, int port, int queueLength)
            {
                socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
                Running = true;
                clientList = new Dictionary<string, Socket>();
                socket.Listen(queueLength);
                Thread myThread = new Thread(ListenClientConnect);
                myThread.Start();
            }

            public void Stop()
            {
                Running = false;
            }

            private void ListenClientConnect()
            {
                while(Running)
                {
                    Socket clientSocket = socket.Accept();
                    clientSocket.Send(Encoding.UTF8.GetBytes("connected"));
                    Thread receiveThread = new Thread(ReceiveMessage);
                    receiveThread.Start(clientSocket);
                }
                socket.Close();
                clientList.Clear();
            }

            private void ReceiveMessage(object socket)
            {
                Socket clientSocket = (Socket)socket;
                while(Running)
                {
                    try
                    {
                        byte[] result = new byte[10240];
                        int receiveNumber = clientSocket.Receive(result);
                        string message = Encoding.UTF8.GetString(result);
                        if(message == "close")
                        {
                            if(clientList.ContainsValue(clientSocket))
                            {
                                int index = clientList.Values.ToList<Socket>().IndexOf(clientSocket);
                                string key = clientList.Keys.ToList<string>()[index];
                                clientList.Remove(key);
                            }
                            clientSocket.Close();
                            break;
                        }
                        else if(message.StartsWith("register"))
                        {
                            string[] msgs = message.Split(new char[] { '|' });
                            string clientid = msgs[1];
                            if(!clientList.ContainsKey(clientid))
                            {
                                clientList.Add(clientid, clientSocket);
                            }
                        }
                        else
                        {
                            receiveCallBack(message);
                        }
                    }
                    catch(Exception exp)
                    {

                    }
                }
            }

            public void SendMessage(string clientid, string message)
            {
                try
                {
                    Socket clientSocket = clientList[clientid];
                    clientSocket.Send(Encoding.UTF8.GetBytes(message));
                }
                catch(Exception exp)
                {

                }
            }
        }

        public class MClientSocket
        {
            public Socket socket;
            public delegate void ReceiveCallBack(string message);
            public ReceiveCallBack receiveCallBack;
            private bool Running = true;
            public string clientid;

            public void Connect(string ip, int port)
            {
                try
                {
                    socket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                }
                catch(Exception exp)
                {

                }
                byte[] result = new byte[10240];
                int receiveLength = socket.Receive(result);
                Thread myThread = new Thread(ReceiveMessage);
                myThread.Start();
            }

            private void ReceiveMessage()
            {
                while(Running)
                {
                    try
                    {
                        byte[] result = new byte[10240];
                        int receiveNumber = socket.Receive(result);
                        string message = Encoding.UTF8.GetString(result);
                        if (message == "close")
                        {
                            socket.Close();
                            break;
                        }
                        else if (message == "connected")
                        {
                            SendMessage("register|" + clientid);
                        }
                        else
                        {
                            receiveCallBack(message);
                        }
                    }
                    catch (Exception exp)
                    {

                    }
                }
            }

            public void SendMessage(string message)
            {
                try
                {
                    socket.Send(Encoding.UTF8.GetBytes(message));
                }
                catch (Exception exp)
                {

                }
            }
        }
    }
}
