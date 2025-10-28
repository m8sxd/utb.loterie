using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace utb.loterie.Pages
{
    // Tato tøída slouží k inicializaci dat pro úvodní stránku (zatím prázdná)
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            // Zde by mohla být logika pro naèítání napø. Top 3 eventù nebo aktuálního Jackpotu.
            // Pro zaèátek je to jen prázdná metoda.
            _logger.LogInformation("Úvodní stránka naètena.");
        }
    }
}