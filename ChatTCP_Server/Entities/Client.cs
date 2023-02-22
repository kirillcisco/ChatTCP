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

        internal ChatCommand chatCommand;

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

                // first get username & userbio
                try
                {
                    _username = await _streamReader.ReadLineAsync();
                    _userbio = await _streamReader.ReadLineAsync();
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
                            // remake this shit
                            // todo
                            /* if (CheckCommand(_message))
                            {
                            } */
                            if (Regex.IsMatch(_message, @"(/+)"))
                            {
                                // debug rightnow
                                
                                Console.WriteLine("command find: " + _message);
                                string[] _opers = Regex.Split(_message, @"(?<!\s)\s|\s(?!\s)");
                                string[] _opersWithoutCommand = _opers.Skip(1).ToArray();
                                string _msgWithoutCommand = string.Join(" ", _opersWithoutCommand);

                                switch (_opers[0])
                                {
                                    case "/exit":
                                        chatCommand.DisconnectByClient(_ID);
                                        break;
                                    case "/pm":
                                        break;
                                    case "/bio":
                                        break;
                                    case "/whoami":
                                        break;
                                    case "/roll":
                                        break;
                                    case "/me":
                                        break;
                                    default:
                                        Console.WriteLine("Invalid command");
                                        break;
                                }

                                //await CommandProcessor(_server,_message);
                                return;
                            }
                        }
                        else return;

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
        
        // todo ?
        /* internal static bool CheckCommand(string _msg)
        {
            string[] msg = _msg.Split()
                    .Where(x => x.StartsWith("/"))
                    .Distinct()
                    .ToArray();
            return msg.Length > 0;
        } */

        internal static async Task CommandProcessor(ChatTCP_Server _srv, string _msg)
        {
            

            /* foreach (var operand in operands)
            {
                Console.WriteLine(operand);
            } */

            
        }
    }
}
