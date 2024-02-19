using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;

namespace PlanIt.Controllers
{
    [ApiController]
    [Route("api/utilisateur")]
    public class TacheController : ControllerBase
    {
        private readonly PlanItContext _context;

        public TacheController(PlanItContext context)
        {
            _context = context;
        }

        [HttpPost("{utilisateurId}/tache")]
        public IActionResult CreerTache(int utilisateurId, [FromBody] TacheDTO tacheDTO)
        {
            if (tacheDTO == null)
            {
                return BadRequest("Les données de la tâche sont incorrectes.");
            }

            // Vérifier si l'ID de l'utilisateur dans le corps de la requête correspond à celui dans l'URL de la requête
            if (utilisateurId != tacheDTO.UtilisateurId)
            {
                return BadRequest("L'ID de l'utilisateur dans le corps de la requête ne correspond pas à celui dans l'URL.");
            }

            // Vérifier si l'utilisateur avec l'ID spécifié existe dans la base de données
            var utilisateur = _context.Utilisateurs
                .Include(u => u.Taches) // Inclure les tâches de l'utilisateur pour éviter les requêtes supplémentaires
                .FirstOrDefault(u => u.UtilisateurId == utilisateurId);

            if (utilisateur == null)
            {
                return BadRequest("L'utilisateur spécifié n'existe pas.");
            }

            // Conversion de la date de chaîne en DateTimeOffset en utilisant le format désiré
            if (!DateTimeOffset.TryParseExact(tacheDTO.Deadline, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset deadline))
            {
                return BadRequest("La date de la tâche est invalide.");
            }

            // Créer votre objet Tache en utilisant les données fournies
            var nouvelleTache = new Tache
            {
                Nom = tacheDTO.Nom,
                Duree = tacheDTO.Duree,
                Deadline = deadline
            };

            // Associez la nouvelle tâche à l'utilisateur
            utilisateur.Taches.Add(nouvelleTache);

            // Ajoutez la nouvelle tâche à votre contexte de données et sauvegardez les modifications
            _context.Taches.Add(nouvelleTache);
            _context.SaveChanges();

            return Ok(nouvelleTache);
        }
    }
}
