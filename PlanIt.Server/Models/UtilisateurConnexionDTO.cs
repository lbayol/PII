using System.ComponentModel.DataAnnotations;
public class UtilisateurConnexionDTO
{
    [Required(ErrorMessage = "Le champ 'Email' est requis.")]
    
    [EmailAddress(ErrorMessage = "Le champ 'Email' doit être une adresse email valide.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Le champ 'Password' est requis.")]
    public string Password { get; set; }
}
