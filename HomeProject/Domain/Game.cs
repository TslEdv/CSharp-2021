using System;
using System.Collections.Generic;
using BattleShipBrain;

namespace Domain
{
    public class Game
    {
        public int GameId { get; set; }
        
        public string GameState { get; set; } = default!;
    }
}