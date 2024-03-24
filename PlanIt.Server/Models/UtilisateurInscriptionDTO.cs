// DTO variante de l'utilisateur permettant à un utilisateur de s'inscrire en ne renseignant que son e-mail, son mot de passe, son nom et son prénom

using System.ComponentModel.DataAnnotations;

public class UtilisateurInscriptionDTO
{
    [Required(ErrorMessage = "Le champ 'Nom' est requis.")]
    public string Nom { get; set; }

    [Required(ErrorMessage = "Le champ 'Prénom' est requis.")]
    public string Prenom { get; set; }

    [Required(ErrorMessage = "Le champ 'Email' est requis.")]
    [EmailAddress(ErrorMessage = "Le champ 'Email' doit être une adresse email valide.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Le champ 'Password' est requis.")]
    public string Password { get; set; }
}