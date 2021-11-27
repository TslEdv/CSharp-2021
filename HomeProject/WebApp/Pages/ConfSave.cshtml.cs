using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using BattleShipBrain;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages
{
    public class ConfSave : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public ConfSave(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Config Config { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Config = await _context.Configs.FirstOrDefaultAsync(m => m.ConfigId== id);

            if (Config == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Config = await _context.Configs.FindAsync(id);


            if (Config == null) return RedirectToPage("/NewGame");
            var datafile =  @"C:\Users\User\Desktop\C#\HomeProject\BattleShipsConsoleApp"+ Path.DirectorySeparatorChar + "Configs" + Path.DirectorySeparatorChar + "standard.json";
            var savedConf = JsonSerializer.Deserialize<GameConfig>(Config.ConfigStr);
            await System.IO.File.WriteAllTextAsync(datafile, savedConf!.ToString());

            return RedirectToPage("/NewGame");
        }
    }
}