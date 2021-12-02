using System.Collections.Generic;

namespace Domain
{
    public class Ship
    {
        public int ShipId { get; set; }
        
        public string Name { get; set; } = default!;
        
        public int ShipLength { get; set; }
        
        public int ShipHeight { get; set; }

        public ICollection<ConfigShip>? ConfigShips { get; set; }
    }
}