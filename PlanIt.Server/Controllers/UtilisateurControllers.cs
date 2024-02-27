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
public IActionResult GetUtilisateurByEmail(string email)
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
public IActionResult PutDisponibilitesUtilisateur(int id, [FromBody] UtilisateurPUTDisponibiliteDTO utilisateurDTO)
{
    if (utilisateurDTO == null || id != utilisateurDTO.UtilisateurId)
    {
        return BadRequest("Les données de l'utilisateur ou l'ID ne correspondent pas.");
    }

    var utilisateur = _context.Utilisateurs.Include(u => u.Disponibilites).FirstOrDefault(u => u.UtilisateurId == id);

    if (utilisateur == null)
    {
        return NotFound("Utilisateur non trouvé.");
    }

    // Assurez-vous que le nombre de disponibilités envoyées est correct
    if (utilisateurDTO.Disponibilites.Count != utilisateur.Disponibilites.Count)
    {
        return BadRequest("Le nombre de disponibilités envoyées est incorrect.");
    }

    // Mettre à jour les disponibilités de l'utilisateur
    for (int i = 0; i < utilisateur.Disponibilites.Count; i++)
    {
        utilisateur.Disponibilites[i].NbHeure = utilisateurDTO.Disponibilites[i];
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

    // Calculer le nombre total d'heures de todos ratées
    int heuresTotalesRatées = utilisateur.Todos.Sum(t => t.Duree * t.Rates);

    // Récupérer la date actuelle
    DateTimeOffset maintenant = DateTimeOffset.Now;

    // Calculer le nombre total d'heures de créneaux disponibles avant la prochaine tâche
    int heuresTotalesDisponibles = 0;
    foreach (var tache in utilisateur.Taches)
    {
        // Trouver la todo associée à cette tâche avec la date la plus récente
        var todoAssociee = utilisateur.Todos.Where(t => t.Nom == tache.Nom)
                                            .OrderByDescending(t => t.Date)
                                            .FirstOrDefault();

        // Vérifier si une todo associée à cette tâche existe et si sa date est dans le futur
        // Vérifier si une todo associée à cette tâche existe, si sa date est dans le futur et si elle est antérieure à la deadline de la tâche
        if (todoAssociee != null && todoAssociee.Date >= maintenant && todoAssociee.Date < tache.Deadline)
        {
            // Parcourir chaque jour à partir de la date de la todo associée jusqu'à la deadline de la tâche
            for (DateTimeOffset date = todoAssociee.Date; date <= tache.Deadline; date = date.AddDays(1))
            {
                // Trouver l'index du jour de la semaine, où 0 correspond à Lundi, 1 à Mardi, etc.
                int indexJour = ((int)date.DayOfWeek + 6) % 7;
                heuresTotalesDisponibles += utilisateur.Disponibilites[indexJour].NbHeure;
            }
        }
    }

    // Vérifier pour éviter une division par zéro
    if (heuresTotalesDisponibles == 0)
    {
        return BadRequest("Impossible de calculer la note : aucun créneau disponible.");
    }

    // Calculer la nouvelle note de l'utilisateur
    double nouvelleNote = 100 - (100.0 * heuresTotalesRatées / heuresTotalesDisponibles);

    // Mettre à jour la note de l'utilisateur dans la base de données
    utilisateur.Note = (int)nouvelleNote;

    _context.SaveChanges();

    return Ok($"La note de l'utilisateur avec l'ID {id} a été mise à jour avec succès.");
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
