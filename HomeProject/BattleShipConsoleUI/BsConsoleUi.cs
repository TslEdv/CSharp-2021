using System;
using BattleShipBrain;

namespace BattleShipConsoleUI
{
    public static class BsConsoleUi
    {
        public static string Move(BsBrain brain)
        {
            return brain.Move() switch
            {
                0 => Player1Move(brain),
                1 => Player2Move(brain),
                _ => ""
            };
        }
        public static string Player1Move(BsBrain brain)
        {
            int x, y;
            Console.WriteLine("To Forfeit, enter 999 into X-coordinate");
            Console.WriteLine("To Save Game, enter 888 into X-coordinate");
            Console.Write("Player 1, choose X-coordinate:");
            if (!int.TryParse(Console.ReadLine()?.ToUpper().Trim(), out x))
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
            if (!int.TryParse(Console.ReadLine()?.ToUpper().Trim(), out y))
            {
                y = 0;
            }
            
            return brain.Player1Move(x,y);
        }
        public static string Player2Move(BsBrain brain)
        {
            int x, y;
            Console.WriteLine("To Forfeit, enter 999 into X-coordinate");
            Console.WriteLine("To Save Game, enter 888 into X-coordinate");
            Console.Write("Player 2, choose X-coordinate:");
            if (!int.TryParse(Console.ReadLine()?.ToUpper().Trim(), out x))
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
            if (!int.TryParse(Console.ReadLine()?.ToUpper().Trim(), out y))
            {
                y = 0;
            }
            
            return brain.Player2Move(x,y);
        }
        public static void DrawBoard(BoardSquareState[,] board)
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
}