using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace BattleShipBrain
{
    public class BsBrain
    {
        private int _currentPlayerNo;
        private readonly GameBoard[] _gameBoards = new GameBoard[2];
        private EGameStatus _status;
        private List<ReplayTile> _gameLog;

        public BsBrain(GameConfig? config)
        {
            _status = EGameStatus.Placing;
            _gameLog = new List<ReplayTile>();
            _gameBoards[0] = new GameBoard();
            _gameBoards[1] = new GameBoard();

            _gameBoards[0].Board = new BoardSquareState[config!.BoardSizeX, config.BoardSizeY];
            _gameBoards[1].Board = new BoardSquareState[config.BoardSizeX, config.BoardSizeY];

            for (var x = 0; x < config.BoardSizeX; x++)
            {
                for (var y = 0; y < config.BoardSizeY; y++)
                {
                    _gameBoards[0].Board[x, y] = new BoardSquareState
                    {
                        IsBomb = false,
                        IsShip = false
                    };
                    _gameBoards[1].Board[x, y] = new BoardSquareState
                    {
                        IsBomb = false,
                        IsShip = false
                    };
                }
            }

            _gameBoards[0].Ships = new List<Ship>();
            _gameBoards[1].Ships = new List<Ship>();
            foreach (var shipConfig in config.ShipConfigs)
            {
                for (var i = 0; i < shipConfig.Quantity; i++)
                {
                    _gameBoards[0].Ships?.Add(new Ship()
                    {
                        Name = shipConfig.Name,
                        Length = shipConfig.ShipSizeX,
                        Height = shipConfig.ShipSizeY,
                        Coordinates = new List<Coordinate>()
                    });
                    _gameBoards[1].Ships?.Add(new Ship()
                    {
                        Name = shipConfig.Name,
                        Length = shipConfig.ShipSizeX,
                        Height = shipConfig.ShipSizeY,
                        Coordinates = new List<Coordinate>()
                    });
                }
            }

            if (config.IsRandom)
            {
                FillBoardRandom(config);
                _status = EGameStatus.Started;
            }
            else
            {
                _status = EGameStatus.Placing;
            }
        }

        public EGameStatus GetGameStatus()
        {
            var status = _status;
            return status;
        }

        public void GameSurrender()
        {
            ChangePlayer();
            _status = EGameStatus.Finished;
        }
        public bool GameFinish()
        {
            foreach (var gameBoard in _gameBoards)
            {
                var sunk = gameBoard.Ships!.Where(ship => ship.Coordinates.Count != 0).Count(ship => ship.IsShipSunk(gameBoard.Board) == "Sunk");

                if (gameBoard.Ships == null || sunk != gameBoard.Ships.Count) continue;
                _status = EGameStatus.Finished;
                return true;
            }

            return false;
        }

        public List<Ship> ListShips(int playerNo)
        {
            return _gameBoards[playerNo].Ships!.ToList();
        }

        public BoardSquareState[,] GetBoard(int playerNo)
        {
            return CopyOfBoard(_gameBoards[playerNo].Board);
        }

        public BoardSquareState[,] GetFireBoard(int playerNo)
        {
            return FireBoard(playerNo);
        }

        private BoardSquareState[,] CopyOfBoard(BoardSquareState[,] board)
        {
            var res = new BoardSquareState[board.GetLength(0), board.GetLength(1)];
            for (var x = 0; x < board.GetLength(0); x++)
            {
                for (var y = 0; y < board.GetLength(1); y++)
                {
                    {
                        res[x, y] = board[x, y];
                    }
                }
            }

            return res;
        }

        private BoardSquareState[,] FireBoard(int playerNo)
        {
            BoardSquareState[,] res = { };
            switch (playerNo)
            {
                case 0:
                    res = new BoardSquareState[_gameBoards[1].Board.GetLength(0), _gameBoards[1].Board.GetLength(1)];
                    for (var x = 0; x < _gameBoards[1].Board.GetLength(0); x++)
                    {
                        for (var y = 0; y < _gameBoards[1].Board.GetLength(1); y++)
                        {
                            {
                                if (_gameBoards[1].Board[x, y].IsShip && !_gameBoards[1].Board[x, y].IsBomb)
                                {
                                    res[x, y].IsBomb = false;
                                    res[x, y].IsShip = false;
                                }
                                else
                                {
                                    res[x, y] = _gameBoards[1].Board[x, y];
                                }
                            }
                        }
                    }

                    break;
                case 1:
                    res = new BoardSquareState[_gameBoards[0].Board.GetLength(0), _gameBoards[0].Board.GetLength(1)];
                    for (var x = 0; x < _gameBoards[0].Board.GetLength(0); x++)
                    {
                        for (var y = 0; y < _gameBoards[0].Board.GetLength(1); y++)
                        {
                            {
                                if (_gameBoards[0].Board[x, y].IsShip && !_gameBoards[0].Board[x, y].IsBomb)
                                {
                                    res[x, y].IsBomb = false;
                                    res[x, y].IsShip = false;
                                }
                                else
                                {
                                    res[x, y] = _gameBoards[0].Board[x, y];
                                }
                            }
                        }
                    }

                    break;
            }

            return res;
        }

        public int Move()
        {
            var turn = _currentPlayerNo;
            return turn;
        }
        
        public void ChangePlayer()
        {
            _currentPlayerNo = _currentPlayerNo switch
            {
                0 => 1,
                1 => 0,
                _ => _currentPlayerNo
            };
        }

        private int NextMove()
        {
            var turn = _currentPlayerNo switch
            {
                0 => 1,
                1 => 0,
                _ => 0
            };
            return turn;
        }

        public bool PlayerMove(int x, int y)
        {
            _gameBoards[NextMove()].Board[x, y].Bombing();
            _gameLog.Add(new ReplayTile
            {
                X = x,
                Y = y,
                IsBomb = _gameBoards[NextMove()].Board[x, y].IsBomb,
                IsShip = _gameBoards[NextMove()].Board[x, y].IsShip,
                Player = _currentPlayerNo,
                Placing = false
            });
            GameFinish();
            if (_status == EGameStatus.Finished)
            {
                return false;
            }
            if (_gameBoards[NextMove()].Board[x, y].IsBomb &&
                _gameBoards[NextMove()].Board[x, y].IsShip)
            {
                return false;
            }

            _currentPlayerNo = _currentPlayerNo switch
            {
                0 => 1,
                1 => 0,
                _ => _currentPlayerNo
            };
            return true;
        }

        public string Player1Move(int x, int y)
        {
            if (x > _gameBoards[1].Board.GetLength(0) - 1 || y > _gameBoards[1].Board.GetLength(1) - 1)
            {
                return "MISS";
            }

            _gameBoards[1].Board[x, y].Bombing();
            _gameLog.Add(new ReplayTile
            {
                X = x,
                Y = y,
                IsBomb = _gameBoards[NextMove()].Board[x, y].IsBomb,
                IsShip = _gameBoards[NextMove()].Board[x, y].IsShip,
                Player = _currentPlayerNo,
                Placing = false
            });
            switch (_gameBoards[1].Board[x, y].IsShip, _gameBoards[1].Board[x, y].IsBomb)
            {
                case (true, true):
                    break;
                case (false, true):
                    _currentPlayerNo = 1;
                    return "MISS";
            }

            return "";
        }

        public string Player2Move(int x, int y)
        {
            if (x > _gameBoards[0].Board.GetLength(0) - 1 || y > _gameBoards[0].Board.GetLength(1) - 1)
            {
                return "MISS";
            }

            _gameBoards[0].Board[x, y].Bombing();
            _gameLog.Add(new ReplayTile
            {
                X = x,
                Y = y,
                IsBomb = _gameBoards[NextMove()].Board[x, y].IsBomb,
                IsShip = _gameBoards[NextMove()].Board[x, y].IsShip,
                Player = _currentPlayerNo,
                Placing = false
            });
            switch (_gameBoards[0].Board[x, y].IsShip, _gameBoards[0].Board[x, y].IsBomb)
            {
                case (true, true):
                    break;
                case (false, true):
                    _currentPlayerNo = 0;
                    return "MISS";
            }

            return "";
        }

        public string GetLogJson()
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            
            return JsonSerializer.Serialize(_gameLog, jsonOptions);
        }

        public void RestoreLog(string log)
        {
           _gameLog = JsonSerializer.Deserialize<List<ReplayTile>>(log)!;
        }
        public string GetBrainJson(int playerNum)
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            var dto = new SaveGameDto
            {
                CurrentPlayerNo = playerNum,
                GameBoards =
                {
                    [0] = new SaveGameDto.GameBoardDto(),
                    [1] = new SaveGameDto.GameBoardDto()
                },
                GameStatus = _status
            };
            for (var i = 0; i < 2; i++)
            {
                dto.GameBoards[i].Board = new List<List<BoardSquareState>>();
                dto.GameBoards[i].Ships = new List<Ship>();
                foreach (var ship in _gameBoards[i].Ships!)
                {
                    dto.GameBoards[i].Ships?.Add(ship);
                }

                for (var x = 0; x < _gameBoards[i].Board.GetLength(0); x++)
                {
                    var xValues = new List<BoardSquareState>();
                    for (var y = 0; y < _gameBoards[i].Board.GetLength(1); y++)
                    {
                        xValues.Add(_gameBoards[i].Board[x, y]);
                    }

                    dto.GameBoards[i].Board?.Add(xValues);
                }
            }

            var jsonStr = JsonSerializer.Serialize(dto, jsonOptions);
            return jsonStr;
        }

        public void RestoreBrainFromJson(string json)
        {
            var restore = JsonSerializer.Deserialize<SaveGameDto>(json);
            _status = restore!.GameStatus;
            _currentPlayerNo = restore.CurrentPlayerNo;
            _gameBoards[0].Ships = restore.GameBoards[0].Ships;
            _gameBoards[1].Ships = restore.GameBoards[1].Ships;
            var xValues = restore.GameBoards[0].Board!.Count;
            var yValues = restore.GameBoards[0].Board![0].Count;
            _gameBoards[0].Board = new BoardSquareState[xValues, yValues];
            _gameBoards[1].Board = new BoardSquareState[xValues, yValues];
            var x = 0;
            var y = 0;
            for (var i = 0; i < 2; i++)
            {
                foreach (var variable in restore.GameBoards[i].Board!)
                {
                    foreach (var squareState in variable)
                    {
                        _gameBoards[i].Board[x, y] = squareState;
                        y++;
                    }

                    y = 0;
                    x++;
                }

                x = 0;
                y = 0;
            }
        }

        private void FillBoardRandom(GameConfig config)
        {
            Random rand = new(Guid.NewGuid().GetHashCode());
            for (var p = 0; p < 2; p++)
            {
                foreach (var ship in _gameBoards[p].Ships!)
                {
                    var isOpen = true;
                    while (isOpen)
                    {
                        var startColumn = rand.Next(0, _gameBoards[0].Board.GetLength(0) - 1);
                        var startRow = rand.Next(0, _gameBoards[0].Board.GetLength(1) - 1);
                        int endRow = startRow, endColumn = startColumn;
                        var orientation = rand.Next(1, 101) % 2; //0 for Horizontal

                        if (orientation == 0)
                        {
                            for (var i = 0; i < ship.Length - 1; i++)
                            {
                                endRow++;
                            }

                            for (var i = 0; i < ship.Height - 1; i++)
                            {
                                endColumn++;
                            }
                        }
                        else
                        {
                            for (var i = 0; i < ship.Length - 1; i++)
                            {
                                endColumn++;
                            }

                            for (var i = 0; i < ship.Height - 1; i++)
                            {
                                endRow++;
                            }
                        }

                        if (endRow > _gameBoards[0].Board.GetLength(1) - 1 - 1 ||
                            endColumn > _gameBoards[0].Board.GetLength(0) - 1)
                        {
                            isOpen = true;
                            continue;
                        }

                        var count = 0;

                        for (var i = startColumn; i < endColumn + 1; i++)
                        {
                            for (var j = startRow; j < endRow + 1; j++)
                            {
                                if (!_gameBoards[p].Board[i, j].IsShip)
                                {
                                    count++;
                                }
                            }
                        }

                        if (count == ship.Height * ship.Length)
                        {
                            if (PlaceShips(startColumn, endColumn, startRow, endRow, ship, config.EShipTouchRule) == false)
                            {
                                continue;
                            }
                        }

                        else
                        {
                            isOpen = true;
                            continue;
                        }

                        isOpen = false;
                    }
                }
                ChangePlayer();
            }
        }

        public bool PlaceShips(int x, int xEnd, int y, int yEnd, Ship ship, EShipTouchRule rule)
        {
            List<Coordinate> testCoordinates = new ();
            for (var i = x; i < xEnd + 1; i++)
            {
                for (var j = y; j < yEnd + 1; j++)
                {
                    foreach (var shipTest in _gameBoards[_currentPlayerNo].Ships!)
                    {
                        if (shipTest.Coordinates.Contains(new Coordinate(i, j)))
                        {
                            return false;
                        }

                        if (testCoordinates.Contains(new Coordinate(i, j))) continue;
                        if (i < _gameBoards[0].Board.GetLength(0) && j < _gameBoards[0].Board.GetLength(1))
                        {
                            testCoordinates.Add(new Coordinate(i, j));
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            if (testCoordinates.Count != ship.Height * ship.Length)
            {
                return false;
            }

            switch (rule)
            {
                case EShipTouchRule.SideTouch:
                    break;
                case EShipTouchRule.NoTouch:
                    foreach (var coordinate in testCoordinates)
                    {
                        for (var i = -1; i < 2; i++)
                        {
                            for (var j = -1; j < 2; j++)
                            {
                                if (coordinate.X + i < 0 || coordinate.X + i >= _gameBoards[0].Board.GetLength(0) ||
                                    coordinate.Y + j < 0 || coordinate.Y + j >= _gameBoards[0].Board.GetLength(1)) continue;
                                if (_gameBoards[_currentPlayerNo].Board[coordinate.X + i, coordinate.Y + j].IsShip)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    break;
                case EShipTouchRule.CornerTouch:
                    foreach (var coordinate in testCoordinates)
                    {
                        for (var i = -1; i < 2; i++)
                        {
                            for (var j = -1; j < 2; j++)
                            {
                                if (coordinate.X + i < 0 || coordinate.X + i >= _gameBoards[0].Board.GetLength(0) ||
                                    coordinate.Y + j < 0 || coordinate.Y + j >= _gameBoards[0].Board.GetLength(1)) continue;
                                if (_gameBoards[_currentPlayerNo].Board[coordinate.X, coordinate.Y + j].IsShip)
                                {
                                    return false;
                                }
                                if (_gameBoards[_currentPlayerNo].Board[coordinate.X + i, coordinate.Y].IsShip)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rule), rule, null);
            }

            for (var i = x; i < xEnd + 1; i++)
            {
                for (var j = y; j < yEnd + 1; j++)
                {
                    foreach (var shipNow in _gameBoards[_currentPlayerNo].Ships!.Where(shipNow => shipNow.Equals(ship)))
                    {
                        shipNow.Coordinates = testCoordinates;
                        _gameBoards[_currentPlayerNo].Board[i, j].IsShip = true;
                    }
                }
            }

            foreach (var logTile in testCoordinates.Select(coordinate => new ReplayTile
            {
                Player = _currentPlayerNo,
                IsShip = true,
                IsBomb = false,
                X = coordinate.X,
                Y = coordinate.Y,
                Placing = true
            }))
            {
                _gameLog.Add(logTile);
            }
            
            return true;
        }

        public void StartGame()
        {
            if (_gameBoards.Any(gameBoard => gameBoard.Ships!.Any(ship => ship.Coordinates.Count == 0)))
            {
                return;
            }
            _status = EGameStatus.Started;
        }
    }
}