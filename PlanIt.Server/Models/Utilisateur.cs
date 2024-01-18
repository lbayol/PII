public class Utilisateur
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string Mail { get; set; }
    public string Password { get; set; }
    public List<int> Disponibilites {get; set;}
    public List<Tache> Taches {get;set;}
    public List<Todo> Todos {get;set;}
    public Utilisateur()
    {
        Disponibilites = new List<int>();
        Taches = new List<Tache>();
        Todos = new List<Todo>();
    }
}