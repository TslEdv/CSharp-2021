using System.Collections.Generic;
using System.Linq;

namespace BattleShipBrain
{
    public class Ship
    {
        public string? Name { get; set; } 
        public int Length { get; set; }
        public int Height { get; set; }
        public List<Coordinate> Coordinates { get; set; } = default!;

        public int GetShipSize() => Coordinates.Count;
        
        public int GetShipDamageCount(BoardSquareState[,] board) =>
            // count all the items that match the predicate
            Coordinates.Count(coordinate => board[coordinate.X, coordinate.Y].IsBomb);

        public string IsShipSunk(BoardSquareState[,] board)
        {
            return Coordinates.All(coordinate => board[coordinate.X, coordinate.Y].IsBomb) switch
            {
                true => "Sunk",
                false => "Alive"
            };
        }
    }
}