using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RemoteOS.Web.Database;

namespace RemoteOS.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public ROSContext context { get; set; }
        public MachineManager manager { get; set; }

        public IndexModel(ILogger<IndexModel> logger, ROSContext ctx, MachineManager manager)
        {
            _logger = logger;
            context = ctx;
            this.manager = manager; 
        }

        public void OnGet()
        {

        }
    }
}