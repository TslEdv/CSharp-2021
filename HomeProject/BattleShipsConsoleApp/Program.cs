using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using BattleShipBrain;
using BattleShipConsoleUI;
using DAL;
using Domain;
using MenuSystem;

namespace BattleShipsConsoleApp
{
    class Program
    {
        private static string? _basePath;
        static void Main(string[] args)
        {
            _basePath = args.Length == 1 ? args[0] : Directory.GetCurrentDirectory();
            if (!Directory.Exists(_basePath + Path.DirectorySeparatorChar + "Configs"))
            {
                Directory.CreateDirectory(_basePath + Path.DirectorySeparatorChar + "Configs");
            }

            if (!Directory.Exists(_basePath + Path.DirectorySeparatorChar + "SavedGames"))
            {
                Directory.CreateDirectory(_basePath + Path.DirectorySeparatorChar + "SavedGames");
            }
            var fileNameStandardConfig = _basePath + Path.DirectorySeparatorChar + "Configs" + Path.DirectorySeparatorChar + "standard.json";
            var fileNameSavedGame = _basePath + Path.DirectorySeparatorChar + "SavedGames" + Path.DirectorySeparatorChar + "game.json";
            Console.Clear();
            var mainMenu = new Menu("<c=RED>Battle</c><c=BLUE>ships</c><c=GREEN> Game</c>", EMenuDepth.Main);
            mainMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("1", "<c=BLUE>Test</c> BattleShip <c=RED>Game</c>", RunGame),
                new MenuItem("2", "<c=RED>See Game Settings</c>", SeeSettings),
            });
            mainMenu.RunMenu(fileNameStandardConfig,fileNameSavedGame);
        }

        private static string RunGame(string filename, string savefilename)
        {
            BsBrain? brain = new BsBrain(new GameConfig());
            using var db = new ApplicationDbContext();
            if (File.Exists(savefilename) && File.Exists(filename))
            {
                Console.WriteLine("Detected a local saved game! Would you like to continue it?(y/n):");
                switch (Console.ReadLine()?.Trim().ToUpper())
                {
                    case "Y":
                        var brainconf = File.ReadAllText(savefilename);
                        brain.RestoreBrainFromJson(brainconf);
                        break;
                    case "N":
                        Console.WriteLine("Load a saved game from database?(y/n):");
                        switch (Console.ReadLine()?.Trim().ToUpper())
                        {
                            case "Y":
                                Console.WriteLine("Insert Game ID:");
                                var id = Convert.ToInt32(Console.ReadLine()?.Trim());
                                brain.RestoreBrainFromJson(db.Games.Find(id).GameState);
                                db.Games.Remove(db.Games.Find(id));
                                db.SaveChanges();
                                break;
                            case "N":
                                Console.WriteLine("Loading config...");
                                var confText = File.ReadAllText(filename);
                                var conf = JsonSerializer.Deserialize<GameConfig>(confText);
                                brain = new BsBrain(conf);
                                break;
                        }
                        break;
                }
            }
            else if (File.Exists(filename))
            {
                Console.WriteLine("Load a saved game from database?(y/n):");
                switch (Console.ReadLine()?.Trim().ToUpper())
                {
                    case "Y":
                        Console.WriteLine("Insert Game ID:");
                        var id = Convert.ToInt32(Console.ReadLine()?.Trim());
                        brain.RestoreBrainFromJson(db.Games.Find(id).GameState);
                        db.Games.Remove(db.Games.Find(id));
                        db.SaveChanges();
                        break;
                    case "N":
                        Console.WriteLine("Loading config...");
                        var confText = File.ReadAllText(filename);
                        var conf = JsonSerializer.Deserialize<GameConfig>(confText);
                        brain = new BsBrain(conf);
                        break;
                }
            }
            else
            {
                Console.WriteLine("No Config found, loading default...");
                brain = new BsBrain(new GameConfig());

            }
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
                BsConsoleUi.DrawBoard(brain.GetBoard(brain.Move()));
                while (true)
                {
                    var ff1 = BsConsoleUi.Move(brain);
                    if (ff1 == "FF")
                    {
                        if (File.Exists(savefilename))
                        {
                            File.Delete(savefilename);
                        } 
                        Thread.Sleep(5000);
                        return "";
                    }
                    if (ff1 == "MISS")
                    {
                        Console.WriteLine("You Missed!");
                        break;
                    }
                    if (ff1 == "SAVE")
                    {
                        var jsonstr = brain.GetBrainJson(brain.Move());
                        Console.WriteLine(jsonstr);
                        File.WriteAllText(savefilename, jsonstr);
                        var savegamedb = new Game
                        {
                            GameState = jsonstr
                        };
                        db.Games.Add(savegamedb);
                        db.SaveChanges();
                        Console.WriteLine("Your Game ID: " + savegamedb.GameId);
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
            if (File.Exists(filename))
            {
                var confText = File.ReadAllText(filename);
                config = JsonSerializer.Deserialize<GameConfig>(confText);
                
            }
            else
            {
                config = new GameConfig();
            }
            var settingsMenu = new Menu("<c=RED>Set</c><c=BLUE>ti</c><c=GREEN>ngs</c>", EMenuDepth.SubMenu);
            settingsMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("1", $"Reset current config:" + Environment.NewLine + $"{config}", ResetSettings),
                new MenuItem("2", "<c=RED>Make New Settings</c>", NewSettings),
                new MenuItem("3", "<c=BLUE>Import Setting From DB</c>", DbSettingsImport),

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
            conf.ShipConfigs = new List<ShipConfig>();
            while (true)
            {
                var shipcon = new ShipConfig();
                Console.WriteLine("To exit ship creation - q");
                Console.WriteLine("Enter ship name:");
                var shipinput = Console.ReadLine()?.Trim();
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
            using var db = new ApplicationDbContext();
            var saveconfdb = new Config
            {
                ConfigStr = confJsonStr
            };
            db.Configs.Add(saveconfdb);
            db.SaveChanges();
            Console.WriteLine("Your Conf ID: " + saveconfdb.ConfigId);
            File.WriteAllText(filename, confJsonStr);
            Thread.Sleep(5000);
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
            File.WriteAllText(filename, confJsonStr);
            return "";
        }

        private static string DbSettingsImport(string filename, string savefilename)
        {
            using var db = new ApplicationDbContext();
            Console.WriteLine("Insert Game ID:");
            var id = Convert.ToInt32(Console.ReadLine()?.Trim());
            File.WriteAllText(filename, db.Configs.Find(id).ConfigStr);
            return "";
        }
    }
}