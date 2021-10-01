using System;
using System.Collections.Generic;
using System.Linq;

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

        private void PrintMenu()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("~~~~~~~~ " + _menuTitle + " ~~~~~~~~");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=====================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (var item in _menuItems)
            {
                Console.WriteLine(item);
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