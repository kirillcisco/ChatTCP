using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChatTCP
{
    internal class ChatCommand
    {
        ChatTCP_Server _chatServer;
        ClientEntity _chatClient;

        internal ChatCommand(string[] _msgArgs, string[] _msgArgsWithoutCommand, string _mainMsg, int _userID, ChatTCP_Server ChatServer)
        {
            _chatServer = ChatServer;

            switch (_msgArgs[0])
            {
                case "/exit":
                    _chatServer.ClientDiscconnect(_userID);
                    break;
                case "/pm":
                    if (_msgArgs.Length > 2)
                    {
                        //delete username from message string just walkaround
                        int i = _mainMsg.IndexOf(" ") + 1;
                        string str = _mainMsg.Substring(i);

                        _chatServer.PersonalMessage(str, _msgArgsWithoutCommand[0], _userID);
                    }
                    else
                    {
                        _chatServer.ServerPersonalMessage("Command isnt complete!", _userID);
                    }
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

            // todo ?
            /* internal static bool CheckCommand(string _msg)
            {
                string[] msg = _msg.Split()
                        .Where(x => x.StartsWith("/"))
                        .Distinct()
                        .ToArray();
                return msg.Length > 0;
            } */
        }

        private void PersonalMessage()
        {

        }

        private void Userbio()
        {

        }

        private void Whoami(int _id)
        {

        }

        private void RollingNumber()
        {

        }

        private void MeCommand()
        {

        }


    }

}
