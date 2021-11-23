using System.Threading.Tasks;
using BattleShipBrain;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
            var savedConf = JsonSerializer.Deserialize<GameConfig>(Config.ConfigStr);
            if (savedConf!.TestConf() == false)
            {
                return RedirectToPage("./CannotCreate");
            }
            _context.Configs.Add(Config);
            await _context.SaveChangesAsync();

            return RedirectToPage("./NewGame");
        }

    }
}