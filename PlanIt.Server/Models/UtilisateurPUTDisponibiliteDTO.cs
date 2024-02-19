using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
public class UtilisateurPUTDisponibiliteDTO
    {
        public int UtilisateurId { get; set; }
        public List<int> Disponibilites { get; set; }

    }