using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private static string? _filename;
        private static string? _saveFileName;
        private static string? _logFile;

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

            Console.Clear();
            _filename = _basePath + Path.DirectorySeparatorChar + "Configs" + Path.DirectorySeparatorChar +
                        "standard.json";
            _saveFileName = _basePath + Path.DirectorySeparatorChar + "SavedGames" + Path.DirectorySeparatorChar +
                            "game.json";
            _logFile = _basePath + Path.DirectorySeparatorChar + "GameLog" + Path.DirectorySeparatorChar + "log.json";
            var mainMenu = new Menu("<c=RED>Battle</c><c=BLUE>ships</c><c=GREEN> Game</c>", EMenuDepth.Main);
            mainMenu.AddMenuItems(new List<MenuItem>()
            {
                //new("1", "<c=BLUE>Test</c> BattleShip <c=RED>Game</c>", RunGame),
                //new("2", "<c=RED>See Game Settings</c>", SeeSettings),
                //new("3", "<c=BLUE>Test</c> Place <c=RED>Game</c>", PlaceGame),
                new("1", "<c=BLUE>Load</c> <c=RED>Game</c>", LoadGames),
                new("2", "<c=RED>New Game</c>", StartGame),
                new("3", "Delete Games", DeleteGames),
                new("4", "Delete Configs", DeleteConfigs)
            });
            mainMenu.RunMenu();
        }

        private static string StartGame(string s)
        {
            using var db = new ApplicationDbContext();
            var startMenu = new Menu("Choose config for the Game", EMenuDepth.SubMenu);
            var menuEntry = new MenuItem("C", "Create Config", NewSettings);
            startMenu.AddMenuItem(menuEntry);
            foreach (var gameConfig in db.Configs)
            {
                menuEntry = new MenuItem(gameConfig.ConfigId.ToString(), gameConfig.CreationTime, RunNewGame);
                startMenu.AddMenuItem(menuEntry);
            }

            return startMenu.RunMenu();
        }

        private static string LoadGames(string s)
        {
            using var db = new ApplicationDbContext();
            var loadMenu = new Menu("Load Games", EMenuDepth.SubMenu);
            if (File.Exists(_saveFileName))
            {
                var menuEntry = new MenuItem("0", "Load Local", RunLocalGame);
                loadMenu.AddMenuItem(menuEntry);
            }

            foreach (var game in db.Games)
            {
                var menuEntry = new MenuItem(game.GameId.ToString(),
                    game.Status.ToString() + " " + game.Status + " " + game.CreationTime + " " + game.ConfigId,
                    RunGame);
                loadMenu.AddMenuItem(menuEntry);
            }

            return loadMenu.RunMenu();
        }

        private static string DeleteGames(string s)
        {
            using var db = new ApplicationDbContext();
            var loadMenu = new Menu("Delete Games", EMenuDepth.SubMenu);
            if (File.Exists(_saveFileName))
            {
                var menuEntry = new MenuItem("0", "Delete Local", DeleteLocalGame);
                loadMenu.AddMenuItem(menuEntry);
            }

            foreach (var game in db.Games)
            {
                var menuEntry = new MenuItem(game.GameId.ToString(),
                    game.Status.ToString() + " " + game.Status + " " + game.CreationTime + " " + game.ConfigId,
                    DeleteGame);
                loadMenu.AddMenuItem(menuEntry);
            }

            loadMenu.RunMenu();
            return "";
        }

        private static string DeleteConfigs(string s)
        {
            using var db = new ApplicationDbContext();
            var loadMenu = new Menu("Delete Configs", EMenuDepth.SubMenu);
            foreach (var gameConfig in db.Configs)
            {
                var deleteMenuItem =
                    new MenuItem(gameConfig.ConfigId.ToString(), gameConfig.CreationTime, DeleteConfig);
                loadMenu.AddMenuItem(deleteMenuItem);
            }

            return loadMenu.RunMenu();
        }

        private static string RunNewGame(string confId)
        {
            using var db = new ApplicationDbContext();
            var savedConf = int.Parse(confId) == 0
                ? new GameConfig()
                : JsonSerializer.Deserialize<GameConfig>(db.Configs.Find(int.Parse(confId)).ConfigStr);

            BsBrain brain = new(savedConf);
            var jsonStr = brain.GetBrainJson(brain.Move());
            var saveGameDb = new Game
            {
                GameState = jsonStr,
                Status = brain.GetGameStatus(),
                ConfigId = int.Parse(confId),
                Config = db.Configs.Find(int.Parse(confId)),
                Replay = new Replay
                {
                    Replays = brain.GetLogJson(),
                }
            };
            db.Games.Add(saveGameDb);
            db.SaveChanges();
            PlayGame(brain, saveGameDb);
            return "";
        }

        private static string RunLocalGame(string s)
        {
            using var db = new ApplicationDbContext();
            const int gameId = 0;
            var brainConf = File.ReadAllText(_saveFileName!);
            var savedConf = JsonSerializer.Deserialize<GameConfig>(brainConf);
            BsBrain brain = new(savedConf);
            brain.RestoreBrainFromJson(brainConf);
            Console.WriteLine("Player " + (brain.Move() + 1) + " turn");
            var done = false;
            while (done != true)
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
                    if (brain.GetGameStatus() == EGameStatus.Placing)
                    {
                        foreach (var ship in brain.ListShips(brain.Move()))
                        {
                            if (ship.Coordinates.Count != 0) continue;
                            while (true)
                            {
                                switch (BsConsoleUi.ConsolePlacement(brain, ship, savedConf!.EShipTouchRule))
                                {
                                    case 0:
                                        var jsonStr = brain.GetBrainJson(brain.Move());
                                        Console.WriteLine("Saving your Game!");
                                        File.WriteAllText(_saveFileName!, jsonStr);
                                        File.WriteAllTextAsync(_filename!, savedConf.ToString());
                                        File.WriteAllTextAsync(_logFile!, brain.GetLogJson());
                                        Thread.Sleep(5000);
                                        return "";
                                    case 1:
                                        continue;
                                    case 2:
                                        break;
                                }

                                break;
                            }
                        }

                        brain.ChangePlayer();
                        foreach (var ship in brain.ListShips(brain.Move()).Where(ship => ship.Coordinates.Count == 0))
                        {
                            if (ship.Coordinates.Count != 0) continue;
                            while (true)
                            {
                                switch (BsConsoleUi.ConsolePlacement(brain, ship, savedConf!.EShipTouchRule))
                                {
                                    case 0:
                                        var jsonStr = brain.GetBrainJson(brain.Move());
                                        Console.WriteLine(jsonStr);
                                        File.WriteAllText(_saveFileName!, jsonStr);
                                        File.WriteAllTextAsync(_filename!, savedConf.ToString());
                                        File.WriteAllTextAsync(_logFile!, brain.GetLogJson());
                                        Thread.Sleep(5000);
                                        return "";
                                    case 1:
                                        continue;
                                    case 2:
                                        break;
                                }

                                break;
                            }
                        }

                        brain.ChangePlayer();
                        brain.StartGame();
                    }

                    var ff1 = BsConsoleUi.Move(brain);
                    if (ff1 == "OVER")
                    {
                        if (File.Exists(_saveFileName))
                        {
                            File.Delete(_saveFileName!);
                        }

                        Thread.Sleep(5000);
                        return "";
                    }

                    if (ff1 == "FF")
                    {
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
                        Console.WriteLine("Saving your Game!");
                        File.WriteAllText(_saveFileName!, jsonStr);
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

        private static void PlayGame(BsBrain brain, Game currentGame)
        {
            using var db = new ApplicationDbContext();
            currentGame = db.Games.Find(currentGame.GameId);
            if (brain.GetGameStatus() == EGameStatus.Finished)
            {
                Console.WriteLine("Player " + brain.Move() + " won!");
                while (true)
                {
                    Console.Write("Would you like to see the replay? (Y/N):");
                    var answer = Console.ReadLine()?.ToUpper().Trim();
                    switch (answer)
                    {
                        case "Y":
                            RePlayGame(currentGame.GameId);
                            return;
                        case "N":
                            Console.WriteLine("Thank you for playing!");
                            Console.WriteLine();
                            Thread.Sleep(2000);
                            return;
                    }
                }
            }

            Console.WriteLine("Player " + (brain.Move() + 1) + " turn");
            var done = false;
            while (done != true)
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
                        return;
                }
            }

            if (brain.GetGameStatus() == EGameStatus.Placing)
            {
                var savedConf = JsonSerializer.Deserialize<GameConfig>(db.Configs.Find(currentGame.ConfigId).ConfigStr);
                foreach (var ship in brain.ListShips(brain.Move()))
                {
                    if (ship.Coordinates.Count != 0) continue;
                    while (true)
                    {
                        switch (BsConsoleUi.ConsolePlacement(brain, ship, savedConf!.EShipTouchRule))
                        {
                            case 0:
                                var jsonStr = brain.GetBrainJson(brain.Move());
                                Console.WriteLine(jsonStr);
                                var testFile = _basePath + Path.DirectorySeparatorChar + "SavedGames" +
                                               Path.DirectorySeparatorChar + "game.json";
                                File.WriteAllText(testFile, jsonStr);
                                currentGame.GameState = brain.GetBrainJson(brain.Move());
                                currentGame.Status = brain.GetGameStatus();
                                db.SaveChanges();
                                Console.WriteLine("Your Game ID: " + currentGame.GameId);
                                Thread.Sleep(5000);
                                return;
                            case 1:
                                continue;
                            case 2:
                                break;
                        }

                        break;
                    }
                }

                brain.ChangePlayer();
                foreach (var ship in brain.ListShips(brain.Move()).Where(ship => ship.Coordinates.Count == 0))
                {
                    if (ship.Coordinates.Count != 0) continue;
                    while (true)
                    {
                        switch (BsConsoleUi.ConsolePlacement(brain, ship, savedConf!.EShipTouchRule))
                        {
                            case 0:
                                var jsonStr = brain.GetBrainJson(brain.Move());
                                Console.WriteLine(jsonStr);
                                var testFile = _basePath + Path.DirectorySeparatorChar + "SavedGames" +
                                               Path.DirectorySeparatorChar + "game.json";
                                File.WriteAllText(testFile, jsonStr);
                                currentGame.GameState = brain.GetBrainJson(brain.Move());
                                currentGame.Status = brain.GetGameStatus();
                                db.SaveChanges();
                                Console.WriteLine("Your Game ID: " + currentGame.GameId);
                                Thread.Sleep(5000);
                                return;
                            case 1:
                                continue;
                            case 2:
                                break;
                        }

                        break;
                    }
                }

                brain.ChangePlayer();
                brain.StartGame();
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
                    Replay? log;
                    if (ff1 == "OVER")
                    {
                        brain.GameFinish();
                        currentGame.Status = brain.GetGameStatus();
                        currentGame.GameState = brain.GetBrainJson(brain.Move());
                        log = db.Replays.Find(currentGame.ReplayId);
                        log.Replays = brain.GetLogJson();
                        db.SaveChanges();
                        Thread.Sleep(5000);
                        return;
                    }

                    if (ff1 == "FF")
                    {
                        brain.GameSurrender();
                        currentGame.Status = brain.GetGameStatus();
                        currentGame.GameState = brain.GetBrainJson(brain.Move());
                        log = db.Replays.Find(currentGame.ReplayId);
                        log.Replays = brain.GetLogJson();
                        db.SaveChanges();
                        Thread.Sleep(5000);
                        return;
                    }

                    if (ff1 == "MISS")
                    {
                        Console.WriteLine("You Missed!");
                        currentGame.GameState = brain.GetBrainJson(brain.Move());
                        currentGame.Status = brain.GetGameStatus();
                        log = db.Replays.Find(currentGame.ReplayId);
                        log.Replays = brain.GetLogJson();
                        db.SaveChanges();
                        break;
                    }

                    if (ff1 == "SAVE")
                    {
                        var jsonStr = brain.GetBrainJson(brain.Move());
                        Console.WriteLine(jsonStr);
                        var testFile = _basePath + Path.DirectorySeparatorChar + "SavedGames" +
                                       Path.DirectorySeparatorChar + "game.json";
                        File.WriteAllText(testFile, jsonStr);
                        currentGame.GameState = brain.GetBrainJson(brain.Move());
                        currentGame.Status = brain.GetGameStatus();
                        log = db.Replays.Find(currentGame.ReplayId);
                        log.Replays = brain.GetLogJson();
                        db.SaveChanges();
                        Console.WriteLine("Your Game ID: " + currentGame.GameId);
                        Thread.Sleep(5000);
                        return;
                    }

                    Console.WriteLine("You Hit!");
                    log = db.Replays.Find(currentGame.ReplayId);
                    log.Replays = brain.GetLogJson();
                    currentGame.GameState = brain.GetBrainJson(brain.Move());
                    currentGame.Status = brain.GetGameStatus();
                    db.SaveChanges();
                }

                Thread.Sleep(5000);
            }
        }

        private static string RunGame(string gameId)
        {
            BsBrain brain = new(new GameConfig());
            using var db = new ApplicationDbContext();
            /*if (File.Exists(SaveFileName) && File.Exists(Filename))
            {
                Console.WriteLine("Detected a local saved game! Would you like to continue it?(y/n):");
                switch (Console.ReadLine()?.Trim().ToUpper())
                {
                    case "Y":
                        var brainConf = File.ReadAllText(SaveFileName);
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
                                var confText = File.ReadAllText(Filename);
                                var conf = JsonSerializer.Deserialize<GameConfig>(confText);
                                brain = new BsBrain(conf);
                                break;
                        }
                        break;
                }
            }
            else if (File.Exists(Filename))
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
                        var confText = File.ReadAllText(Filename);
                        var conf = JsonSerializer.Deserialize<GameConfig>(confText);
                        Console.WriteLine("Randomize ship placement?(y/n):");
                        switch (Console.ReadLine()?.Trim().ToUpper())
                        {
                            case "Y":
                                brain = new BsBrain(conf);
                                break;
                            case "N":
                                if (conf != null) conf.IsRandom = false;
                                brain = new BsBrain(conf);
                                foreach (var ship in brain.ListShips(brain.Move()))
                                {
                                    while(!BsConsoleUi.ConsolePlacement(brain, ship, conf!.EShipTouchRule));
                                }
                                brain.ChangePlayer();
                                foreach (var ship in brain.ListShips(brain.Move()))
                                {
                                    while(!BsConsoleUi.ConsolePlacement(brain, ship, conf!.EShipTouchRule));
                                }
                                brain.ChangePlayer();
                                break;
                        }
                        break;
                }
            }
            else
            {
                Console.WriteLine("No Config found, loading default...");
                Console.WriteLine("Randomize ship placement?(y/n):");
                GameConfig config = new GameConfig();
                switch (Console.ReadLine()?.Trim().ToUpper())
                {
                    case "Y":
                        brain = new BsBrain(config);
                        break;
                    case "N":
                        config.IsRandom = false;
                        brain = new BsBrain(config);
                        foreach (var ship in brain.ListShips(brain.Move()))
                        {
                            while(!BsConsoleUi.ConsolePlacement(brain, ship, config.EShipTouchRule));
                        }
                        brain.ChangePlayer();
                        foreach (var ship in brain.ListShips(brain.Move()))
                        {
                            while(!BsConsoleUi.ConsolePlacement(brain, ship, config.EShipTouchRule));
                        }
                        brain.ChangePlayer();
                        break;
                }
            }
            */
            var id = Convert.ToInt32(gameId);
            var currentGame = db.Games.Find(id);
            brain.RestoreBrainFromJson(db.Games.Find(id).GameState);
            /*if (brain.GetGameStatus() == EGameStatus.Placing)
            {
                var savedConf = JsonSerializer.Deserialize<GameConfig>(db.Configs.Find(currentGame.ConfigId).ConfigStr);
                foreach (var ship in brain.ListShips(brain.Move()))
                {
                    while(!BsConsoleUi.ConsolePlacement(brain, ship, savedConf!.EShipTouchRule))
                    {
                        brain.StartGame();
                        currentGame.GameState = brain.GetBrainJson(brain.Move());
                        currentGame.Status = brain.GetGameStatus();
                        db.SaveChanges();
                    }
                }
                brain.ChangePlayer();
                foreach (var ship in brain.ListShips(brain.Move()))
                {
                    while(!BsConsoleUi.ConsolePlacement(brain, ship, savedConf!.EShipTouchRule))
                    {
                        brain.StartGame();
                        currentGame.GameState = brain.GetBrainJson(brain.Move());
                        currentGame.Status = brain.GetGameStatus();
                        db.SaveChanges();
                    }
                }
                brain.ChangePlayer();
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
                    if (ff1 == "OVER")
                    {
                        if (File.Exists(SaveFileName))
                        {
                            File.Delete(SaveFileName);
                        }
                        if (id != 0)
                        {
                            db.Games.Remove(currentGame);
                            db.SaveChanges();
                        }
                        Thread.Sleep(5000);
                        return "";
                    }
                    if (ff1 == "FF")
                    {
                        if (File.Exists(SaveFileName))
                        {
                            File.Delete(SaveFileName);
                        }
                        if (id != 0)
                        {
                            db.Games.Remove(currentGame);
                            db.SaveChanges();
                        }
                        Thread.Sleep(5000);
                        return "";
                    }
                    if (ff1 == "MISS")
                    {
                        Console.WriteLine("You Missed!");
                        currentGame.GameState = brain.GetBrainJson(brain.Move());
                        currentGame.Status = brain.GetGameStatus();
                        db.SaveChanges();
                        break;
                    }
                    if (ff1 == "SAVE")
                    {
                        var jsonStr = brain.GetBrainJson(brain.Move());
                        Console.WriteLine(jsonStr);
                        File.WriteAllText(SaveFileName, jsonStr);
                        currentGame.GameState = brain.GetBrainJson(brain.Move());
                        currentGame.Status = brain.GetGameStatus();
                        db.SaveChanges();
                        Console.WriteLine("Your Game ID: " + currentGame.GameId);
                        Thread.Sleep(5000);
                        return "";
                    }

                    Console.WriteLine("You Hit!");
                    currentGame.GameState = brain.GetBrainJson(brain.Move());
                    currentGame.Status = brain.GetGameStatus();
                    db.SaveChanges();

                }
                Thread.Sleep(5000);
            }
            */
            PlayGame(brain, currentGame);
            return "";
        }

        /*private static string SeeSettings(string empty)
        {
            GameConfig? config;
            if (File.Exists(_filename))
            {
                var confText = File.ReadAllText(_filename!);
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
            return settingsMenu.RunMenu();
        }*/

        private static string NewSettings(string s)
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
            using var db = new ApplicationDbContext();
            var saveConfDb = new Config
            {
                ConfigStr = confJsonStr
            };
            db.Configs.Add(saveConfDb);
            db.SaveChanges();
            Console.WriteLine("Your Conf ID: " + saveConfDb.ConfigId);
            Thread.Sleep(5000);
            return "";
        }

        /*private static string ResetSettings(string filename)
        {
            GameConfig conf = new();
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            var confJsonStr = JsonSerializer.Serialize(conf, jsonOptions);
            Console.WriteLine("Saving default config!");
            File.WriteAllText(filename, confJsonStr);
            return "";
        }*/

        /*private static string DbSettingsImport(string filename)
        {
            using var db = new ApplicationDbContext();
            Console.WriteLine("Insert Game ID:");
            var id = Convert.ToInt32(Console.ReadLine()?.Trim());
            File.WriteAllText(filename, db.Configs.Find(id).ConfigStr);
            return "";
        }*/

        /*private static string PlaceGame(string s)
        {
            GameConfig config = new()
            {
                IsRandom = false
            };
            BsBrain brain = new BsBrain(config);
            foreach (var ship in brain.ListShips(brain.Move()))
            {
                BsConsoleUi.ConsolePlacement(brain, ship, config.EShipTouchRule);
            }

            brain.ChangePlayer();
            foreach (var ship in brain.ListShips(brain.Move()))
            {
                BsConsoleUi.ConsolePlacement(brain, ship, config.EShipTouchRule);
            }

            Console.WriteLine("First player board:");
            BsConsoleUi.DrawBoard(brain.GetBoard(0), brain.GetFireBoard(0));
            Console.WriteLine("Second player board:");
            BsConsoleUi.DrawBoard(brain.GetBoard(1), brain.GetFireBoard(1));
            Thread.Sleep(50000);
            return "";
        }
        */

        private static string DeleteGame(string gameId)
        {
            using var db = new ApplicationDbContext();
            var id = Convert.ToInt32(gameId);
            var game = db.Games.Find(id);
            var replay = db.Replays.Find(game.ReplayId);
            db.Replays.Remove(replay);
            db.Games.Remove(game);
            db.SaveChanges();
            return "";
        }

        private static string DeleteConfig(string confId)
        {
            using var db = new ApplicationDbContext();
            var id = Convert.ToInt32(confId);
            var config = db.Configs.Find(id);
            List<Game> games = db.Games.ToList();
            if (games.Any(game => config.ConfigId == game.ConfigId))
            {
                Console.WriteLine("Cannot delete this Config, it still has games linked to it!");
                Thread.Sleep(5000);
                return "";
            }

            db.Configs.Remove(db.Configs.Find(id));
            db.SaveChanges();
            return "";
        }

        private static string DeleteLocalGame(string s)
        {
            File.Delete(_saveFileName!);
            File.Delete(_filename!);
            File.Delete(_logFile!);
            return "";
        }

        private static void RePlayGame(int id)
        {
            using var ctx = new ApplicationDbContext();
            var config = new GameConfig();
            var replay = new List<ReplayTile>();
            if (id == 0)
            {
                config = JsonSerializer.Deserialize<GameConfig>(File.ReadAllText(_filename!));
                replay = JsonSerializer.Deserialize<List<ReplayTile>>(File.ReadAllText(_logFile!));
            }
            else
            {
                var game = ctx.Games.Find(id);
                if (game.Status != EGameStatus.Finished)
                {
                    Console.WriteLine("Game is Not Finished!"); //may be redundant
                    Thread.Sleep(5000);
                    return;
                }

                config = game.ConfigId != null
                    ? JsonSerializer.Deserialize<GameConfig>(ctx.Configs.Find(game.ConfigId).ConfigStr)
                    : new GameConfig();
                var gameLog = ctx.Replays.Find(game.ReplayId);
                replay = JsonSerializer.Deserialize<List<ReplayTile>>(gameLog.Replays!);
            }
            
            var placementSkip = replay!.Count(play => play.Placing);

            var move = 0;
            ConsoleKey key;

            Console.CursorVisible = false;
            do
            {
                var board1 = new BoardSquareState[config!.BoardSizeX, config.BoardSizeY];
                var board2 = new BoardSquareState[config.BoardSizeX, config.BoardSizeY];
                for (var i = 0; i < move; i++)
                {
                    var tile = replay![i];
                    switch (tile.Player)
                    {
                        case 0:
                            switch (tile.Placing)
                            {
                                case true:
                                    board1[tile.X, tile.Y] = new BoardSquareState
                                    {
                                        IsBomb = tile.IsBomb,
                                        IsShip = tile.IsShip
                                    };
                                    break;
                                case false:
                                    board2[tile.X, tile.Y] = new BoardSquareState
                                    {
                                        IsBomb = tile.IsBomb,
                                        IsShip = tile.IsShip
                                    };
                                    break;
                            }

                            break;
                        case 1:
                            switch (tile.Placing)
                            {
                                case true:
                                    board2[tile.X, tile.Y] = new BoardSquareState
                                    {
                                        IsBomb = tile.IsBomb,
                                        IsShip = tile.IsShip
                                    };
                                    break;
                                case false:
                                    board1[tile.X, tile.Y] = new BoardSquareState
                                    {
                                        IsBomb = tile.IsBomb,
                                        IsShip = tile.IsShip
                                    };
                                    break;
                            }

                            break;
                    }
                }

                Console.Clear();
                Console.WriteLine("Player 1 & Player 2 boards");
                BsConsoleUi.DrawBoard(board1, board2);
                Console.WriteLine("Use LEFT arrow key to go back a step");
                Console.WriteLine("Use RIGHT arrow key to go further a step");
                Console.WriteLine("Press S to skip to ship placements");
                Console.WriteLine("Press X to return");
                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                    {
                        if (move <= 0)
                        {
                        }
                        else
                        {
                            move--;  
                        }

                        break;
                    }
                    case ConsoleKey.RightArrow:
                    {
                        if (move >= replay!.Count)
                        {
                        }
                        else
                        {
                            move++;  
                        }
                        break;
                    }
                    case ConsoleKey.S:
                    {
                        move = placementSkip;
                        break;
                    }
                    case ConsoleKey.X:
                    {
                        return;
                    }
                }
            } while (key != ConsoleKey.Enter);

            Console.CursorVisible = true;
        }
    }
}