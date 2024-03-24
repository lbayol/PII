// Cette classe représente un utilisateur. Chaque utilisateur renseigne lors de la création du planning sa liste de disponibilité (une par jour de la semaine)
// ses tâches et c'est alors après que ses todos sont générées. Chaque utilisateur a une note lui permettant d'évaluer son avancée sur le planning.
// La note est initialisée à 100, le nombre d'heures évaluées à 0 et le nombre d'heures disponibles à 0.

using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

public class Utilisateur
{
    // Un identifiant unique pour l'utilisateur.
    [Key]
    public int UtilisateurId { get; set; }

    // Le nom de l'utilisateur.
    public string Nom { get; set; }

    // Le prénom de l'utilisateur.
    public string Prenom { get; set; }

    // L'adresse e-mail de l'utilisateur.
    public string Email { get; set; }

    // Le mot de passe de l'utilisateur.
    public string Password { get; set; }

    // Une liste de disponibilités pour l'utilisateur, une pour chaque jour de la semaine.
    public List<Disponibilite> Disponibilites { get; set; }

    // Une liste de tâches assignées à l'utilisateur.
    public List<Tache> Taches { get; set; }

    // Une liste de todos à faire assignées à l'utilisateur. (TodoGET, explication dans le cartouche du fichier TodoGET.cs)
    public List<TodoGET> Todos { get; set; }

    // Une note pour l'utilisateur, calculée dans le Utilisateur.Controller.cs
    public int Note { get; set; }

    // Le nombre total d'heures ratées par l'utilisateur par rapport à la tâche dont la deadline est la plus proche.
    public int NombreHeuresRates { get; set; }

    // Le nombre total d'heures disponibles pour la tache de l'utilisateur dont la deadline est la plus proche.
    public int NombreHeuresDisponibles { get; set; }

    // Initialise les propriétés Disponibilites, Taches, Todos, Note, NombreHeuresRates et NombreHeuresDisponibles.
    public Utilisateur()
    {
        Disponibilites = new List<Disponibilite>();
        Taches = new List<Tache>();
        Todos = new List<TodoGET>();
        Note = 100;
        NombreHeuresRates = 0;
        NombreHeuresDisponibles = 0;
    }
}
