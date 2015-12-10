using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace COM.MeshStudio.Lib.BasicComponent
{
    public class SocketTool
    {
        public static MServerSocket FactoryGenerateServerTCPSocket()
        {
            MServerSocket result = new MServerSocket();
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            result.server = socket;
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
            public Socket server;
            public delegate void ReceiveCallBack(string message);
            public ReceiveCallBack receiveCallBack;
            private Dictionary<string, Socket> clientList = new Dictionary<string, Socket>();
            private List<Socket> anonymousClientList = new List<Socket>();

            public void Listen(string ip, int port, int queueLength)
            {
                server.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
                clientList = new Dictionary<string, Socket>();
                server.Listen(queueLength);
                server.BeginAccept(new AsyncCallback(OnConnectRequest), server);
            }

            private void OnConnectRequest(IAsyncResult ar)
            {
                Socket server = (Socket)ar.AsyncState;
                Socket client = server.EndAccept(ar);
                client.Send(Encoding.UTF8.GetBytes("connected"));
                StateObject state = new StateObject();
                state.socket = client;
                client.BeginReceive(state.buffers, SocketFlags.None, new AsyncCallback(ReceiveMessage), state);
                anonymousClientList.Add(client);
                server.BeginAccept(new AsyncCallback(OnConnectRequest), server);
            }

            private void OnServerClose(IAsyncResult ar)
            {

            }

            public void Stop()
            {
                foreach(Socket s in anonymousClientList)
                {
                    s.Send(Encoding.UTF8.GetBytes("close"));
                }
                foreach(KeyValuePair<string, Socket> kv in clientList)
                {
                    kv.Value.Send(Encoding.UTF8.GetBytes("close"));
                }
                anonymousClientList.Clear();
                clientList.Clear();
            }

            private void ReceiveMessage(IAsyncResult ar)
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.socket;
                int receiveNumber = client.EndReceive(ar);
                string message = Encoding.UTF8.GetString(state.buffers[0].Array, 0, receiveNumber);
                if (message == "close")
                {
                    receiveCallBack("Client Closed Socket");
                    if (clientList.ContainsValue(client))
                    {
                        int index = clientList.Values.ToList<Socket>().IndexOf(client);
                        string key = clientList.Keys.ToList<string>()[index];
                        clientList.Remove(key);
                    }
                    client.Send(Encoding.UTF8.GetBytes("close"));
                    client.Close();
                }
                else if (message == "clientClose")
                {
                    if (clientList.ContainsValue(client))
                    {
                        int index = clientList.Values.ToList<Socket>().IndexOf(client);
                        string key = clientList.Keys.ToList<string>()[index];
                        clientList.Remove(key);
                    }
                    client.Close();
                }
                else if (message.StartsWith("register"))
                {
                    string[] msgs = message.Split(new char[] { '|' });
                    string clientid = msgs[1];
                    if (!clientList.ContainsKey(clientid))
                    {
                        clientList.Add(clientid, client);
                    }
                    anonymousClientList.Remove(client);
                    client.BeginReceive(state.buffers, SocketFlags.None, new AsyncCallback(ReceiveMessage), state);
                }
                else
                {
                    receiveCallBack(message);
                    client.BeginReceive(state.buffers, SocketFlags.None, new AsyncCallback(ReceiveMessage), state);
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
            private IPEndPoint serverEP;
            public Socket socket;
            public delegate void ReceiveCallBack(string message);
            public ReceiveCallBack receiveCallBack;
            public string clientid;

            public void Connect(string ip, int port)
            {
                try
                {
                    serverEP = new IPEndPoint(IPAddress.Parse(ip), port);
                    socket.BeginConnect(serverEP, new AsyncCallback(OnConnectResponse), socket);
                }
                catch(Exception exp)
                {

                }
            }

            private void OnConnectResponse(IAsyncResult ar)
            {
                Socket socket = (Socket)ar.AsyncState;
                if (socket.Connected)
                {
                    socket.EndConnect(ar);
                    StateObject state = new StateObject();
                    state.socket = socket;
                    socket.BeginReceive(state.buffers, SocketFlags.None, new AsyncCallback(ReceiveMessage), state);
                }
                else
                {
                    socket.BeginConnect(this.serverEP, new AsyncCallback(OnConnectResponse), socket);
                }
            }

            private void ReceiveMessage(IAsyncResult ar)
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket socket = state.socket;
                int receiveNumber = socket.EndReceive(ar);
                string message = Encoding.UTF8.GetString(state.buffers[0].Array, 0, receiveNumber);
                if (message == "close")
                {
                    receiveCallBack("Server Closed Socket");
                    SendMessage("close");
                    socket.Close();
                }
                else if(message == "connected")
                {
                    SendMessage("register|" + clientid);
                    socket.BeginReceive(state.buffers, SocketFlags.None, new AsyncCallback(ReceiveMessage), state);
                }
                else
                {
                    receiveCallBack(message);
                    socket.BeginReceive(state.buffers, SocketFlags.None, new AsyncCallback(ReceiveMessage), state);
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

            public void Stop()
            {
                SendMessage("clientClose");
                socket.Close();
            }
        }

        public class StateObject
        {
            public List<ArraySegment<byte>> buffers = new List<ArraySegment<byte>>();
            public Socket socket = null;

            public StateObject()
            {
                buffers = new List<ArraySegment<byte>>();
                byte[] buffer = new byte[10240];
                buffers.Add(new ArraySegment<byte>(buffer));
            }
        }
    }
}
