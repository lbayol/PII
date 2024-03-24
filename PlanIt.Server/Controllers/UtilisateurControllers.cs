using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt;
using Microsoft.AspNetCore.Cors;

namespace PlanIt.Controllers
{
    [ApiController]
    [Route("api/utilisateur")]
    public class UtilisateurController : ControllerBase
    {
        // Contexte de base de données
        private readonly PlanItContext _context;

        // Constructeur
        public UtilisateurController(PlanItContext context)
        {
            _context = context;
        }

        // Récupérer les informations d'un utilisateur par son ID
        [HttpGet("infos/{id}")]
        public IActionResult GetUtilisateurById(int id)
        {
            // Rechercher l'utilisateur dans la base de données
            var utilisateur = _context.Utilisateurs
                                      .Include(u => u.Disponibilites)
                                      .Include(u => u.Taches)
                                      .Include(u => u.Todos)
                                      .FirstOrDefault(u => u.UtilisateurId == id);

            // Vérifier si l'utilisateur existe
            if (utilisateur == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            // Retourner les informations de l'utilisateur
            return Ok(utilisateur);
        }

        // Inscription d'un nouvel utilisateur
        [HttpPost("Inscription")]
        public IActionResult PostUtilisateur([FromBody] UtilisateurInscriptionDTO utilisateurDTOPOST)
        {
            // Vérifier si les informations de l'utilisateur sont valides
            if (utilisateurDTOPOST == null)
            {
                return BadRequest("Erreur : Veuillez remplir les informations du nouvel utilisateur.");
            }

            var utilisateurExistant = _context.Utilisateurs.FirstOrDefault(u => u.Email == utilisateurDTOPOST.Email);

            if (utilisateurExistant != null)
            {
                return BadRequest("Erreur : Un utilisateur avec cet email existe déjà.");
            }

            // Hacher le mot de passe
            string motDePasseHaché = BCrypt.Net.BCrypt.HashPassword(utilisateurDTOPOST.Password);

            // Créer un nouvel utilisateur
            Utilisateur nouvelUtilisateur = new Utilisateur
            {
                Nom = utilisateurDTOPOST.Nom,
                Prenom = utilisateurDTOPOST.Prenom,
                Email = utilisateurDTOPOST.Email,
                Password = motDePasseHaché,
            };

            // Ajouter les disponibilités par défaut à l'utilisateur (0 pour chaque jour)
            AjouterDisponibilitesParDefaut(nouvelUtilisateur);

            // Ajouter l'utilisateur à la base de données
            _context.Utilisateurs.Add(nouvelUtilisateur);
            _context.SaveChanges();

            // Retourner un message de succès
            return Ok("Inscription réussie");
        }

        // Ajouter les disponibilités par défaut à un utilisateur (0 pour chaque jour)
        private void AjouterDisponibilitesParDefaut(Utilisateur utilisateur)
        {
            for (int i = 0; i < 7; i++)
            {
                utilisateur.Disponibilites.Add(new Disponibilite { NbHeure = 0 });
            }
        }

        // Connexion d'un utilisateur
        [HttpPost("Connexion")]
        public IActionResult Connexion([FromBody] UtilisateurConnexionDTO connexionDTO)
        {
            // Rechercher l'utilisateur dans la base de données
            var utilisateur = _context.Utilisateurs.FirstOrDefault(u => u.Email == connexionDTO.Email);

            // Vérifier si l'utilisateur existe et si le mot de passe est correct
            if (utilisateur != null && BCrypt.Net.BCrypt.Verify(connexionDTO.Password, utilisateur.Password))
            {
                // Retourner un message de succès
                return Ok("Connexion réussie !");
            }

            // Retourner un message d'erreur
            return BadRequest("L'authentification a échoué.");
        }

        // Récupérer les informations d'un utilisateur par son email (pour stocker les variables dans le localStorage lors de la connexion)
        [HttpGet("infosConnexion")]
        public IActionResult GetUtilisateurByEmail([FromQuery] string email)
        {
            // Rechercher l'utilisateur dans la base de données
            var utilisateur = _context.Utilisateurs
            .Include(u => u.Disponibilites)
            .Include(u => u.Taches)
            .Include(u => u.Todos)
            .FirstOrDefault(u => u.Email == email);

            // Vérifier si l'utilisateur existe
            if (utilisateur == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            // Retourner les informations de l'utilisateur
            return Ok(utilisateur);
        }

        // Mettre à jour les disponibilités d'un utilisateur
        [HttpPut("UpdateDisponibilités/{id}")]
        public IActionResult PutDisponibilitesUtilisateur(int id, [FromBody] List<int> disponibilitesData)
        {
            // Vérifier si les données de disponibilités sont valides
            if (disponibilitesData == null)
            {
                return BadRequest("Les données de disponibilités sont manquantes.");
            }

            var utilisateur = _context.Utilisateurs.Include(u => u.Disponibilites).FirstOrDefault(u => u.UtilisateurId == id);

            if (utilisateur == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            // Vérifier si le nombre de disponibilités envoyées est correct
            if (disponibilitesData.Count != utilisateur.Disponibilites.Count)
            {
                return BadRequest("Le nombre de disponibilités envoyées est incorrect.");
            }

            // Mettre à jour les disponibilités de l'utilisateur
            for (int i = 0; i < utilisateur.Disponibilites.Count; i++)
            {
                utilisateur.Disponibilites[i].NbHeure = disponibilitesData[i];
            }

            _context.SaveChanges();

            // Retourner l'utilisateur mis à jour
            return Ok(utilisateur);
        }

        // Mettre à jour la note d'un utilisateur
        [HttpPut("UpdateNote/{id}")]
        public IActionResult UpdateUserNote(int id)
        {
            var utilisateur = _context.Utilisateurs
                                      .Include(u => u.Todos)
                                      .Include(u => u.Disponibilites)
                                      .Include(u => u.Taches)
                                      .FirstOrDefault(u => u.UtilisateurId == id);

            if (utilisateur == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            // Vérifier pour éviter une division par zéro
            if (utilisateur.NombreHeuresDisponibles == 0)
            {
                return BadRequest("Impossible de calculer la note : aucun créneau disponible.");
            }

            // Calculer la nouvelle note de l'utilisateur. Choix arbitraire de calculer la note en fonction de la todo dont la deadline est la plus proche.
            double nouvelleNote = 100 - (100.0 * utilisateur.NombreHeuresRates / utilisateur.NombreHeuresDisponibles);

            // Mettre à jour la note de l'utilisateur dans la base de données
            utilisateur.Note = (int)nouvelleNote;

            if (utilisateur.Note == 0)
            {
                utilisateur.Note = 1;
            }

            _context.SaveChanges();

            // Retourner un message de succès
            return Ok($"La note de l'utilisateur avec l'ID {id} a été mise à jour avec succès.");
        }

        // Calculer les heures disponibles d'un utilisateur
        [HttpPut("CalculerHeuresDisponibles")]
        public IActionResult CalculerHeuresDisponibles(int idUtilisateur)
        {
            var utilisateur = _context.Utilisateurs
                                      .Include(u => u.Todos)
                                      .Include(u => u.Disponibilites)
                                      .Include(u => u.Taches)
                                      .FirstOrDefault(u => u.UtilisateurId == idUtilisateur);

            if (utilisateur == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            // Calculer le nombre total d'heures de créneaux disponibles avant la prochaine tâche
            int heuresTotalesDisponibles = 0;
            var tachePlusProche = utilisateur.Taches.Where(t => t.Duree != t.NombreHeuresRealisees) // Filtrer les tâches où la durée est différente du nombre d'heures réalisées pour ne pas prendre une tâche déjà réalisée.
            .OrderBy(t => t.Deadline) // Ordonner les tâches par date limite
            .FirstOrDefault(); // Sélectionner la première tâche dans l'ordre trié

            // Trouver la todo associée à cette tâche avec la date la plus récente
            var todoAssociee = utilisateur.Todos.Where(t => t.Nom == tachePlusProche.Nom)
                                                .OrderByDescending(t => t.Date)
                                                .FirstOrDefault();

            // Vérifier si une todo associée à cette tâche existe et si sa date est dans le futur
            if (todoAssociee != null && todoAssociee.Date >= DateTimeOffset.Now && todoAssociee.Date <= tachePlusProche.Deadline)
            {
                // Parcourir chaque jour à partir de la date de la todo associée jusqu'à la deadline de la tâche
                for (DateTimeOffset date = todoAssociee.Date.AddDays(1); date <= tachePlusProche.Deadline; date = date.AddDays(1))
                {
                    // Trouver l'index du jour de la semaine, où 0 correspond à Lundi, 1 à Mardi, etc.
                    int indexJour = ((int)date.DayOfWeek + 6) % 7;
                    heuresTotalesDisponibles += utilisateur.Disponibilites[indexJour].NbHeure;
                }
            }

            utilisateur.NombreHeuresDisponibles = heuresTotalesDisponibles;
            _context.SaveChanges();

            // Retourner un message de succès
            return Ok("Le nombre d'heures disponibles de l'utilisateur a été mis à jour.");
        }

        // Supprimer toutes les tâches d'un utilisateur
        [HttpDelete("{idUtilisateur}/taches")]
        public IActionResult DeleteTachesUtilisateur(int idUtilisateur)
        {
            var utilisateur = _context.Utilisateurs.Include(u => u.Taches).FirstOrDefault(u => u.UtilisateurId == idUtilisateur);

            if (utilisateur == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            // Supprimer toutes les tâches de l'utilisateur de la base de données
            _context.Taches.RemoveRange(utilisateur.Taches);
            _context.SaveChanges();

            // Retourner un message de succès
            return Ok("Toutes les tâches de l'utilisateur ont été supprimées avec succès.");
        }

        // Rafraîchir la note et les ratés d'un utilisateur
        [HttpPut("RafraichirNoteRates")]
        public IActionResult RafraichirNoteRates(int idUtilisateur)
        {
            var utilisateur = _context.Utilisateurs
                                      .Include(u => u.Todos)
                                      .Include(u => u.Disponibilites)
                                      .Include(u => u.Taches)
                                      .FirstOrDefault(u => u.UtilisateurId == idUtilisateur);

            if (utilisateur == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            utilisateur.NombreHeuresRates = 0;
            utilisateur.Note = 100;

            _context.SaveChanges();

            // Retourner un message de succès
            return Ok("Les ratés et la note de l'utilisateur ont été mis à jour.");
        }

        // Rafraîchir les ratés d'un utilisateur pour une todo spécifique
        [HttpPut("RafraichirRates")]
        public IActionResult RafraichirRates(int idUtilisateur, int idTodo)
        {
            var utilisateur = _context.Utilisateurs
                                      .Include(u => u.Todos)
                                      .Include(u => u.Disponibilites)
                                      .Include(u => u.Taches)
                                      .FirstOrDefault(u => u.UtilisateurId == idUtilisateur);

            if (utilisateur == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            foreach (var todo in utilisateur.Todos)
            {
                if (todo.TodoId == idTodo)
                {
                    utilisateur.NombreHeuresRates += todo.Duree;
                }
            }

            _context.SaveChanges();

            // Retourner un message de succès
            return Ok("Les ratés de l'utilisateur ont été mis à jour.");
        }

        // Rafraîchir la note, les ratés et les heures disponibles d'un utilisateur pour une todo spécifique
        [HttpPut("RafraichirNoteRatesDisponibles")]
        public IActionResult RafraichirNoteRatesDisponibles(int idUtilisateur, string nomTodo)
        {
            var utilisateur = _context.Utilisateurs
                                      .Include(u => u.Todos)
                                      .Include(u => u.Disponibilites)
                                      .Include(u => u.Taches)
                                      .FirstOrDefault(u => u.UtilisateurId == idUtilisateur);

            if (utilisateur == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            foreach (var tache in utilisateur.Taches)
            {
                if (tache.Nom == nomTodo && tache.NombreHeuresRealisees == tache.Duree)
                {
                    utilisateur.Note = 100;
                    utilisateur.NombreHeuresRates = 0;
                    _context.SaveChanges();

                    // Calculer les heures disponibles
                    return CalculerHeuresDisponibles(idUtilisateur);
                }
            }

            // Si la condition n'est pas satisfaite, retourne une réponse sans contenu
            return NoContent();
        }
    }
}


