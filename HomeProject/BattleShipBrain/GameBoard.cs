using System.Collections.Generic;

namespace BattleShipBrain
{
    public class GameBoard
    {
        public BoardSquareState[,] Board { get; set; } = null!;
        public List<Ship>? Ships { get; set; }
    }
}