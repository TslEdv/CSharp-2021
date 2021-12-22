using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Orders
{
    public class Done : PageModel
    {
        private readonly WebApp.DAL.AppDbContext _context;

        public Done(WebApp.DAL.AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            _context.Orders.Find(id).OrderStatus = OrderStatus.Done;
            _context.Attach(_context.Orders.Find(id)).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}