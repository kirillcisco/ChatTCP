using System.Net;
using System.Net.Sockets;
using System;
using ChatTCP;
using System.Security.Cryptography;

ChatTCP_Server ChatServer = new ChatTCP_Server();
ChatServer.StartServer();
await ChatServer.ConnectionHandler();

namespace ChatTCP
{ 
    class ChatTCP_Server
    {
        // Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        TcpListener tcpHandler = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);

        List<ClientEntity> connectedClients = new List<ClientEntity>();

        // IDBase[ID] = nickname
        List<string> IDBase = new List<string>();

        internal void StartServer()
        {
            //socket.Bind(_ipPoint);
            //socket.Listen(4096);
            Console.WriteLine("Server Started at: " + IPAddress.Parse("127.0.0.1"));
        }

        internal async Task ConnectionHandler()
        {

            try
            {
                tcpHandler.Start();

                while (true)
                {
                    TcpClient tcpClient = await tcpHandler.AcceptTcpClientAsync();
                    ClientEntity clientEntity = new ClientEntity(tcpClient, this, NewID());
                    connectedClients.Add(clientEntity);
                    Task.Run(clientEntity.ClientProcessor);
                }          
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        internal int NewID()
        {
            int _listLength = IDBase.Count;
            IDBase.Insert(0, "DefaultNickname");
            return _listLength;
        }

        internal string GetNicknameByID(int _id)
        {
            if (String.IsNullOrEmpty(IDBase[_id]))
            {
                return "UnknownNickname";
            }
            else
            {
                string _nickname = IDBase[_id];
                return _nickname;
            }
        }

        internal void ClientDiscconnect(int _id)
        {
            // получаем по id закрытое подключение
            ClientEntity? client = connectedClients.FirstOrDefault(_client => _client._ID == _id);

            // и удаляем его из списка подключений
            if (client != null) connectedClients.Remove(client);
            client?.CloseConnectionByServer();

            Console.WriteLine("User " + client._ID + " disconnected");
            BroadcastMessage($"{client._username} leaves the chat", client._ID);
        }

        internal async Task BroadcastMessage(string message, int _id)
        {
            foreach (var _client in connectedClients)
            {
                if (_client._ID != _id) // если id клиента не равно id отправителя
                {
                    await _client._streamWriter.WriteLineAsync(message);
                    await _client._streamWriter.FlushAsync();
                }
            }
        }
    }
}







