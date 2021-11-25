namespace BattleShipBrain
{
    public struct ReplayTile
    {
        public int Player { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        
        public bool Placing { get; set; }
        
        public bool IsShip { get; set; }
        public bool IsBomb { get; set; }

        public ReplayTile(int x, int y, bool placing ,bool ship, bool bomb, int player)
        {
            X = x;
            Y = y;
            Placing = placing;
            IsShip = ship;
            IsBomb = bomb;
            Player = player;
        }
    }
}