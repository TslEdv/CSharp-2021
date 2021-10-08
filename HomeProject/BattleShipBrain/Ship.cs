using System.Collections.Generic;
using System.Linq;

namespace BattleShipBrain
{
    public class Ship
    {
        public string Name { get; private set; } 
        
        private readonly List<Coordinate> _coordinates = new List<Coordinate>();

        public Ship(string name, Coordinate position, int length, int height)
        {
            Name = name;
            for (var x = 0; x < position.X + length; x++)
            {
                for (var y = 0; y < position.Y + height; y++)
                {
                    _coordinates.Add(new Coordinate(){X = x, Y = y});
                }
            }
        }

        public int GetShipSize() => _coordinates.Count;
        
        public int GetShipDamageCount(BoardSquareState[,] board) =>
            // count all the items that match the predicate
            _coordinates.Count(coordinate => board[coordinate.X, coordinate.Y].IsBomb);

        public bool IsShipSunk(BoardSquareState[,] board) =>
            // returns true when all the items in the list match predicate
            _coordinates.All(coordinate => board[coordinate.X, coordinate.Y].IsBomb);

    }
}