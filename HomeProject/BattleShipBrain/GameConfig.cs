using System.Collections.Generic;
using System.Text.Json;

namespace BattleShipBrain
{
    public class GameConfig
    {
        public int BoardSizeX { get; set; } = 10;
        public int BoardSizeY { get; set; } = 10;

        public List<ShipConfig> ShipConfigs { get; set; } = new List<ShipConfig>()
        {
            new ShipConfig()
            {
                Name = "Patrol",
                Quantity = 1,
                ShipSizeY = 1,
                ShipSizeX = 1,
            },
            new ShipConfig()
            {
                Name = "Cruiser",
                Quantity = 1,
                ShipSizeY = 1,
                ShipSizeX = 2,
            },
            new ShipConfig()
            {
                Name = "Submarine",
                Quantity = 1,
                ShipSizeY = 1,
                ShipSizeX = 3,
            },
            new ShipConfig()
            {
                Name = "Battleship",
                Quantity = 1,
                ShipSizeY = 1,
                ShipSizeX = 4,
            },
            new ShipConfig()
            {
                Name = "Carrier",
                Quantity = 1,
                ShipSizeY = 1,
                ShipSizeX = 5,
            },
        };

        public EShipTouchRule EShipTouchRule { get; set; } = EShipTouchRule.NoTouch;

        public override string ToString()
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(this, jsonOptions);
        }
    }
}