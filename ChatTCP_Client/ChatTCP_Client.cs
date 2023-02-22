using System.Net.Sockets;
using System.Net;
using System.IO;
using ChatTCP;

ChatTCP_Client chatClient = new ChatTCP_Client();

chatClient.StartClient();
chatClient.ReadKey();

namespace ChatTCP
{
    internal class ChatTCP_Client
    {
        LocalClient localClient = new LocalClient();
        internal static List<MenuOption> menuOptions = new List<MenuOption>
            {
                new MenuOption("Connect by IP to chat"),
                new MenuOption("Change BIO info"),
                new MenuOption("Change nickname"),
                new MenuOption("Close"),
            };

        internal int selectedMenu;

        ConsoleKeyInfo keyinfo;

        int port = 8888;

        internal void StartClient()
        {
            ConsoleRender.WriteMenu(menuOptions, menuOptions[0]);
        }

        internal async void ConnectByIP(string _ip)
        {
            TcpClient tcpHandler = new TcpClient();
            StreamReader? Reader = null;
            StreamWriter? Writer = null;

            try
            {
                tcpHandler.Connect(_ip, port);
                Reader = new StreamReader(tcpHandler.GetStream());
                Writer = new StreamWriter(tcpHandler.GetStream());
                if (Writer is null || Reader is null) return;

                Task.Run(() => ReceiveMessageAsync(Reader));

                SendMessageAsync(Writer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally 
            {
                Console.WriteLine("Connection closed");
                Reader.Close();
                Writer.Close();
                tcpHandler.Close(); 
            }
        }

        internal async void ReadKey() // REWORK this SHIT!!! TODO!
        {
            do
            {
                keyinfo = Console.ReadKey();
                if (keyinfo.Key == ConsoleKey.DownArrow)
                {
                    if (selectedMenu + 1 < menuOptions.Count)
                    {
                        selectedMenu++;
                        ConsoleRender.WriteMenu(menuOptions, menuOptions[selectedMenu]);
                    }
                }
                if (keyinfo.Key == ConsoleKey.UpArrow)
                {
                    if (selectedMenu - 1 >= 0)
                    {
                        selectedMenu--;
                        ConsoleRender.WriteMenu(menuOptions, menuOptions[selectedMenu]);
                    }
                }
                // Handle different action for the option
                if (keyinfo.Key == ConsoleKey.Enter)
                {
                    switch (selectedMenu)
                    {
                        case 0:
                            Console.Clear();

                            Console.WriteLine("Enter server IP: ");
                            string? _reqIP = Console.ReadLine();
                            ConnectByIP(_reqIP);
                            break;
                        case 1:
                            Console.Clear();

                            Console.WriteLine("Enter your BIO: ");
                            string? bio = Console.ReadLine();
                            localClient.SetUserbio(bio);
                            ConsoleRender.WriteMenu(menuOptions, menuOptions[0]);
                            break;
                        case 2:
                            Console.Clear();

                            Console.WriteLine("Enter your nickname: ");
                            string? nickname = Console.ReadLine();
                            localClient.SetUsername(nickname);
                            ConsoleRender.WriteMenu(menuOptions, menuOptions[0]);
                            break;
                        default:
                            Console.WriteLine("Select menu error");
                            break;
                    }
                    selectedMenu = 0;
                }
            } while (keyinfo.Key != ConsoleKey.X);
            Console.ReadKey();
        }

        async Task SendMessageAsync(StreamWriter writer)
        {
            await writer.WriteLineAsync(localClient.Username());
            await writer.FlushAsync();
            await writer.WriteLineAsync(localClient.Userbio());
            await writer.FlushAsync();
            Console.WriteLine("Enter your message and press enter");

            while (true)
            {
                string? message = Console.ReadLine();
                await writer.WriteLineAsync(message);
                await writer.FlushAsync();

                // clear output line with msg to tag you msg by server (msg -> You: msg)
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                ClearOutputLine();
            }
        }

        async Task ReceiveMessageAsync(StreamReader reader)
        {

            while (true)
            {
                try
                {
                    // check the null or empty incoming msg
                    string? message = await reader.ReadLineAsync();
                    
                    if (string.IsNullOrEmpty(message)) continue;
                    Print(message);
                }
                catch
                {
                    break;
                }
            }
        }

        void Print(string _message)
        {
            if (!OperatingSystem.IsWindows())    
            {
                Console.WriteLine(_message);
            }
            else // Win walkaround printing
            {
                var position = Console.GetCursorPosition(); // получаем текущую позицию курсора
                int left = position.Left;   // смещение в символах относительно левого края
                int top = position.Top;     // смещение в строках относительно верха


                // копируем ранее введенные символы в строке на следующую строку
                Console.MoveBufferArea(0, top, left, 1, 0, top + 1);
                // устанавливаем курсор в начало текущей строки
                Console.SetCursorPosition(0, top);
                // в текущей строке выводит полученное сообщение
                Console.WriteLine(_message);
                // переносим курсор на следующую строку
                // и пользователь продолжает ввод уже на следующей строке
                Console.SetCursorPosition(left, top + 1);
            }
        }

        public static void ClearOutputLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }

    internal class MenuOption
    {
        public string Option { get; }

        public MenuOption(string option)
        {
            Option = option;
        }
    }

    internal class LocalClient
    {
        private string username;
        private string userbio;

        public string Username()
        {
            return username;
        }
        public string Userbio()
        {
            return userbio;
        }

        internal void SetUsername(string _username)
        {
            username = _username;
        }

        internal void SetUserbio(string _userbio)
        { 
            userbio = _userbio; 
        }
    }
}