// DTO variante des tâches permettant de créer une tâche sans renseigner toutes les variables mais seulement celles utiles à la création. 

public class TacheDTO
{
    public string Nom { get; set; }
    public int Duree { get; set; }
    public string Deadline { get; set; }
    public int UtilisateurId { get; set; }
}
