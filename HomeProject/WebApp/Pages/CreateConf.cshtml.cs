using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages
{
    public class CreateConf : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public CreateConf(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty] public Config Config { get; set; } = default!;
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Configs.Add(Config);
            await _context.SaveChangesAsync();

            return RedirectToPage("./NewGame");
        }

    }
}