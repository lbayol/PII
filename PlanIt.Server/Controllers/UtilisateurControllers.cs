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
        private readonly PlanItContext _context;

        public UtilisateurController(PlanItContext context)
        {
            _context = context;
        }

[HttpGet("infos/{id}")]
public IActionResult GetUtilisateurById(int id)
{
    var utilisateur = _context.Utilisateurs
                              .Include(u => u.Disponibilites)
                              .Include(u => u.Taches)
                              .Include(u => u.Todos)
                              .FirstOrDefault(u => u.UtilisateurId == id);

    if (utilisateur == null)
    {
        return NotFound("Utilisateur non trouvé.");
    }

    return Ok(utilisateur);
}




[HttpPost("Inscription")]
public IActionResult PostUtilisateur([FromBody] UtilisateurInscriptionDTO utilisateurDTOPOST)
{
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

    Utilisateur nouvelUtilisateur = new Utilisateur
    {
        Nom = utilisateurDTOPOST.Nom,
        Prenom = utilisateurDTOPOST.Prenom,
        Email = utilisateurDTOPOST.Email,
        Password = motDePasseHaché,
    };

    AjouterDisponibilitesParDefaut(nouvelUtilisateur);

    _context.Utilisateurs.Add(nouvelUtilisateur);
    _context.SaveChanges();

    return Ok("Inscription réussie");
}

private void AjouterDisponibilitesParDefaut(Utilisateur utilisateur)
{
    for (int i = 0; i < 7; i++)
    {
        utilisateur.Disponibilites.Add(new Disponibilite { NbHeure = 0 });
    }
}

[HttpPost("Connexion")]
public IActionResult Connexion([FromBody] UtilisateurConnexionDTO connexionDTO)
{
    // Recherchez l'utilisateur dans la base de données en utilisant l'adresse e-mail
    var utilisateur = _context.Utilisateurs.FirstOrDefault(u => u.Email == connexionDTO.Email);

    // Vérifiez si l'utilisateur existe et si le mot de passe correspond
    if (utilisateur != null && BCrypt.Net.BCrypt.Verify(connexionDTO.Password, utilisateur.Password))
    {
        // L'authentification a réussi, retournez un message réussi
        return Ok("Connexion réussie !");
    }

    // L'authentification a échoué, retournez une réponse d'échec
    return BadRequest("L'authentification a échoué.");
}

[HttpGet("infosConnexion")]
public IActionResult GetUtilisateurByEmail([FromQuery] string email)
{
    var utilisateur = _context.Utilisateurs
                              .Include(u => u.Disponibilites)
                              .Include(u => u.Taches)
                              .Include(u => u.Todos)
                              .FirstOrDefault(u => u.Email == email);

    if (utilisateur == null)
    {
        return NotFound("Utilisateur non trouvé.");
    }

    return Ok(utilisateur);
}


[HttpPut("UpdateDisponibilités/{id}")]
public IActionResult PutDisponibilitesUtilisateur(int id, [FromBody] List<int> disponibilitesData)
{
    if (disponibilitesData == null)
    {
        return BadRequest("Les données de disponibilités sont manquantes.");
    }

    var utilisateur = _context.Utilisateurs.Include(u => u.Disponibilites).FirstOrDefault(u => u.UtilisateurId == id);

    if (utilisateur == null)
    {
        return NotFound("Utilisateur non trouvé.");
    }

    // Assurez-vous que le nombre de disponibilités envoyées est correct
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

    return Ok(utilisateur);
}


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

    // Calculer la nouvelle note de l'utilisateur
    double nouvelleNote = 100 - (100.0 * utilisateur.NombreHeuresRates / utilisateur.NombreHeuresDisponibles);

    // Mettre à jour la note de l'utilisateur dans la base de données
    utilisateur.Note = (int)nouvelleNote;

    if(utilisateur.Note == 0)
    {
        utilisateur.Note = 1;
    }

    _context.SaveChanges();

    return Ok($"La note de l'utilisateur avec l'ID {id} a été mise à jour avec succès.");
}

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

    // Récupérer la date actuelle
    DateTimeOffset maintenant = DateTimeOffset.Now;

    // Calculer le nombre total d'heures de créneaux disponibles avant la prochaine tâche
    int heuresTotalesDisponibles = 0;
    var tachePlusProche = utilisateur.Taches
    .Where(t => t.Duree != t.NombreHeuresRealisees) // Filtrer les tâches où la durée est différente du nombre d'heures réalisées
    .OrderBy(t => t.Deadline) // Ordonner les tâches par date limite
    .FirstOrDefault(); // Sélectionner la première tâche dans l'ordre trié
        // Trouver la todo associée à cette tâche avec la date la plus récente
        var todoAssociee = utilisateur.Todos.Where(t => t.Nom == tachePlusProche.Nom)
                                            .OrderByDescending(t => t.Date)
                                            .FirstOrDefault();

        // Vérifier si une todo associée à cette tâche existe et si sa date est dans le futur
        // Vérifier si une todo associée à cette tâche existe, si sa date est dans le futur et si elle est antérieure à la deadline de la tâche
        if (todoAssociee != null && todoAssociee.Date >= maintenant && todoAssociee.Date <= tachePlusProche.Deadline)
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
    return Ok("Le nombre d'heures disponibles de l'utilisateur a été mis à jour.");
}


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

    return Ok("Toutes les tâches de l'utilisateur ont été supprimées avec succès.");
}
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

    return Ok("Les ratés et la note de l'utilisateur ont été mis à jour.");
}

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

    foreach(var todo in utilisateur.Todos)
    {
        if(todo.TodoId == idTodo)
        {
            utilisateur.NombreHeuresRates += todo.Duree;
        }
    }

     _context.SaveChanges();

    return Ok("Les ratés de l'utilisateur ont été mis à jour.");
}

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

    // Si la condition n'est pas satisfaite, vous pouvez simplement retourner une réponse sans contenu
    return NoContent();
}




        /*[HttpDelete("{id}")]
        public IActionResult DeleteUtilisateur(int id)
        {
            var utilisateur = _context.Utilisateurs.Find(id);

            if (utilisateur == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }
            if (_context.Emprunts.Any(e => e.UtilisateurId == id))
            {
                return BadRequest("Impossible de supprimer l'utilisateur car il a des emprunts en cours.");
            }

            _context.Utilisateurs.Remove(utilisateur);
            _context.SaveChanges();

            return Ok(utilisateur);
        }*/
    }
}
