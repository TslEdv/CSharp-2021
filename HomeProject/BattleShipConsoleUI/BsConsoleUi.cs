using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Xml;
using BattleShipBrain;

namespace BattleShipConsoleUI
{
    public static class BsConsoleUi
    {
        public static string Move(BsBrain brain)
        {
            if (!brain.GameFinish())
                return brain.Move() switch
                {
                    0 => Player1Move(brain),
                    1 => Player2Move(brain),
                    _ => ""
                };
            Console.WriteLine("Game Over! Player " + (brain.Move() + 1) + "wins!");
            return "OVER";
        }

        private static string Player1Move(BsBrain brain)
        {
            Console.WriteLine("To Forfeit, enter 999 into X-coordinate");
            Console.WriteLine("To Save Game, enter 888 into X-coordinate");
            Console.Write("Player 1, choose X-coordinate:");
            if (!int.TryParse(Console.ReadLine()?.ToUpper().Trim(), out var x))
            {
                x = 0;
            }

            if (x == 999)
            {
                Console.WriteLine("Player 1 has forfeited, Player 2 wins!");
                return "FF";
            }

            if (x == 888)
            {
                Console.WriteLine("Saving the game...");
                return "SAVE";
            }

            Console.Write("Player 1, choose Y-coordinate:");
            if (!int.TryParse(Console.ReadLine()?.ToUpper().Trim(), out var y))
            {
                y = 0;
            }

            return brain.Player1Move(x, y);
        }

        private static string Player2Move(BsBrain brain)
        {
            Console.WriteLine("To Forfeit, enter 999 into X-coordinate");
            Console.WriteLine("To Save Game, enter 888 into X-coordinate");
            Console.Write("Player 2, choose X-coordinate:");
            if (!int.TryParse(Console.ReadLine()?.ToUpper().Trim(), out var x))
            {
                x = 0;
            }

            if (x == 999)
            {
                Console.WriteLine("Player 2 has forfeited, Player 1 wins!");
                return "FF";
            }

            if (x == 888)
            {
                Console.WriteLine("Saving the game...");
                return "SAVE";
            }

            Console.Write("Player 2, choose Y-coordinate:");
            if (!int.TryParse(Console.ReadLine()?.ToUpper().Trim(), out var y))
            {
                y = 0;
            }

            return brain.Player2Move(x, y);
        }
        
        public static void DrawBoard(BoardSquareState[,] boardOwn, BoardSquareState[,] boardFire)
        {
            List<BoardSquareState[,]> boards = new List<BoardSquareState[,]>
            {
                boardOwn,
                boardFire
            };
            foreach (var board in boards)
            {
                Console.Write($"      ");
                for (var x = 0; x < board.GetLength(0); x++)
                {
                    Console.Write($"   {x}  ");
                }

                Console.WriteLine();
                for (var x = 0; x < board.GetLength(0) + 1; x++)
                {
                    if (x > 0)
                    {
                        Console.Write($"+-----");
                    }
                    else
                    {
                        Console.Write("      ");
                    }
                }

                for (var y = 0; y < board.GetLength(1); y++)
                {
                    Console.WriteLine("+");
                    Console.Write($"   {y}  ");
                    for (var x = 0; x < board.GetLength(0); x++)
                    {
                        Console.Write("|  ");
                        Console.Write(board[x, y]);
                    }

                    Console.WriteLine("|");
                    for (var x = 0; x < board.GetLength(0) + 1; x++)
                    {
                        if (x > 0)
                        {
                            Console.Write($"+-----");
                        }
                        else
                        {
                            Console.Write("      ");
                        }
                    }
                }

                Console.WriteLine("+");
                for (var x = 0; x < board.GetLength(0) + 1; x++)
                {
                    Console.Write($"=======");
                }

                Console.WriteLine();
            }
        }

        public static int ConsolePlacement(BsBrain brain, Ship ship, EShipTouchRule rule)
        {
            BoardSquareState[,] board = brain.GetBoard(brain.Move());
            var x = 0;
            var y = 0;
            var xEnd = x + ship.Length - 1;
            var yEnd = y + ship.Height - 1;

            ConsoleKey key;

            Console.CursorVisible = false;
            do
            {
                var coordinateList = new List<Coordinate>();
                for (var i = x; i < xEnd + 1; i++)
                {
                    for (var j = y; j < yEnd + 1; j++)
                    {
                        coordinateList.Add(new Coordinate()
                        {
                            X = i,
                            Y = j
                        });
                    }
                }

                Console.Clear();

                DrawPlacementBoard(board, coordinateList);

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                    {
                        if (x != 0)
                        {
                            x--;
                            xEnd--;
                        }

                        break;
                    }
                    case ConsoleKey.RightArrow:
                    {
                        if (x != board.GetLength(0) - 1 && xEnd != board.GetLength(0) - 1)
                        {
                            x++;
                            xEnd++;
                        }

                        break;
                    }
                    case ConsoleKey.UpArrow:
                    {
                        if (y != 0)
                        {
                            y--;
                            yEnd--;
                        }

                        break;
                    }
                    case ConsoleKey.DownArrow:
                    {
                        if (y != board.GetLength(1) - 1 && yEnd != board.GetLength(1) - 1)
                        {
                            y++;
                            yEnd++;
                        }

                        break;
                    }
                    case ConsoleKey.R:
                    {
                        if (xEnd == x + ship.Length - 1)
                        {
                            xEnd = x + ship.Height - 1;
                            yEnd = y + ship.Length - 1;
                        }
                        else
                        {
                            xEnd = x + ship.Length - 1;
                            yEnd = y + ship.Height - 1;
                        }

                        break;
                    }
                    case ConsoleKey.X:
                    {
                        return 0;
                    }
                }
            } while (key != ConsoleKey.Enter);

            Console.CursorVisible = true;
            switch (brain.PlaceShips(x, xEnd, y, yEnd, ship, rule))
            {
                case false:
                    return 1;
                case true:
                    return 2;
            }
        }

        private static void DrawPlacementBoard(BoardSquareState[,] board, List<Coordinate> coordinates)
        {
            Console.Write($"      ");
            for (var x = 0; x < board.GetLength(0); x++)
            {
                Console.Write($"   {x}  ");
            }

            Console.WriteLine();
            for (var x = 0; x < board.GetLength(0) + 1; x++)
            {
                if (x > 0)
                {
                    Console.Write($"+-----");
                }
                else
                {
                    Console.Write("      ");
                }
            }

            for (var y = 0; y < board.GetLength(1); y++)
            {
                Console.WriteLine("+");
                Console.Write($"   {y}  ");
                for (var x = 0; x < board.GetLength(0); x++)
                {
                    if (coordinates.Any(coordinate => x == coordinate.X && y == coordinate.Y))
                    {
                        Console.Write("|  ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(board[x, y]);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write("|  ");
                        Console.Write(board[x, y]);
                    }
                }

                Console.WriteLine("|");
                for (var x = 0; x < board.GetLength(0) + 1; x++)
                {
                    if (x > 0)
                    {
                        Console.Write($"+-----");
                    }
                    else
                    {
                        Console.Write("      ");
                    }
                }
            }

            Console.WriteLine("+");
            for (var x = 0; x < board.GetLength(0) + 1; x++)
            {
                Console.Write($"=======");
            }

            Console.WriteLine();
            Console.WriteLine("Press X to SAVE and EXIT");
            Console.WriteLine("Press R to rotate the ship");
            Console.WriteLine("Press ENTER to place the ship");
            Console.WriteLine();
        }
    }
}