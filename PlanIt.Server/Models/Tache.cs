// Cette classe représente une tâche. Chaque utilisateur renseigne les tâches qu'il aura à faire afin que son planning soit généré.
// Chaque tâche est ensuite divisée en plusieurs todos selon les disponibilités de l'utilisateur.

using System.Text.Json.Serialization;

public class Tache
{
    // Un identifiant unique pour la tâche.
    public int TacheId { get; set; }

    // Le nom de la tâche.
    public string Nom { get; set; }

    // La durée estimée pour accomplir la tâche, en heures.
    public int Duree { get; set; }

    // La date limite à laquelle la tâche doit être accomplie.
    public DateTimeOffset Deadline { get; set; }

    // L'identifiant unique de l'utilisateur auquel la tâche est assignée.
    public int UtilisateurId { get; set; }

    // L'utilisateur auquel la tâche est assignée.
    // L'attribut [JsonIgnore] est utilisé pour ignorer cette propriété lors de la sérialisation et de la désérialisation JSON.
    [JsonIgnore]
    public Utilisateur Utilisateur { get; set; }

    // Le nombre d'heures déjà réalisées pour cette tâche.
    public int NombreHeuresRealisees { get; set; }

    // Un booléen indiquant si la tâche a été accomplie ou non.
    public bool Realisation { get; set; }

    // Initialise les propriétés NombreHeuresRealisees et Realisation à 0 et false, respectivement.
    public Tache()
    {
        NombreHeuresRealisees = 0;
        Realisation = false;
    }
}
