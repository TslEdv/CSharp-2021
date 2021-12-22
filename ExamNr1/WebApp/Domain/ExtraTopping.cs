namespace WebApp.Domain
{
    public class ExtraTopping: BaseEntity
    {
        public int OrderPizzaId { get; set; }
        public OrderPizza? OrderPizza { get; set; }

        public int ToppingId { get; set; }
        public Topping? Topping { get; set; }

    }
}