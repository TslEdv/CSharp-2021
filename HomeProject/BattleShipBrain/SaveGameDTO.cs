using System.Collections.Generic;

namespace BattleShipBrain
{
    public class SaveGameDTO
    {
        public int CurrentPlayerNo { get; set; } = 0;
        public GameBoardDTO[] GameBoards  { get; set; } = new GameBoardDTO[2];
        
        public class GameBoardDTO
        {
            public List<List<BoardSquareState>>? Board { get; set; }
            public List<Ship>? Ships { get; set; }
        }

    }
}