// Cette classe représente la disponibilité d'un utilisateur. Chaque utilisateur a une liste de 7 disponibilités, chaque disponibilité correspondant à un jour de la semaine
// (la 0 pour lundi, 1 pour mardi, ..., 6 pour dimanche). Chaque disponibilité a un nombre d'heures et un identifiant unique. Pour des raisons de simplification, nous partons du principe
// qu'un utilisateur a la même semaine type chaque semaine (toujours les mêmes disponibilités).

using System.Text.Json.Serialization;

public class Disponibilite
{
    // Un identifiant unique pour la disponibilité.
    public int DisponibiliteId { get; set; }

    // Le nombre d'heures de disponibilité pour un jour donné.
    public int NbHeure { get; set; }

    // L'identifiant unique de l'utilisateur auquel cette disponibilité appartient.
    public int UtilisateurId { get; set; }

    // L'utilisateur auquel cette disponibilité appartient.
    // L'attribut [JsonIgnore] est utilisé pour ignorer cette propriété lors de la sérialisation et de la désérialisation JSON.
    [JsonIgnore]
    public Utilisateur Utilisateur { get; set; }
}
