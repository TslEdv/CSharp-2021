using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.DAL;
using WebApp.Domain;

namespace WebApp.Pages_Orders
{
    public class IndexModel : PageModel
    {
        private readonly WebApp.DAL.AppDbContext _context;

        public IndexModel(WebApp.DAL.AppDbContext context)
        {
            _context = context;
        }

        public IList<Order> Order { get;set; } = default!;

        public string? SearchName { get; set; }
        public string? SearchCode { get; set; }
        public string? SearchDate { get; set; }

        
        public async Task OnGetAsync(string? searchName, string? searchCode,string? searchDate, string action)
        {
            if (action == "Clear")
            {
                searchName = null;
                searchCode = null;
                searchDate = null;
            }
            SearchName = searchName;
            SearchCode = searchCode;
            SearchDate = searchDate;
            var query = _context.Orders.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchName))
            {
                searchName = searchName.Trim();
                query = query.Where(r => 
                    r.ClientName!.ToUpper().Contains(searchName.ToUpper()));
                Order = await query.ToListAsync();
            }

            if (!string.IsNullOrWhiteSpace(searchCode))
            {
                searchCode = searchCode.Trim();
                query = query.Where(r => 
                    r.OrderNumber.ToUpper().Contains(searchCode.ToUpper()));
                Order = await query.ToListAsync();
            }
            if (!string.IsNullOrWhiteSpace(searchDate))
            {
                searchDate = searchDate.Trim();
                query = query.Where(r => 
                    r.CreationTime.ToUpper().Contains(searchDate.ToUpper()));
                Order = await query.ToListAsync();
            }

            else
            {
                Order = await query.ToListAsync();
            }


        }
    }
}
