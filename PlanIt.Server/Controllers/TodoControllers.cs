using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PlanIt.Controllers
{
    [ApiController]
    [Route("api/utilisateur")]
    public class TodoController : ControllerBase
    {
        private readonly PlanItContext _context;

        public TodoController(PlanItContext context)
        {
            _context = context;
        }

[HttpPost("{utilisateurId}/genererTodos")]
public IActionResult GenererTodos(int utilisateurId, [FromBody] string dateDemarrageString)
{
    // Convertir la chaîne de date en DateTimeOffset avec le format spécifié
    if (!DateTimeOffset.TryParseExact(dateDemarrageString, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset dateDemarrage))
    {
        return BadRequest("Format de date invalide. Utilisez le format dd-MM-yyyy");
    }

    var utilisateur = _context.Utilisateurs
        .Include(u => u.Disponibilites)
        .Include(u => u.Taches)
        .Include(u => u.Todos)
        .FirstOrDefault(u => u.UtilisateurId == utilisateurId);

    if (utilisateur == null)
        return NotFound("Utilisateur non trouvé");

    var todos = new List<Todo>();

    // Recherche de la dernière date utilisée pour toutes les tâches
    var lastTodoDate = utilisateur.Todos
        .Where(td => td.Date <= dateDemarrage)  // Prendre en compte seulement les Todos avant la date de démarrage
        .OrderByDescending(td => td.Date)
        .Select(td => td.Date)
        .FirstOrDefault();

    // Utiliser la date de démarrage fournie si aucune date n'a été trouvée
    var date = lastTodoDate != default(DateTimeOffset) ? lastTodoDate.AddDays(1) : dateDemarrage;

    foreach (var tache in utilisateur.Taches)
    {
        var dureeTacheRestante = tache.Duree;

        // Vérifier s'il reste de la disponibilité du jour précédent
        if (lastTodoDate != default(DateTimeOffset) && date > dateDemarrage.AddDays(1))
        {
            // Calculer la disponibilité totale pour le jour actuel
            var jourSemaine = ((int)lastTodoDate.DayOfWeek + 6) % 7;
            var disponibiliteJourPrecedent = utilisateur.Disponibilites.ElementAt(jourSemaine);
            var disponibiliteTotale = disponibiliteJourPrecedent.NbHeure;

            // Calculer la durée de la tâche à attribuer pour ce jour
            var dureeTodo = Math.Min(disponibiliteTotale, dureeTacheRestante);

            // Créer le Todo
            var todo = new Todo
            {
                Nom = tache.Nom,
                Duree = dureeTodo,
                Date = lastTodoDate,
                UtilisateurId = utilisateurId,
                TacheId = tache.TacheId
            };
            todos.Add(todo);

            // Mettre à jour la durée restante de la tâche
            dureeTacheRestante -= dureeTodo;
        }

        // Tant qu'il reste de la durée à attribuer à la tâche et qu'on n'a pas dépassé la deadline
        while (dureeTacheRestante > 0 && date <= tache.Deadline)
        {
            // Obtenir l'index du jour de la semaine correspondant à la date actuelle
            var jourSemaine = ((int)date.DayOfWeek + 6) % 7; // Ajustement pour obtenir l'index correct du Lundi au Dimanche
            var disponibiliteJour = utilisateur.Disponibilites.ElementAt(jourSemaine);

            // Vérifier si l'utilisateur a de la disponibilité pour ce jour
            if (disponibiliteJour.NbHeure > 0)
            {
                // Calculer la durée du Todo pour ce jour
                var dureeTodo = Math.Min(disponibiliteJour.NbHeure, dureeTacheRestante);

                // Créer le Todo
                var todo = new Todo
                {
                    Nom = tache.Nom,
                    Duree = dureeTodo,
                    Date = date,
                    UtilisateurId = utilisateurId,
                    TacheId = tache.TacheId
                };

                todos.Add(todo);

                // Mettre à jour la durée restante de la tâche
                dureeTacheRestante -= dureeTodo;
            }

            // Passer au jour suivant
            date = date.AddDays(1);
        }
    }

    // Ajouter les Todos à la base de données
    _context.Todos.AddRange(todos);
    _context.SaveChanges();

    return Ok("Todos générés avec succès");
}
















        [HttpDelete("{utilisateurId}/supprimerTodos")]
        public IActionResult SupprimerTodos(int utilisateurId)
        {
            var utilisateur = _context.Utilisateurs
                .Include(u => u.Todos)
                .FirstOrDefault(u => u.UtilisateurId == utilisateurId);

            if (utilisateur == null)
                return NotFound("Utilisateur non trouvé");

            // Supprimer tous les Todos liés à l'utilisateur
            _context.Todos.RemoveRange(utilisateur.Todos);
            _context.SaveChanges();

            return Ok("Tous les Todos de l'utilisateur ont été supprimés avec succès");
        }

    }

}
