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

                    DisconnectByClient(_userID);
                    break;
                case "/pm":
                    
                    PersonalMessage();
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

        private void DisconnectByClient(int _id)
        {
            // _chatServer.ServerPersonalMessage("You are disconnected from the server", _id);   
            _chatServer.ClientDiscconnect(_id);
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
