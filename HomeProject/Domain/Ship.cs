using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Ship
    {
        public int ShipId { get; set; }
        
        [MaxLength(128)]
        public string Name { get; set; } = default!;
        
        public int ShipLength { get; set; }
        
        public int ShipHeight { get; set; }

        public ICollection<ConfigShip>? ConfigShips { get; set; }
    }
}