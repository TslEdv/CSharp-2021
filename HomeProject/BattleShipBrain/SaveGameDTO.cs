using System.Collections.Generic;

namespace BattleShipBrain
{
    public class SaveGameDto
    {
        public int CurrentPlayerNo { get; set; }
        
        public EGameStatus GameStatus { get; set; }
        public GameBoardDto[] GameBoards  { get; set; } = new GameBoardDto[2];
        
        public class GameBoardDto
        {
            public List<List<BoardSquareState>>? Board { get; set; }
            public List<Ship>? Ships { get; set; }

        }

    }
}