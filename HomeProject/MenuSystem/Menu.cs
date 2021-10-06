using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;

namespace MenuSystem
{
    public class Menu
    {
        private readonly string _menuTitle;
        private readonly List<MenuItem> _menuItems = new List<MenuItem>();
        private HashSet<string> _shortCuts = new HashSet<string>();
        private readonly HashSet<string> _specialShortCuts = new HashSet<string>();
        private readonly MenuItem _return = new MenuItem("R", "Return", null);
        private readonly MenuItem _exit = new MenuItem("X", "Exit", null);
        private readonly EMenuDepth _menuDepth;

        public Menu(string title, EMenuDepth depth)
        {
            _menuTitle = title;
            _menuDepth = depth;
            switch (depth)
            {
                case EMenuDepth.Main:
                    _specialShortCuts.Add(_exit.ShortCut.ToUpper());
                    break;
                case EMenuDepth.SubMenu:
                    _specialShortCuts.Add(_return.ShortCut.ToUpper());
                    _specialShortCuts.Add(_exit.ShortCut.ToUpper());
                    break;
            }
        }

        private void AddMenuItem(MenuItem item, int position = -1)
        {
            if (!_specialShortCuts.Add(item.ShortCut.ToUpper()))
            {
                throw new ApplicationException($"{item.ShortCut.ToUpper()} - already in use by special shortcuts");
            }
            else if (!_shortCuts.Add(item.ShortCut.ToUpper()))
            {
                throw new ApplicationException($"{item.ShortCut.ToUpper()} - already in use by shortcuts");
            }
            else if (position == -1)
            {
                _menuItems.Add(item);
            }
            else
            {
                _menuItems.Insert(position, item);
            }
        }

        public void AddMenuItems(List<MenuItem> items)
        {
            foreach (var menuItem in items)
            {
                AddMenuItem(menuItem);
            }
        }

        private static void GetTextAndColor(string strSource)
        {
            const string strStart = "<c=";
            const string strEnd = "</c>";
            var sentence = strSource;
            int end;
            if (sentence.Contains(strStart) && sentence.Contains(strEnd))
            {
                end = sentence.IndexOf(strStart, 0, StringComparison.Ordinal);
                Console.Write(sentence.Substring(0, end ));
            }
            while(sentence.Contains(strStart) && sentence.Contains(strEnd))
            {
                var start = sentence.IndexOf(strStart, 0, StringComparison.Ordinal) + strStart.Length;
                end = sentence.IndexOf(">", start, StringComparison.Ordinal);
                var color = sentence.Substring(start, end - start);
                Console.ForegroundColor = (ConsoleColor) Enum.Parse(typeof(ConsoleColor), color, true);
                start = sentence.IndexOf(strStart, 0, StringComparison.Ordinal) + strStart.Length + color.Length + 1;
                end = sentence.IndexOf(strEnd, start, StringComparison.Ordinal);
                Console.Write(sentence.Substring(start, end - start));
                start = end+4;
                if (sentence[start..].Contains(strStart))
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    end = sentence.IndexOf(strStart, start, StringComparison.Ordinal);
                    Console.Write(sentence.Substring(start, end - start));
                }
                end = sentence.IndexOf(strEnd, sentence.IndexOf(strStart, 0, StringComparison.Ordinal) + strStart.Length + color.Length + 1, StringComparison.Ordinal);
                sentence = sentence[(end+4)..];
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(sentence);
        }
        private void PrintMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("~~~~~~~~ ");
            GetTextAndColor(_menuTitle);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(" ~~~~~~~~");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=====================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (var item in _menuItems)
            {
                GetTextAndColor(item.ToString());
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=====================================");
            Console.ForegroundColor = ConsoleColor.White;
            switch (_menuDepth)
            {
                case EMenuDepth.Main:
                    Console.WriteLine(_exit);
                    break;
                case EMenuDepth.SubMenu:
                    Console.WriteLine(_return);
                    Console.WriteLine(_exit);
                    break;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=====================================");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Option: ");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public double? RunMenu()
        {
            var input = "";
            do
            {
                PrintMenu();
                input = Console.ReadLine()?.Trim().ToUpper();
                if (!_shortCuts.Contains(input)) continue;
                var wanted = _menuItems.FirstOrDefault(item => item.ShortCut.ToUpper() == input);
                input = wanted?.RunMethod == null ? input : wanted.RunMethod();
            } while (!_specialShortCuts.Contains(input));
            if (input == _exit.ShortCut.ToUpper()) System.Environment.Exit(0);
            return null;
        }
    }
}