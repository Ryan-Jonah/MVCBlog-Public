using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVCBlog.Data;
using MVCBlog.Models;

namespace MVCBlog.Views.Shared.Components.Learning
{
    public class FooterTagsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public FooterTagsViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync() //Gather parameters here
        {
            var items = await GetItemsAsync();

            return View(items);
        }
        private Task<List<Tag>> GetItemsAsync()
        {
            return _context.Tags.AsQueryable().ToListAsync();
        }
    }
}