using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.DAL;
using WebApp.Domain;

namespace WebApp.Pages_OrderPizzas
{
    public class CreateModel : PageModel
    {
        private readonly WebApp.DAL.AppDbContext _context;

        public CreateModel(WebApp.DAL.AppDbContext context)
        {
            _context = context;
        }

        public string? SearchName { get; set; }
        public string? SearchTop { get; set; }
        public string? Spicy { get; set; }
        public string? Vegan { get; set; }
        public int Id;
        public SelectList Pizzas = default!;
        public List<Pizza> ListOfPizzas = new List<Pizza>();
        public async Task OnGetAsync(int id, string? searchName, string? searchTop,string? spicy, string? vegan, string action)
        {
            if (action == "Clear")
            {
                searchName = null;
                searchTop = null;
                spicy = null;
                vegan = null;
                Id = id;
            }
            SearchName = searchName;
            SearchTop = searchTop;
            Spicy = spicy;
            Vegan = vegan;
            Id = id;
            Pizzas = new SelectList(_context.Pizzas, "Id", "Name");
            var query = _context.Pizzas
                .Include(p => p.PizzaTopings)
                .ThenInclude(t => t.Topping).AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchName))
            {
                searchName = searchName.Trim();
                query = query.Where(p => 
                    p.Name.ToUpper().Contains(searchName.ToUpper()) ||
                    p.Description.ToUpper().Contains(searchName.ToUpper()));
                ListOfPizzas = await query.ToListAsync();
            }
            if (!string.IsNullOrWhiteSpace(searchTop))
            {
                searchTop = searchTop.Trim();
                query = query.Where(p => 
                    p.PizzaTopings!.Any(t => t.Topping!.Name.ToUpper().Contains(searchTop.ToUpper())));
                ListOfPizzas = await query.ToListAsync();
            }
            if (!string.IsNullOrWhiteSpace(spicy))
            {
                query = query.Where(p =>
                    p.Spicy.Equals(true));
                ListOfPizzas = await query.ToListAsync();
            }

            if (!string.IsNullOrWhiteSpace(vegan))
            {
                query = query.Where(p =>
                    p.Vegan.Equals(true));
                ListOfPizzas = await query.ToListAsync();
            }
            else
            {
                ListOfPizzas = await query.ToListAsync();
            }
        }

        [BindProperty]
        public OrderPizza OrderPizza { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            OrderPizza.Price = (await _context.Pizzas.FindAsync(OrderPizza.PizzaId)).Price * OrderPizza.Quantity;
            (await _context.Orders.FindAsync(OrderPizza.OrderId)).Price += OrderPizza.Price;
            _context.OrderPizzas.Add(OrderPizza);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Orders/Edit", new {id = OrderPizza.OrderId});
        }
    }
}
