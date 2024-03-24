// DTO variante de l'utilisateur permettant à un utilisateur de se connecter en ne renseignant que son e-mail et son mot de passe.

using System.ComponentModel.DataAnnotations;
public class UtilisateurConnexionDTO
{
    [Required(ErrorMessage = "Le champ 'Email' est requis.")]

    [EmailAddress(ErrorMessage = "Le champ 'Email' doit être une adresse email valide.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Le champ 'Password' est requis.")]
    public string Password { get; set; }
}
