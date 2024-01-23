using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt;

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

        /*[HttpGet]
        public async Task<ActionResult<IEnumerable<UtilisateurDTOGET>>> GetUtilisateurs()
        {
            var utilisateurs = _context.Utilisateurs.Select(x => new UtilisateurDTOGET(x, _context));
            return await utilisateurs.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UtilisateurDTOGET>> GetUtilisateur(int id)
        {
            var utilisateur = await _context.Utilisateurs.SingleOrDefaultAsync(u => u.Id == id);

            if (utilisateur == null)
            {
                return NotFound("L'ID fourni ne correspond à aucun utilisateur.");
            }

            return new UtilisateurDTOGET(utilisateur, _context);
        }*/


[HttpPost("Inscription")]
public IActionResult PostUtilisateur([FromBody] UtilisateurInscriptionDTO utilisateurDTOPOST)
{
    if (utilisateurDTOPOST == null)
    {
        return BadRequest("Erreur : Veuillez remplir les informations du nouvel utilisateur.");
    }

    // Hacher le mot de passe
    string motDePasseHaché = BCrypt.Net.BCrypt.HashPassword(utilisateurDTOPOST.Password);

    Utilisateur nouvelUtilisateur = new Utilisateur
    {
        Nom = utilisateurDTOPOST.Nom,
        Prenom = utilisateurDTOPOST.Prenom,
        Email = utilisateurDTOPOST.Email,
        Password = motDePasseHaché  // Stocker le mot de passe haché dans la base de données
    };

    _context.Utilisateurs.Add(nouvelUtilisateur);
    _context.SaveChanges();

    return CreatedAtAction("GetUtilisateur", new { id = nouvelUtilisateur.UtilisateurId }, nouvelUtilisateur);
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



        

        /*
        [HttpPut("{id}")]
        public IActionResult PutUtilisateur(int id, [FromBody] UtilisateurDTOPUT utilisateurDTO)
        {
            if (utilisateurDTO == null || id != utilisateurDTO.Id)
            {
                return BadRequest("Les données de l'utilisateur ou l'ID ne correspondent pas.");
            }

            var utilisateur = _context.Utilisateurs.Find(id);

            if (utilisateur == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            utilisateur.Nom = utilisateurDTO.Nom;
            utilisateur.Prenom = utilisateurDTO.Prenom;
            utilisateur.Email = utilisateurDTO.Email;
            _context.SaveChanges();

            return Ok(utilisateur);
        }*/

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
