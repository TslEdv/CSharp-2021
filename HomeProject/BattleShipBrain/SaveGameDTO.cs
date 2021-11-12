using System.Collections.Generic;

namespace BattleShipBrain
{
    public class SaveGameDto
    {
        public int CurrentPlayerNo { get; set; } = 0;
        public GameBoardDto[] GameBoards  { get; set; } = new GameBoardDto[2];
        
        public class GameBoardDto
        {
            public List<List<BoardSquareState>>? Board { get; set; }
            public List<Ship>? Ships { get; set; }

        }

    }
}