using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace BattleShipBrain
{
    public class GameConfig
    {
        public int BoardSizeX { get; set; } = 10;
        public int BoardSizeY { get; set; } = 10;
        public bool IsRandom { get; set; } = true;

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

        public bool TestConf()
        {
            int countSize;
            switch (EShipTouchRule)
            {
                case EShipTouchRule.SideTouch:
                    countSize = ShipConfigs.Sum(ship => ship.Quantity * ship.ShipSizeX * ship.ShipSizeY);
                    return countSize <= BoardSizeX * BoardSizeY;
                case EShipTouchRule.NoTouch:
                    countSize = ShipConfigs.Sum(ship => ship.Quantity * (ship.ShipSizeX * ship.ShipSizeY + 2 * ship.ShipSizeX + 2 * ship.ShipSizeY + 4));
                    return countSize <= BoardSizeX * BoardSizeY;
                case EShipTouchRule.CornerTouch:
                    countSize = ShipConfigs.Sum(ship => ship.Quantity * (ship.ShipSizeX * ship.ShipSizeY + 2 * ship.ShipSizeX + 2 * ship.ShipSizeY));
                    return countSize <= BoardSizeX * BoardSizeY;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}