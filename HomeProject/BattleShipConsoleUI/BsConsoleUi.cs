using System;
using BattleShipBrain;

namespace BattleShipConsoleUI
{
    public static class BsConsoleUi
    {
        public static void DrawBoard(BoardSquareState[,] board)
        {
            Console.Write($"   F  ");
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
                    Console.Write(board[x,y]);
                }
                Console.WriteLine("|");
                for (var x = 0; x < board.GetLength(0)+1; x++)
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
            for (var x = 0; x < board.GetLength(0)+1; x++)
            {
                Console.Write($"=======");
            }
            Console.WriteLine();
        }
    }
}