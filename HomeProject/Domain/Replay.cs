using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BattleShipBrain;

namespace Domain
{
    public class Replay
    {
        public int ReplayId { get; set; }

        public string? Replays { get; set; }
        
    }
}