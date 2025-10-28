using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace utb.loterie.Pages
{
    // Tato t��da slou�� k inicializaci dat pro �vodn� str�nku (zat�m pr�zdn�)
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            // Zde by mohla b�t logika pro na��t�n� nap�. Top 3 event� nebo aktu�ln�ho Jackpotu.
            // Pro za��tek je to jen pr�zdn� metoda.
            _logger.LogInformation("�vodn� str�nka na�tena.");
        }
    }
}