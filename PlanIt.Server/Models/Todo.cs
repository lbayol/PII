public class Todo
{
    public int Id {get; set;}
    public string Nom {get; set;}
    public int Duree {get; set;}
    public DateTime Date {get; set;}
    public Utilisateur Utilisateur {get; set;}
    public int UtilisateurId {get; set;}
    public Tache Tache {get;set;}
    public int TacheId {get; set;}
    public bool Realisation {get; set;}
    public Todo()
    {
        Realisation = false;
    }
}