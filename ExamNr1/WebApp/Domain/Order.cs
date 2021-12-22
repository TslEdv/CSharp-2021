using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Domain
{
    public class Order: BaseEntity
    {
        [MaxLength(512)]
        public string OrderNumber { get; set; } = default!;
        [MaxLength(128)]
        public string? ClientName { get; set; }
        public string CreationTime { get; set; } = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
        public OrderStatus OrderStatus { get; set; }
        public float Price { get; set; }
        
        public ICollection<OrderPizza>? OrderPizzas { get; set; }

    }
}