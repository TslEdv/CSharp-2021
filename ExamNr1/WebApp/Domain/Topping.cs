using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Domain
{
    public class Topping: BaseEntity
    {
        [MaxLength(128)]
        public string Name { get; set; } = default!;
        
        public ICollection<ExtraTopping>? ExtraToppings { get; set; }
        public ICollection<Pizza>? Pizzas { get; set; }
    }
}