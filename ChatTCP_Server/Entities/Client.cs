using ChatTCP;
using System.Collections.Generic;
using System;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

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
        ChatTCP_Server _server;

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

                // first get username & userbio
                try
                {
                    _username = await _streamReader.ReadLineAsync();

                    if (_server.IsUsernameExist(_username, _ID))
                    {

                        _streamWriter.WriteLineAsync("(Server): Sorry, this username is already in use. Please change before connection");
                        Console.WriteLine("Catched 2 nicknames");
                        _server.ClientDiscconnect(_ID);
                    }
                    else if (string.IsNullOrEmpty(_username))
                    {
                        _username = "Unknown_" + _ID;
                    }

                    _userbio = await _streamReader.ReadLineAsync();

                    if (string.IsNullOrEmpty(_userbio))
                    {
                        _userbio = "We don't know anything about " + _username;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.GetType().Name);
                    Console.WriteLine("");
                    Console.WriteLine(ex.Message);
                    _server.ClientDiscconnect(_ID); // idk?
                    throw;
                }

                // log connecting
                var _timeStamp = new DateTimeOffset(DateTime.UtcNow);
                Console.WriteLine($"User: ID: {_ID}, Nickname: {_username}, bio: {_userbio} [CONNECTED] [{_timeStamp}]");

                while (true)
                {
                    try
                    {
                        _message = await _streamReader.ReadLineAsync();

                        // check for empty messages and commands
                        if (!(_message == null))
                        {
                            // remake this shit
                            // todo
                            /* if (CheckCommand(_message))
                            {
                            } */
                            if (Regex.IsMatch(_message, @"(/+)")) // check chat string for starting with /....
                            {
                                string[] _args = Regex.Split(_message, @"(?<!\s)\s|\s(?!\s)");
                                string[] _argsWithoutCommand = _args.Skip(1).ToArray();
                                string _msgWithoutCommand = string.Join(" ", _argsWithoutCommand);

                                CommandProcessor commandProcessor = new CommandProcessor(_args, _argsWithoutCommand, _msgWithoutCommand, _ID, _server);
                                continue;
                            }

                            _message = $"{_username}: {_message}";
                            Console.WriteLine(_message);
                            await _server.BroadcastMessage(_message, _ID);
                        }
                        else continue;
                    }
                    catch
                    {
                        _message = $"{_username} -  downlink connection closed";
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
                Console.WriteLine("Client processor is closed, user ID: " + _ID);
                _server.ClientDiscconnect(_ID);
            }

        }

        internal void CloseConnectionByServer()
        {
            _streamReader.Close();
            _streamWriter.Close();
            _client.Close();
        }

        internal void DisplayLocalError(string _errorMsg)
        {
            Console.WriteLine("Error: " + _errorMsg);
        }
    }
}
