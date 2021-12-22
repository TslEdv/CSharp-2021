using System.Collections.Generic;

namespace WebApp.Domain
{
    public class OrderPizza: BaseEntity
    {
        public int Quantity { get; set; }
        public float Price { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        
        public int PizzaId { get; set; }
        public Pizza? Pizza { get; set; }
        
        public ICollection<ExtraTopping>? ExtraToppings { get; set; }
    }
}