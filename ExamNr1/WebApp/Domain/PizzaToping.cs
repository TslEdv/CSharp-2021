namespace WebApp.Domain
{
    public class PizzaToping: BaseEntity
    {
        public int PizzaId { get; set; }
        public Pizza? Pizza { get; set; }

        public int ToppingId { get; set; }
        public Topping? Topping { get; set; }
    }
}