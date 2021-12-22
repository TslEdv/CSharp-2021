using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.DAL;
using WebApp.Domain;

namespace WebApp.Pages_Orders
{
    public class CreateModel : PageModel
    {
        private readonly WebApp.DAL.AppDbContext _context;

        public CreateModel(WebApp.DAL.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Order Order { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            Order.OrderNumber = Guid.NewGuid().ToString();
            Order.OrderStatus = OrderStatus.Ordering;
            Order.CreationTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
            
            _context.Orders.Add(Order);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Edit", new {id = Order.Id});
        }
    }
}
