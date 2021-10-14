using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace BattleShipBrain
{
    public class BsBrain
    {
        private int _currentPlayerNo = 0;
        private GameBoard[] GameBoards = new GameBoard[2];


        private readonly Random _rnd = new Random();

        public BsBrain(GameConfig? config)
        {
            GameBoards[0] = new GameBoard();
            GameBoards[1] = new GameBoard();
            
            GameBoards[0].Board = new BoardSquareState[config!.BoardSizeX, config.BoardSizeY];
            GameBoards[1].Board = new BoardSquareState[config!.BoardSizeX, config.BoardSizeY];

            for (var x = 0; x < config.BoardSizeX; x++)
            {
                for (var y = 0; y < config.BoardSizeY; y++)
                {
                    GameBoards[0].Board[x, y] = new BoardSquareState
                    {
                        IsBomb = _rnd.Next(0,2) != 0,
                        IsShip = _rnd.Next(0,2) != 0
                    };
                    GameBoards[1].Board[x, y] = new BoardSquareState
                    {
                        IsBomb = _rnd.Next(0,2) != 0,
                        IsShip = _rnd.Next(0,2) != 0
                    };
                }
            }
        }

        public BoardSquareState[,] GetBoard(int playerNo)
        {
            return CopyOfBoard(GameBoards[playerNo].Board);
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

        public int Move()
        {
            var turn = this._currentPlayerNo;
            return turn;
        }
        public string Player1Move(int x, int y)
        {
            if (x > GameBoards[0].Board.GetLength(0) - 1 || y > GameBoards[0].Board.GetLength(1) - 1)
            {
                return "MISS";
            }
            GameBoards[0].Board[x, y].Bombing();
            switch ( GameBoards[0].Board[x,y].IsShip, GameBoards[0].Board[x,y].IsBomb)
            {
                case (true,true):
                    break;
                case (false,true):
                    this._currentPlayerNo = 1;
                    return "MISS";
            }
            
            return "";
        }
        public string Player2Move(int x, int y)
        {
            if (x > GameBoards[1].Board.GetLength(0) - 1 || y > GameBoards[1].Board.GetLength(1) - 1)
            {
                return "MISS";
            }
            GameBoards[1].Board[x, y].Bombing();
            switch ( GameBoards[1].Board[x,y].IsShip, GameBoards[1].Board[x,y].IsBomb)
            {
                case (true,true):
                    break;
                case (false,true):
                    this._currentPlayerNo = 0;
                    return "MISS";
            }
            
            return "";
        }
        public string GetBrainJson(int playerNum)
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            var dto = new SaveGameDTO();
            dto.CurrentPlayerNo = playerNum;
            dto.GameBoards[0] = new SaveGameDTO.GameBoardDTO();
            dto.GameBoards[1] = new SaveGameDTO.GameBoardDTO();
            for (var i = 0; i < 2; i++)
            {
                dto.GameBoards[i].Board = new List<List<BoardSquareState>>();
                dto.GameBoards[i].Ships = GameBoards[i].Ships;
                for (var x = 0; x < GameBoards[i].Board.GetLength(0); x++)
                {
                    var xvalues = new List<BoardSquareState>();
                    for (var y = 0; y < GameBoards[i].Board.GetLength(1); y++)
                    {
                        xvalues.Add(GameBoards[i].Board[x, y]);
                    }
                    dto.GameBoards[i].Board?.Add(xvalues);
                }
            }
            var jsonStr = JsonSerializer.Serialize(dto, jsonOptions);
            return jsonStr;
        }

        public void RestoreBrainFromJson(string json)
        {
            var restore = JsonSerializer.Deserialize<SaveGameDTO>(json);
            this._currentPlayerNo = restore!.CurrentPlayerNo;
            this.GameBoards[0].Ships = restore.GameBoards[0].Ships;
            this.GameBoards[1].Ships = restore.GameBoards[1].Ships;
            var xvalues = restore.GameBoards[0].Board!.Count;
            var yvalues = restore.GameBoards[0].Board![0].Count;
            this.GameBoards[0].Board = new BoardSquareState[xvalues, yvalues];
            this.GameBoards[1].Board = new BoardSquareState[xvalues, yvalues];
            var x = 0;
            var y = 0;
            for (var i = 0; i < 2; i++)
            {
                foreach (var variable in restore.GameBoards[i].Board!)
                {
                    foreach (var squareState in variable)
                    {
                        this.GameBoards[i].Board[x, y] = squareState;
                        y++;
                    }

                    y = 0;
                    x++;
                }

                x = 0;
                y = 0;
            }
        }
    }
}