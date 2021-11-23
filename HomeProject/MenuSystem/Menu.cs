using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Enumeration;
using System.Linq;

namespace MenuSystem
{
    public class Menu
    {
        private readonly string _menuTitle;
        private readonly List<MenuItem> _menuItems = new List<MenuItem>();
        private readonly HashSet<string> _shortCuts = new HashSet<string>();
        private readonly HashSet<string> _specialShortCuts = new HashSet<string>();
        private readonly MenuItem _return = new MenuItem("R", "Return", null!);
        private readonly MenuItem _exit = new MenuItem("X", "Exit", null!);
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

        public void AddMenuItem(MenuItem item, int position = -1)
        {
            if (!_specialShortCuts.Add(item.ShortCut.ToUpper()))
            {
                throw new ApplicationException($"{item.ShortCut.ToUpper()} - already in use by special shortcuts");
            }

            if (!_shortCuts.Add(item.ShortCut.ToUpper()))
            {
                throw new ApplicationException($"{item.ShortCut.ToUpper()} - already in use by shortcuts");
            }
            if (position == -1)
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
        
        private static void GetTextAndColor(string strSource, bool chosen)
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
                if (chosen)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = (ConsoleColor) Enum.Parse(typeof(ConsoleColor), color, true);
                }
                start = sentence.IndexOf(strStart, 0, StringComparison.Ordinal) + strStart.Length + color.Length + 1;
                end = sentence.IndexOf(strEnd, start, StringComparison.Ordinal);
                Console.Write(sentence.Substring(start, end - start));
                start = end+4;
                if (sentence[start..].Contains(strStart))
                {
                    Console.ForegroundColor = chosen ? ConsoleColor.Red : ConsoleColor.White;
                    end = sentence.IndexOf(strStart, start, StringComparison.Ordinal);
                    Console.Write(sentence.Substring(start, end - start));
                }
                end = sentence.IndexOf(strEnd, sentence.IndexOf(strStart, 0, StringComparison.Ordinal) + strStart.Length + color.Length + 1, StringComparison.Ordinal);
                sentence = sentence[(end+4)..];
            }
            Console.ForegroundColor = chosen ? ConsoleColor.Red : ConsoleColor.Yellow;
            Console.Write(sentence);
        }
        private void PrintMenu(MenuItem? chosenOne)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("~~~~~~~~ ");
            GetTextAndColor(_menuTitle, false);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(" ~~~~~~~~");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=====================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (var item in _menuItems)
            {
                if (item == chosenOne)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    GetTextAndColor(item.ToString(), true);
                    Console.WriteLine();
                    Console.ResetColor();
                }
                else
                {
                    GetTextAndColor(item.ToString(), false);
                    Console.WriteLine();
                }
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
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=====================================");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Option: ");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public string RunMenu()
        {
            /*ConsoleKey key;
            var i = 0;
            Console.CursorVisible = false;
            var chosenOne = _menuItems[i];
            do
            {
                Console.Clear();
                PrintMenu(chosenOne);
                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                        if (i != 0)
                        {
                            i--;
                            chosenOne = _menuItems[i];
                        }

                        break;
                    case ConsoleKey.RightArrow:
                        if (_menuItems.Count-1 > i)
                        {
                            i++;
                            chosenOne = _menuItems[i];
                        }

                        break;
                    case ConsoleKey.UpArrow:
                        if (i != 0)
                        {
                            i--;
                            chosenOne = _menuItems[i];
                        }

                        break;
                    case ConsoleKey.DownArrow:
                        if (_menuItems.Count-1 > i)
                        {
                            i++;
                            chosenOne = _menuItems[i];
                        }

                        break;
                    case ConsoleKey.X:
                        Environment.Exit(0);
                        break;
                }
            } while (key != ConsoleKey.Enter);
            Console.CursorVisible = true;
            var input = _menuItems[i].RunMethod(filename, savefilename);
            return input;
            */
            
            bool runDone;
            var input = "";
            do
            {
                PrintMenu(null);
                input = Console.ReadLine()?.Trim().ToUpper();
                Debug.Assert(input != null, nameof(input) + " != null");
                var isInputValid = _shortCuts.Contains(input);
                if (isInputValid)
                {
                    var item = _menuItems.FirstOrDefault(t => t.ShortCut.ToUpper() == input);
                    input = item?.RunMethod == null ? input : item.RunMethod(input);
                }
                runDone = _specialShortCuts.Contains(input);
            } while (!runDone);
            if (input == _return.ShortCut.ToUpper()) return "";
            if (input == _exit.ShortCut.ToUpper()) Environment.Exit(0);
            return input ?? "";
            
        }
    }
}