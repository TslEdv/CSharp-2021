using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages
{
    public class MissMove : PageModel
    {

        public int Gameid { get; set; } = default!;

        public void OnGet(int id)
        {
            Gameid = id;
        }
    }
}