using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading;
using BattleShipBrain;
using BattleShipConsoleUI;
using MenuSystem;

namespace BattleShipsConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            var mainMenu = new Menu("<c=RED>Battle</c><c=BLUE>ships</c><c=GREEN> Game</c>", EMenuDepth.Main);
            mainMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("1", "<c=BLUE>Test</c> BattleShip <c=RED>Game</c>", runGame),
                new MenuItem("2", "<c=CYAN>Test</c>", null),
            });
            mainMenu.RunMenu();
        }

        public static string runGame()
        {
            var brain = new BsBrain(10, 5);
            Console.WriteLine("First player board:");
            BsConsoleUi.DrawBoard(brain.GetBoard(0));
            Console.WriteLine("Second player board:");
            BsConsoleUi.DrawBoard(brain.GetBoard(1));
            var done = false;
            while(done != true)
            {
                Console.Write("Start Game? (Y/N):");
                var answer = Console.ReadLine()?.ToUpper().Trim();
                switch (answer)
                {
                    case "Y":
                        done = true;
                        break;
                    case "N":
                        Console.WriteLine("Thank you for playing!");
                        Console.WriteLine();
                        Thread.Sleep(2000);
                        return "";
                }
            }

            while (true)
            {
                Console.Clear();
                BsConsoleUi.DrawBoard(brain.GetBoard(0));
                while (true)
                {
                    var FF1 = brain.Player1Move();
                    if (FF1 == "FF")
                    {
                        System.Environment.Exit(0);
                    }
                    if (FF1 == "MISS")
                    {
                        break;
                    }
                }
                BsConsoleUi.DrawBoard(brain.GetBoard(0));
                Thread.Sleep(5000);
                Console.Clear();
                BsConsoleUi.DrawBoard(brain.GetBoard(1));
                while (true)
                {
                    var FF2 = brain.Player2Move();
                    if (FF2 == "FF")
                    {
                        System.Environment.Exit(0);
                    }
                    if (FF2 == "MISS")
                    {
                        break;
                    }
                }
                BsConsoleUi.DrawBoard(brain.GetBoard(1));
                Thread.Sleep(5000);
                Console.Clear();
            }
        }
    }
}