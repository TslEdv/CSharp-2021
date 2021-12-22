using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages
{
    public class Statistics : PageModel
    {
        private readonly WebApp.DAL.AppDbContext _context;

        public Statistics(WebApp.DAL.AppDbContext context)
        {
            _context = context;
        }

        public float Revenue;
        public int SoldPizzas;
        public int Ordering;
        public int Waiting;
        public int Satisfied;
        public string MostPopular = default!;

        public void OnGet()
        {
            var pizzas = new List<string>();
            foreach (var ordered in _context.Orders.Where(o => o.OrderStatus == OrderStatus.Done || o.OrderStatus == OrderStatus.Making)
                .Include(o => o.OrderPizzas).ThenInclude(op => op.Pizza))
            {
                Revenue += ordered.Price;
                foreach (var ordpiz in ordered.OrderPizzas!)
                {
                    for (var i = 0; i < ordpiz.Quantity; i++)
                    {
                        pizzas.Add(ordpiz.Pizza!.Name);
                        SoldPizzas++;
                    }
                }
            }

            foreach (var unused in _context.Orders.Where(o=> o.OrderStatus == OrderStatus.Ordering))
            {
                Ordering++;
            }
            
            foreach (var unused in _context.Orders.Where(o=> o.OrderStatus == OrderStatus.Making))
            {
                Waiting++;
            }
            foreach (var unused in _context.Orders.Where(o=> o.OrderStatus == OrderStatus.Done))
            {
                Satisfied++;
            }
            MostPopular = pizzas.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).FirstOrDefault(x => x != null)!;
            {
                
            }

        }
    }
}