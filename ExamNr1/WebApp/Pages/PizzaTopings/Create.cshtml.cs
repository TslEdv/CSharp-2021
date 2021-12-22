using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.DAL;
using WebApp.Domain;

namespace WebApp.Pages_PizzaTopings
{
    public class CreateModel : PageModel
    {
        private readonly WebApp.DAL.AppDbContext _context;

        public CreateModel(WebApp.DAL.AppDbContext context)
        {
            _context = context;
        }

        public SelectList Toppings = default!;
        public int Id;
        public IActionResult OnGet(int id)
        {
            Id = id;
            Toppings = new SelectList(_context.Toppings, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public PizzaToping PizzaToping { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.PizzaTopings.Add(PizzaToping);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Pizzas/Edit", new {id = PizzaToping.PizzaId});
        }
    }
}
