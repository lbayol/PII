using System.ComponentModel.DataAnnotations;

public class TodoGET
{
    [Key]
    public int TodoId {get; set;}
    public string Nom {get; set;}
    public int Duree {get; set;}
    public DateTimeOffset Date {get; set;}
    public bool Realisation {get; set;}
    public int Rates {get; set;}
    public TodoGET()
    {
        Realisation = false;
        Rates = 0;
    }
}