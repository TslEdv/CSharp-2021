using System;
using BattleShipBrain;

namespace Domain
{
    public class Game
    {
        public int GameId { get; set; }
        
        public string GameState { get; set; } = default!;
        
        public EGameStatus Status { get; set; }

        public string CreationTime { get; set; } = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
        
        public int? ConfigId { get; set; }
        public Config? Config { get; set; }
        
        public int? ReplayId{ get; set; }
        public Replay? Replay { get; set; }

        public override string ToString()
        {
            return Status.ToString();
        }
    }
}