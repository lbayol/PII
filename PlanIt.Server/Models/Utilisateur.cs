using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

public class Utilisateur
{
    [Key]
    public int UtilisateurId { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public List<Disponibilite> Disponibilites {get; set;}
    public List<Tache> Taches {get;set;}
    public List<TodoGET> Todos {get;set;}
    public int Note {get; set;}
    public Utilisateur()
    {
        Disponibilites = new List<Disponibilite>();
        Taches = new List<Tache>();
        Todos = new List<TodoGET>();
        Note = 0;
    }
}