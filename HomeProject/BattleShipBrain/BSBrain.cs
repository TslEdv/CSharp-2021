using System;
using System.Threading;

namespace BattleShipBrain
{
    public class BsBrain
    {
        private readonly BoardSquareState[,] _boardA;
        private readonly BoardSquareState[,] _boardB;

        private readonly Random _rnd = new Random();

        public BsBrain(int xSize, int ySize)
        {
            _boardA = new BoardSquareState[xSize, ySize];
            _boardB = new BoardSquareState[xSize, ySize];

            for (var x = 0; x < xSize; x++)
            {
                for (var y = 0; y < ySize; y++)
                {
                    _boardA[x, y] = new BoardSquareState
                    {
                        IsBomb = _rnd.Next(0,2) != 0,
                        IsShip = _rnd.Next(0,2) != 0
                    };
                    _boardB[x, y] = new BoardSquareState
                    {
                        IsBomb = _rnd.Next(0,2) != 0,
                        IsShip = _rnd.Next(0,2) != 0
                    };
                }
            }
        }

        public BoardSquareState[,] GetBoard(int playerNo)
        {
            return CopyOfBoard(playerNo == 0 ? _boardA : _boardB);
        }

        private BoardSquareState[,] CopyOfBoard (BoardSquareState[,] board)
        {
            var res = new BoardSquareState[board.GetLength(0), board.GetLength(1)];
            for (var x = 0; x < board.GetLength(0); x++)
            {
                for (var y = 0; y < board.GetLength(1); y++)
                {
                    {
                        res[x,y] = board[x,y];
                    };
                }
            }
            return res;
        }

        public string Player1Move()
        {
            int x, y;
            Console.WriteLine("To Forfeit, enter 999 into X-coordinate");
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
            Console.Write("Player 1, choose Y-coordinate:");
            if (!int.TryParse(Console.ReadLine()?.ToUpper().Trim(), out y))
            {
                y = 0;
            }
            _boardA[x, y].Bombing();
            switch (_boardA[x,y].IsShip,_boardA[x,y].IsBomb)
            {
                case (true,true):
                    Console.WriteLine("You Hit!");
                    break;
                case (false,true):
                    Console.WriteLine("You Missed!");
                    return "MISS";
            }
            return "";
        }
        public string Player2Move()
        {
            int x, y;
            Console.WriteLine("To Forfeit, enter 999 into X-coordinate");
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
            Console.Write("Player 2, choose Y-coordinate:");
            if (!int.TryParse(Console.ReadLine()?.ToUpper().Trim(), out y))
            {
                y = 0;
            }
            _boardB[x, y].Bombing();
            switch (_boardB[x,y].IsShip,_boardB[x,y].IsBomb)
            {
                case (true,true):
                    Console.WriteLine("You Hit!");
                    break;
                case (false,true):
                    Console.WriteLine("You Missed!");
                    return "MISS";
            }
            return "";
        }
    }
}