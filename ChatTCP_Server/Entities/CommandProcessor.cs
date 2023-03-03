using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChatTCP
{
    internal enum CommandID
    {
        ExitByClient = 0,
        PersonalMessage = 1,
        GetUserbio = 2,
        GetWhoamiInfo = 3,
        ChatRoll = 4,
        ChatMeRoleplay = 5
    }

    internal class CommandProcessor
    {
        ClientEntity _chatClient;
        internal CommandProcessor(string[] _msgArgs, string[] _msgArgsWithoutCommand, string _mainMsg, int _userID, ChatTCP_Server _chatServer)
        {
            List<ClientEntity> _connectedClients = _chatServer.connectedClients;

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
                    if (_msgArgs.Length == 2)
                    {
                        _chatServer.GetUserbioByNickname(_msgArgsWithoutCommand[0], _userID);
                    }
                    else
                    {
                        _chatServer.ServerPersonalMessage("Command error!", _userID);
                    }
                    break;
                case "/whoami":
                    break;
                case "/roll":
                    break;
                case "/me":
                    break;
                default:
                    _chatServer.ServerPersonalMessage("Command not found!", _userID);
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
    }
}
