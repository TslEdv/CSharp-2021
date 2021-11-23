using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
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
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            var datafile =  @"C:\Users\User\Desktop\C#\HomeProject\BattleShipsConsoleApp"+ Path.DirectorySeparatorChar + "Configs" + Path.DirectorySeparatorChar + "standard.json";
            var confJsonStr = JsonSerializer.Serialize(Config.ConfigStr, jsonOptions);
            await System.IO.File.WriteAllTextAsync(datafile, confJsonStr);

            return RedirectToPage("/NewGame");
        }
    }
}