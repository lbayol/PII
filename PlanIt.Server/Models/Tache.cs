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
    public int NombreHeuresRealisees { get; set; }
    public bool Realisation {get; set;}
    public Tache()
    {
        NombreHeuresRealisees = 0;
        Realisation = false;
    }
}