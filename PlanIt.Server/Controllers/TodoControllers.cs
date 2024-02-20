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

    var date = dateDemarrage;

    foreach (var tache in utilisateur.Taches)
    {
        var dureeTacheRestante = tache.Duree;

        // Tant qu'il reste de la durée à attribuer à la tâche et qu'on n'a pas dépassé la deadline
        while (dureeTacheRestante > 0 && date <= tache.Deadline)
        {
            // Obtenir l'index du jour de la semaine correspondant à la date actuelle
            var jourSemaine = ((int)date.DayOfWeek + 6) % 7; // Ajustement pour obtenir l'index correct du Lundi au Dimanche
            var disponibiliteJour = utilisateur.Disponibilites.ElementAt(jourSemaine);
            var heureDisponibiliteRestante = disponibiliteJour.NbHeure;
            if(todos.Count>0)
            {
                var lastTodo = todos[todos.Count - 1];
                if(lastTodo.Date==date)
                {
                    heureDisponibiliteRestante -= lastTodo.Duree;
                }
            }
            // Vérifier si l'utilisateur a de la disponibilité pour ce jour
            if (disponibiliteJour.NbHeure > 0)
            {
                // Calculer la durée de la tâche à attribuer pour ce jour
                var dureeTodo = Math.Min(heureDisponibiliteRestante, dureeTacheRestante);

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

                // Mettre à jour la disponibilité restante
                heureDisponibiliteRestante -= dureeTodo;
            }
            if(dureeTacheRestante!=0 || heureDisponibiliteRestante == 0)
            {
                // Passer au jour suivant
                date = date.AddDays(1);
            }
        }
    }

    // Ajouter les Todos à la base de données
    _context.Todos.AddRange(todos);
    _context.SaveChanges();

    return Ok("Todos générés avec succès");
}

[HttpPut("{utilisateurId}/todos/{todoId}/changerRates")]
public IActionResult ChangerRatesTodo(int utilisateurId, int todoId, [FromBody] int nouveauRates)
{
    var utilisateur = _context.Utilisateurs
        .Include(u => u.Todos)
        .FirstOrDefault(u => u.UtilisateurId == utilisateurId);

    if (utilisateur == null)
        return NotFound("Utilisateur non trouvé");

    // Trouver la Todo correspondante dans la liste des Todos de l'utilisateur
    var todo = utilisateur.Todos.FirstOrDefault(t => t.TodoId == todoId);

    if (todo == null)
        return NotFound("Todo non trouvé");

    // Mettre à jour l'entier Rates de la Todo
    todo.Rates = nouveauRates;

    // Enregistrer les modifications dans la base de données
    _context.SaveChanges();

    return Ok("Rates de la Todo mis à jour avec succès");
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
