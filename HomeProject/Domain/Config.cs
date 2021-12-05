using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BattleShipBrain;

namespace Domain
{
    public class Config
    {
        public int ConfigId { get; set; }
        
        [MaxLength(128)]
        public string? ConfigName { get; set; }
        
        public bool IsRandom { get; set; }

        public int BoardSizeX { get; set; }
        
        public int BoardSizeY { get; set; }
        
        public EShipTouchRule TouchRule { get; set; }

        public string ConfigStr { get; set; } = default!;
        
        public string CreationTime { get; set; } = DateTime.Now.ToString("MM/dd/yyyy HH:mm");

        public ICollection<ConfigShip>? ConfigShips { get; set; }
    }
}