using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.DAL;
using WebApp.Domain;

namespace WebApp.Pages_Pizzas
{
    public class CreateModel : PageModel
    {
        private readonly WebApp.DAL.AppDbContext _context;

        public CreateModel(WebApp.DAL.AppDbContext context)
        {
            _context = context;
        }
        public List<SelectListItem> AvailableToppings = new();
        
        public IActionResult OnGet()
        {
            foreach (var topping in _context.Toppings)
            {
                AvailableToppings.Add(new SelectListItem{Text = topping.Name, Value = topping.Name});
            }
            return Page();
        }
        
        [BindProperty]
        public Pizza Pizza { get; set; } = default!;

        [BindProperty] public List<string> SelectedTopping { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            _context.Pizzas.Add(Pizza);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Edit", new {id = Pizza.Id});
        }
    }
}
