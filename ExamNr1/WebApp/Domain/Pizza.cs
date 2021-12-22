using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Domain
{
    public class Pizza: BaseEntity
    {
        [MaxLength(128)]
        public string Name { get; set; } = default!;
        [MaxLength(256)]
        public string Description { get; set; } = default!;
        public bool Spicy { get; set; }
        public bool Vegan { get; set; }
        public float Price { get; set; }
        
        public ICollection<PizzaToping>? PizzaTopings { get; set; }
        public ICollection<OrderPizza>? OrderPizzas { get; set; }
    }
}