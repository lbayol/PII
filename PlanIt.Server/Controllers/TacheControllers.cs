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
        // Contexte de base de données
        private readonly PlanItContext _context;

        // Constructeur

        public TacheController(PlanItContext context)
        {
            _context = context;
        }

        // Méthode permettant de créer une tâche pour un utilisateur

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

        // Méthode pour rafraîchir le nombre d'heure réalisée d'une tâche

        [HttpPut("RafraichirHeuresRealisees")]
        public IActionResult RafraichirHeuresRealisees(int idUtilisateur, int idTache, int idTodo)
        {
            // Récupérer l'utilisateur correspondant à l'idUtilisateur à l'aide de la méthode FirstOrDefault de LINQ, en incluant ses todos, ses disponibilités et ses tâches
            var utilisateur = _context.Utilisateurs
            .Include(u => u.Todos)
            .Include(u => u.Disponibilites)
            .Include(u => u.Taches)
            .FirstOrDefault(u => u.UtilisateurId == idUtilisateur);

            // Vérifier si l'utilisateur existe, sinon renvoie une erreur.
            if (utilisateur == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            // Parcourir toutes les todos de l'utilisateur et vérifier si l'ID de la todo correspond à l'idTodo fourni en paramètre
            foreach (var todo in utilisateur.Todos)
            {
                if (todo.TodoId == idTodo)
                {
                    // Parcourir toutes les tâches de l'utilisateur et vérifier si le nom de la tâche correspond au nom de la todo
                    foreach (var tache in utilisateur.Taches)
                    {
                        if (tache.Nom == todo.Nom)
                        {
                            // Mettre à jour le nombre d'heures réalisées pour cette tâche en ajoutant la durée de la todo
                            tache.NombreHeuresRealisees += todo.Duree;
                            // Si le nombre d'heures réalisées est égal à la durée totale de la tâche, marquer la tâche comme réalisée
                            if (tache.NombreHeuresRealisees == tache.Duree)
                            {
                                tache.Realisation = true;
                            }
                        }
                    }
                }
            }

            _context.SaveChanges();

            // Renvoie un message de succès.
            return Ok("Les ratés de l'utilisateur ont été mis à jour.");
        }
    }
}
