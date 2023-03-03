using System.Net;
using System.Net.Sockets;
using System;
using ChatTCP;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;

ChatTCP_Server ChatServer = new ChatTCP_Server();
ChatServer.StartServer();

namespace ChatTCP
{ 
    public class ChatTCP_Server
    {
        IPEndPoint ipEndPoint;
        TcpListener tcpHandler;
        internal List<ClientEntity> connectedClients = new List<ClientEntity>();

        // IDBase[ID] = nickname, Unknown_RANDOMINDEX TODO
        List<string> IDBase = new List<string>();

        internal void StartServer()
        {
            try
            {
                ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
                Console.WriteLine("Server starting at " + ipEndPoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fault to allocate ipEndPoint: " + ex);
                return;
            }

            try
            {
                tcpHandler = new TcpListener(ipEndPoint);
                tcpHandler.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fault to start tcpHandler: " + ex);
                return;
            }

            var connectionHandler = Task.Run(() => ConnectionHandler());
            connectionHandler.Wait(); 
        }

        internal void StopServer()
        {
            tcpHandler.Stop();
            
            foreach (var _client in connectedClients)
            {
                _client?.CloseConnectionByServer();
                connectedClients.Remove(_client);
            }

            Console.WriteLine("Server stopped");
        }

        internal async Task ConnectionHandler()
        {
            Console.WriteLine("Server and connection hadler is started");
            try
            {
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

        internal bool IsUsernameExist(string _username, int _userID) 
        {
            foreach (var _client in connectedClients)
            {
                if (_client._username == _username && !(_client._ID == _userID))
                {
                    return true;
                }
            }
            return false;
        }

        internal bool ClientDiscconnect(int _id)
        {
            // получаем по id закрытое подключение
            ClientEntity? client = connectedClients.FirstOrDefault(_client => _client._ID == _id);
            
            if (client == null) 
            {
                Console.WriteLine("User not found");
                return false;
            }

            Console.WriteLine("User " + client._ID + " disconnected");
            BroadcastMessage($"{client._username} leaves the chat", client._ID);

            // и удаляем его из списка подключений
            if (client != null) connectedClients.Remove(client);
            client?.CloseConnectionByServer();
            return true;
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

        internal async Task CommandProcessor(byte _commandID, string _message, string _username, int _userID)
        {
            



            /* switch (_commandID)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 0:
                    break;
            } */
        }
        internal async Task PersonalMessage(string _message, string _username, int _userID)
        {
            //var _destionation_client = FindClientObjectByUsername(_username);
            //var _source_client = FindClientObjectByID(_userID);

            var _destionation_client = connectedClients.Find(x => x._username == _username);
            var _source_client = connectedClients.Find(x => x._ID == _userID);

            try
            {
                if(_destionation_client != null && _source_client._ID == _userID)
                {
                    await _destionation_client._streamWriter.WriteLineAsync("(From " + _source_client._username + "): " + _message);
                    await _destionation_client._streamWriter.FlushAsync();
                    await _source_client._streamWriter.WriteLineAsync("(You) > (" + _destionation_client._username + "): " + _message);
                    await _source_client._streamWriter.FlushAsync();
                }
                else
                {
                    await _source_client._streamWriter.WriteLineAsync("(Server): User not found :(");
                    await _source_client._streamWriter.FlushAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        internal async Task GetUserbioByNickname(string _username, int _userID)
        {
            //var _destionation_client = FindClientObjectByUsername(_username);
            //var _source_client = FindClientObjectByID(_userID);
            var _destionation_client = connectedClients.Find(x => x._username == _username);
            var _source_client = connectedClients.Find(x => x._ID == _userID);

            try
            {
                if (_destionation_client != null && _source_client._ID == _userID)
                {
                    Console.WriteLine("Sended bio: " + _destionation_client._userbio);
                    await _source_client._streamWriter.WriteLineAsync("(" + _destionation_client._username + " BIO): " + _destionation_client._userbio);
                    await _source_client._streamWriter.FlushAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        internal async Task ServerPersonalMessage(string _message, int _userID)
        {
            var _client = connectedClients.Find(x => x._ID == _userID);

            try
            {
                await _client._streamWriter.WriteLineAsync("(Server): " + _message);
                await _client._streamWriter.FlushAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        /* internal ClientEntity? FindClientObjectByUsername(string _username)
        {
            if (string.IsNullOrEmpty(_username))
            {
                try
                {
                    var _client = connectedClients.Find(x => x._username == _username);
                    return _client;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }
            return null;
        }

        internal ClientEntity FindClientObjectByID(int _id)
        {
            try
            {
                var _client = connectedClients.Find(x => x._ID == _id);
                return _client;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        */
    }
}