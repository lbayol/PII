// Afin de pouvoir obtenir la liste de Todo d'un utilisateur à l'aide d'un GET, il a fallu créer cette DTO, pour éviter
// un effet de cycle puisque cette DTO ne possède pas d'utilisateur. (Sinon, on appelle l'utilisateur, qui appelle la todo, qui appelle l'utilisateur etc.)

using System.ComponentModel.DataAnnotations;

public class TodoGET
{
    [Key]
    public int TodoId { get; set; }
    public string Nom { get; set; }
    public int Duree { get; set; }
    public DateTimeOffset Date { get; set; }
    public bool Realisation { get; set; }
    public int Rates { get; set; }
    public TodoGET()
    {
        Realisation = false;
        Rates = 0;
    }
}