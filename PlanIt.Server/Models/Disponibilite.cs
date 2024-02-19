using System.Text.Json.Serialization;

public class Disponibilite
{
    public int DisponibiliteId {get; set;}
    public int NbHeure {get; set;}
    public int UtilisateurId { get; set; }
    [JsonIgnore]
    public Utilisateur Utilisateur { get; set; }
}