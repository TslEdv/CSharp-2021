namespace Domain
{
    public class ConfigShip
    {
        public int ConfigShipId { get; set; }
        public int Quantity { get; set; }
        
        public int ConfigId { get; set; }
        public Config? Config { get; set; }
        
        public int ShipId { get; set; }
        public Ship? Ship { get; set; }

    }
}