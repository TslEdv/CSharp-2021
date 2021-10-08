using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Xml;
using BattleShipBrain;
using BattleShipConsoleUI;
using MenuSystem;

namespace BattleShipsConsoleApp
{
    class Program
    {
        private static string? _basePath;
        static void Main(string[] args)
        {
            _basePath = args.Length == 1 ? args[0] : System.IO.Directory.GetCurrentDirectory();
            var fileNameStandardConfig = _basePath + System.IO.Path.DirectorySeparatorChar + "Configs" + System.IO.Path.DirectorySeparatorChar + "standard.json";
            var fileNameSavedGame = _basePath + System.IO.Path.DirectorySeparatorChar + "SavedGames" + System.IO.Path.DirectorySeparatorChar + "game.json";
            Console.Clear();
            var mainMenu = new Menu("<c=RED>Battle</c><c=BLUE>ships</c><c=GREEN> Game</c>", EMenuDepth.Main);
            mainMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("1", "<c=BLUE>Test</c> BattleShip <c=RED>Game</c>", runGame),
                new MenuItem("2", "<c=RED>See Game Settings</c>", SeeSettings),
            });
            mainMenu.RunMenu(fileNameStandardConfig,fileNameSavedGame);
        }

        private static string runGame(string filename, string savefilename)
        {
            BsBrain? brain = new BsBrain(new GameConfig());
            if (System.IO.File.Exists(savefilename) && System.IO.File.Exists(filename))
            {
                Console.WriteLine("Detected a saved game! Would you like to continue it?(y/n):");
                switch (Console.ReadLine()?.Trim().ToUpper())
                {
                    case "Y":
                        var brainconf = System.IO.File.ReadAllText(savefilename);
                        brain!.RestoreBrainFromJson(brainconf);
                        break;
                    case "N":
                        Console.WriteLine("Loading config...");
                        var confText = System.IO.File.ReadAllText(filename);
                        var conf = JsonSerializer.Deserialize<GameConfig>(confText);
                        brain = new BsBrain(conf);
                        break;
                }
            }
            else if (System.IO.File.Exists(filename))
            {
                Console.WriteLine("Loading config...");
                var confText = System.IO.File.ReadAllText(filename);
                var conf = JsonSerializer.Deserialize<GameConfig>(confText);
                brain = new BsBrain(conf);
            }
            else
            {
                Console.WriteLine("No Config found, loading default...");
                brain = new BsBrain(new GameConfig());

            }
            Console.WriteLine("First player board:");
            BsConsoleUi.DrawBoard(brain!.GetBoard(0));
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
                BsConsoleUi.DrawBoard(brain.GetBoard(brain.Move()));
                while (true)
                {
                    var FF1 = BsConsoleUi.Move(brain);
                    if (FF1 == "FF")
                    {
                        if (System.IO.File.Exists(savefilename))
                        {
                            System.IO.File.Delete(savefilename);
                        } 
                        Thread.Sleep(5000);
                        return "";
                    }
                    if (FF1 == "MISS")
                    {
                        Console.WriteLine("You Missed!");
                        break;
                    }
                    if (FF1 == "SAVE")
                    {
                        var jsonstr = brain.GetBrainJson(brain.Move());
                        Console.WriteLine(jsonstr);
                        System.IO.File.WriteAllText(savefilename, jsonstr);
                        Thread.Sleep(5000);
                        return "";
                    }
                    Console.WriteLine("You Hit!");
                }
                BsConsoleUi.DrawBoard(brain.GetBoard(0));
                Thread.Sleep(5000);
            }
        }

        private static string SeeSettings(string filename, string savefilename)
        {
            GameConfig? config;
            if (System.IO.File.Exists(filename))
            {
                var confText = System.IO.File.ReadAllText(filename);
                config = JsonSerializer.Deserialize<GameConfig>(confText);
                
            }
            else
            {
                config = new GameConfig();
            }
            var settingsMenu = new Menu("<c=RED>Set</c><c=BLUE>ti</c><c=GREEN>ngs</c>", EMenuDepth.SubMenu);
            settingsMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("1", $"Reset current config:" + System.Environment.NewLine + $"{config}", ResetSettings),
                new MenuItem("2", "<c=RED>Make New Settings</c>", NewSettings),
            });
            return settingsMenu.RunMenu(filename, savefilename)!;
        }

        private static string NewSettings(string filename, string savefilename)
        {
            GameConfig conf = new GameConfig();
            Console.WriteLine("Enter BoardSizeX:");
            var input = int.Parse(Console.ReadLine()?.Trim().ToUpper() ?? string.Empty);
            conf.BoardSizeX = input;
            Console.WriteLine("Enter BoardSizeY:");
            input = int.Parse(Console.ReadLine()?.Trim().ToUpper() ?? string.Empty);
            conf.BoardSizeY = input;
            Console.WriteLine("Enter TouchRule (0 - NoTouch, 1 - CornerTouch, 2 - SideTouch):");
            input = int.Parse(Console.ReadLine()?.Trim().ToUpper() ?? string.Empty);
            conf.EShipTouchRule = (EShipTouchRule) input;
            var shipinput = "";
            conf.ShipConfigs = new List<ShipConfig>();
            while (true)
            {
                var shipcon = new ShipConfig();
                Console.WriteLine("To exit ship creation - q");
                Console.WriteLine("Enter ship name:");
                shipinput = Console.ReadLine()?.Trim();
                if (shipinput != null && shipinput.ToUpper() == "Q")
                {
                    break;
                }
                shipcon.Name = shipinput;
                Console.WriteLine("Enter ship quantity:");
                input = Convert.ToInt32(Console.ReadLine()?.Trim().ToUpper());
                shipcon.Quantity = input;
                Console.WriteLine("Enter ShipSizeX:");
                input = Convert.ToInt32(Console.ReadLine()?.Trim().ToUpper());
                shipcon.ShipSizeX = input;
                Console.WriteLine("Enter ShipSizeY:");
                input = Convert.ToInt32(Console.ReadLine()?.Trim().ToUpper());
                shipcon.ShipSizeY = input;
                conf.ShipConfigs.Add(shipcon);
            }
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            var confJsonStr = JsonSerializer.Serialize(conf, jsonOptions);
            Console.WriteLine("Saving default config!");
            System.IO.File.WriteAllText(filename, confJsonStr);
            return "";
        }

        private static string ResetSettings(string filename, string savefilename)
        {
            GameConfig conf = new GameConfig();
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            var confJsonStr = JsonSerializer.Serialize(conf, jsonOptions);
            Console.WriteLine("Saving default config!");
            System.IO.File.WriteAllText(filename, confJsonStr);
            return "";
        }
    }
}