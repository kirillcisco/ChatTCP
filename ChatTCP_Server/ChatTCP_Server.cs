using System.Net;
using System.Net.Sockets;
using System;
using ChatTCP;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;

ChatTCP_Server ChatServer = new ChatTCP_Server();
ChatServer.StartServer();
await ChatServer.ConnectionHandler();

namespace ChatTCP
{ 
    class ChatTCP_Server
    {
        TcpListener tcpHandler = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);

        List<ClientEntity> connectedClients = new List<ClientEntity>();

        // IDBase[ID] = nickname
        List<string> IDBase = new List<string>();

        internal void StartServer()
        {
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

        internal bool CheckForConnection(int _id)
        {
            return true;
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

        internal async Task BroadcastMessage(string _message, int _id)
        {
            foreach (var _client in connectedClients)
            {
                if (_client._ID != _id) // если id клиента не равно id отправителя
                {
                    await _client._streamWriter.WriteLineAsync(_message);
                    await _client._streamWriter.FlushAsync();
                }
                else
                {
                    await _client._streamWriter.WriteLineAsync("(You) " + _message);
                    await _client._streamWriter.FlushAsync();
                }
            }
        }

        internal async Task PersonalMessage(string _message, string _username, int _id)
        {
            ClientEntity _destionation_client = connectedClients.Find(x => x._username == _username);
            ClientEntity _source_client = connectedClients.Find(x => x._ID == _id);

            try
            {
                await _destionation_client._streamWriter.WriteAsync("(From " + _source_client._username + "):" + _message);
                await _destionation_client._streamWriter.FlushAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}







