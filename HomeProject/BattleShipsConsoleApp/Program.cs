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
    internal static class Program
    {
        private static string? _basePath;

        private static void Main(string[] args)
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

        private static string RunGame(string filename, string saveFileName)
        {
            BsBrain brain = new BsBrain(new GameConfig());
            var gameId = 0;
            using var db = new ApplicationDbContext();
            if (File.Exists(saveFileName) && File.Exists(filename))
            {
                Console.WriteLine("Detected a local saved game! Would you like to continue it?(y/n):");
                switch (Console.ReadLine()?.Trim().ToUpper())
                {
                    case "Y":
                        var brainConf = File.ReadAllText(saveFileName);
                        brain.RestoreBrainFromJson(brainConf);
                        break;
                    case "N":
                        Console.WriteLine("Load a saved game from database?(y/n):");
                        switch (Console.ReadLine()?.Trim().ToUpper())
                        {
                            case "Y":
                                Console.WriteLine("Insert Game ID:");
                                var id = Convert.ToInt32(Console.ReadLine()?.Trim());
                                brain.RestoreBrainFromJson(db.Games.Find(id).GameState);
                                gameId = id;
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
                        gameId = id;
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
            BsConsoleUi.DrawBoard(brain.GetBoard(0), brain.GetFireBoard(0));
            Console.WriteLine("Second player board:");
            BsConsoleUi.DrawBoard(brain.GetBoard(1), brain.GetFireBoard(1));
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
                BsConsoleUi.DrawBoard(brain.GetBoard(brain.Move()), brain.GetFireBoard(brain.Move()));
                foreach (var ship in brain.ListShips(brain.Move()))
                {
                    Console.WriteLine("Name: " + ship.Name);
                    Console.WriteLine("Size: " + ship.GetShipSize());
                    Console.WriteLine("Damage: " + ship.GetShipDamageCount(brain.GetBoard(brain.Move())));
                    Console.WriteLine("Status: " + ship.IsShipSunk(brain.GetBoard(brain.Move())));
                }
                while (true)
                {
                    var ff1 = BsConsoleUi.Move(brain);
                    if (ff1 == "FF")
                    {
                        if (File.Exists(saveFileName))
                        {
                            File.Delete(saveFileName);
                        }
                        if (gameId != 0)
                        {
                            db.Games.Remove(db.Games.Find(gameId));
                            db.SaveChanges();
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
                        var jsonStr = brain.GetBrainJson(brain.Move());
                        Console.WriteLine(jsonStr);
                        File.WriteAllText(saveFileName, jsonStr);
                        var saveGameDb = db.Games.Find(gameId);
                        saveGameDb.GameState = jsonStr;
                        db.SaveChanges();
                        Console.WriteLine("Your Game ID: " + saveGameDb.GameId);
                        Thread.Sleep(5000);
                        return "";
                    }
                    Console.WriteLine("You Hit!");
                }
                Thread.Sleep(5000);
            }
        }

        private static string SeeSettings(string filename, string saveFileName)
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
            return settingsMenu.RunMenu(filename, saveFileName)!;
        }

        private static string NewSettings(string filename, string saveFileName)
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
                var shipConf = new ShipConfig();
                Console.WriteLine("To exit ship creation - q");
                Console.WriteLine("Enter ship name:");
                var shipInput = Console.ReadLine()?.Trim();
                if (shipInput != null && shipInput.ToUpper() == "Q")
                {
                    break;
                }
                shipConf.Name = shipInput;
                Console.WriteLine("Enter ship quantity:");
                input = Convert.ToInt32(Console.ReadLine()?.Trim().ToUpper());
                shipConf.Quantity = input;
                Console.WriteLine("Enter ShipSizeX:");
                input = Convert.ToInt32(Console.ReadLine()?.Trim().ToUpper());
                shipConf.ShipSizeX = input;
                Console.WriteLine("Enter ShipSizeY:");
                input = Convert.ToInt32(Console.ReadLine()?.Trim().ToUpper());
                shipConf.ShipSizeY = input;
                conf.ShipConfigs.Add(shipConf);
            }
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            var confJsonStr = JsonSerializer.Serialize(conf, jsonOptions);
            Console.WriteLine("Saving default config!");
            using var db = new ApplicationDbContext();
            var saveConfDb = new Config
            {
                ConfigStr = confJsonStr
            };
            db.Configs.Add(saveConfDb);
            db.SaveChanges();
            Console.WriteLine("Your Conf ID: " + saveConfDb.ConfigId);
            File.WriteAllText(filename, confJsonStr);
            Thread.Sleep(5000);
            return "";
        }

        private static string ResetSettings(string filename, string saveFileName)
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

        private static string DbSettingsImport(string filename, string saveFileName)
        {
            using var db = new ApplicationDbContext();
            Console.WriteLine("Insert Game ID:");
            var id = Convert.ToInt32(Console.ReadLine()?.Trim());
            File.WriteAllText(filename, db.Configs.Find(id).ConfigStr);
            return "";
        }
    }
}