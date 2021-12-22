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

namespace WebApp.Pages_ExtraToppings
{
    public class CreateModel : PageModel
    {
        private readonly WebApp.DAL.AppDbContext _context;

        public CreateModel(WebApp.DAL.AppDbContext context)
        {
            _context = context;
        }
        
        public int Id;
        public SelectList Toppings = default!;
        public IActionResult OnGet(int id)
        {
            Id = id;
            Toppings = new SelectList(_context.Toppings, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public ExtraTopping ExtraTopping { get; set; } = default!;
        
        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _context.ExtraToppings.Add(ExtraTopping);
            await _context.SaveChangesAsync();
            var routeback = _context.ExtraToppings.Include(top => top.OrderPizza);
            var waybackid = 0;
            var quantity = 0;
            foreach (var route in routeback)
            {
                if (route.Id == ExtraTopping.Id)
                {
                    waybackid = route.OrderPizza!.OrderId;
                    quantity = route.OrderPizza.Quantity;
                }
            }
            (await _context.OrderPizzas.FindAsync(ExtraTopping.OrderPizzaId)).Price += 1 * quantity;
            await _context.SaveChangesAsync();
            return RedirectToPage("/Orders/Edit", new {id = waybackid});
        }
    }
}
