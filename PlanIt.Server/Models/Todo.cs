// Cette classe est la classe des Todos. Chaque tâche à faire est divisée en une multitude de todos
// en fonction de la durée de la tâche et des disponibilités de l'utilisateur.
public class Todo
{
    // Un identifiant unique pour la todo.
    public int TodoId { get; set; }

    // Le nom de la todo.
    public string Nom { get; set; }

    // La durée de la todo en heure.
    public int Duree { get; set; }

    // La date de la todo.
    public DateTimeOffset Date { get; set; }

    // L'utilisateur auquel la todo est assignée.
    public Utilisateur Utilisateur { get; set; }

    // L'identifiant unique de l'utilisateur auquel la todo est assignée.
    public int UtilisateurId { get; set; }

    // La tâche depuis laquelle provient la todo.
    public Tache Tache { get; set; }

    // L'identifiant unique de la tâche parente.
    public int TacheId { get; set; }

    // Un booléen indiquant si la todo a été accomplie ou non. (ce booléen n'a par la suite jamais été utilisé, mais il aurait été possible de passer les todos en réalisé plutôt que de les supprimer lorsqu'elles sont réalisées)
    public bool Realisation { get; set; }


    // Initialise les propriétés Realisation et Rates à false et 0, respectivement.
    public Todo()
    {
        Realisation = false;
    }
}
