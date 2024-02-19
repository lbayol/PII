using System.Text.Json.Serialization;

public class Tache
{
    public int TacheId {get; set;}
    public string Nom {get; set;}
    public int Duree {get; set;}
    public DateTimeOffset Deadline {get; set;}
    public int UtilisateurId { get; set; }
    [JsonIgnore]
    public Utilisateur Utilisateur { get; set; }
    public Tache()
    {}
}