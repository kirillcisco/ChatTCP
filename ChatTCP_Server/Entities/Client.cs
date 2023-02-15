using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ChatTCP;

namespace ChatTCP
{
    internal class ClientEntity
    {
        protected internal int _ID { get; }

        protected internal string _username { get; set; }
        protected internal string _userbio { get; set; }
        protected internal StreamWriter _streamWriter { get; }
        protected internal StreamReader _streamReader { get; }


        TcpClient _client;
        ChatTCP_Server _server; // объект сервера

        internal ClientEntity(TcpClient tcpClient, ChatTCP_Server _connectedTCPServer, int _userChatID)
        {
            _ID = _userChatID;

            _client = tcpClient;
            _server = _connectedTCPServer;

            var stream = _client.GetStream();
            _streamReader = new StreamReader(stream);
            _streamWriter = new StreamWriter(stream);
        }

        public async Task ClientProcessor()
        {
            try
            {
                Console.WriteLine("User: " + _ID + " connected");
                string _message;

                _username = await _streamReader.ReadLineAsync();
                _userbio = await _streamReader.ReadLineAsync();

                var _timeStamp = new DateTimeOffset(DateTime.UtcNow);

                Console.WriteLine($"User: ID: {_ID}, Nickname: {_username}, bio: {_userbio} [CONNECTED] [{_timeStamp}]");

                if (string.IsNullOrEmpty(_username)) 
                {
                    _username = "Unknown";
                }

                while (true)
                {

                    try
                    {
                        _message = await _streamReader.ReadLineAsync();
                        
                        // check for empty messages and commands
                        if (!(_message == null))
                        {
                            if (Regex.IsMatch(_message, @"(/+)"))
                            {
                                string[] operands = Regex.Split(_message, @"/ | null");
                                foreach (var operand in operands)
                                {
                                    Console.WriteLine(operand);
                                }
                            }
                        }
                        else continue;

                        _message = $"{_username}: {_message}";
                        Console.WriteLine(_message);
                        await _server.BroadcastMessage(_message, _ID);
                    }
                    catch
                    {
                        _message = $"{_username} -  loss of signal";
                        Console.WriteLine(_message);
                        break;
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
            finally
            {
                _server.ClientDiscconnect(_ID);
            }
        }

        internal void CloseConnectionByServer()
        {
            _streamReader.Close();
            _streamWriter.Close();
            _client.Close();
        }
    }

    // Commands with /....

    interface IChatCommand
    {
        void Execute();
    }

    internal class SimpleCommand : IChatCommand
    {
        public void Execute()
        {

        }
    }
}
