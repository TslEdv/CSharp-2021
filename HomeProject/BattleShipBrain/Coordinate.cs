namespace BattleShipBrain
{
    public struct Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }

        public override string ToString() => $"X: {X}, Y: {Y}";

    }
}