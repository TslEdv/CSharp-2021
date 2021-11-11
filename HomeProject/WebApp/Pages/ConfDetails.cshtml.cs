using System.Threading.Tasks;
using BattleShipBrain;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages
{
    public class ConfDetails : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public ConfDetails(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public Config GameConfig { get; set; } = new Config();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (id == 0)
            {
                GameConfig.ConfigStr = new GameConfig().ToString();
                GameConfig.ConfigId = 0;
                return Page();
            }
            else
            {
                GameConfig = await _context.Configs.FirstOrDefaultAsync(m => m.ConfigId == id);

                if (GameConfig == null)
                {
                    return NotFound();
                }
                return Page();
            }
        }

    }
}