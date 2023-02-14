using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ChatTCP
{
    internal class ConsoleRender // Todo: some effects and more??
    {    
        internal static int selectedOption;

        internal static void WriteMenu(List<MenuOption> menuOptions, MenuOption selectedOption)
        {
            Console.Clear();

            foreach (MenuOption option in menuOptions)
            {
                if (option == selectedOption)
                {
                    Console.Write("> ");
                }
                else
                {
                    Console.Write(" ");
                }

                Console.WriteLine(option.Option);
            }
        }
    }
}

