using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BattleShipBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL;
using Domain;

namespace WebApp.Pages_Configs
{
    public class CreateModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public CreateModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Config Config { get; set; }  = default!;

        public IActionResult OnGet()
        {
            return Page();
        }
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            var conf = new GameConfig
            {
                BoardSizeX = Config.BoardSizeX,
                BoardSizeY = Config.BoardSizeY,
                EShipTouchRule = Config.TouchRule,
                IsRandom = Config.IsRandom,
                Name = Config.ConfigName,
                ShipConfigs = new List<ShipConfig>()
            };
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            Config.ConfigStr = JsonSerializer.Serialize(conf, jsonOptions);
            _context.Configs.Add(Config);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
