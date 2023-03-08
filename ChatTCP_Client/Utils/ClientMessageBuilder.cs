using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatTCP
{
    // [1B = type of message][4B = length][XB = message] - where X == BitConvert.ToInt32(length);
    /* internal static class ClientMessageBuilder
    {
        private const int DefaultCapacity = 1024;
        private char[] buffer = new char[1024];

        public static Task<char[]> ClientMessageBuild(sbyte msgType, char[] _msg)
        {
            return Buffer; 
        }
    }*/
}
