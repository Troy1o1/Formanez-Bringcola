using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Formanez_Bringcola.Pages
{
    public class HomePageModel : PageModel
    {
        public class HomeModel : PageModel
        {
            private readonly ILogger<HomeModel> _logger;

            public HomeModel(ILogger<HomeModel> logger)
            {
                _logger = logger;
            }
            public void OnGet()
            {
            }
        }
    }
}
