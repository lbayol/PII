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

[HttpGet("{id}")]
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

[HttpGet("Utilisateur/{Email}")]
public IActionResult GetUserInfo(string Email)
{
    var utilisateur = _context.Utilisateurs.FirstOrDefault(u => u.Email == Email);
    if (utilisateur == null)
        return NotFound("Utilisateur non trouvé");

    return Ok(new { Prenom = utilisateur.Prenom, Nom = utilisateur.Nom });
}

[HttpPut("{id}")]
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
