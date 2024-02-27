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
            if(todos.Count > 0)
            {
                var lastTodo = todos.LastOrDefault();
                if(lastTodo != null && lastTodo.Date == date)
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
                var todoGET = new TodoGET
                {
                    TodoId = todo.TodoId,
                    Nom = todo.Nom,
                    Duree = todo.Duree,
                    Date = todo.Date,
                    Realisation = todo.Realisation,
                    Rates = todo.Rates
                };
                todos.Add(todo);
                utilisateur.Todos.Add(todoGET);
                // Mettre à jour la durée restante de la tâche
                dureeTacheRestante -= dureeTodo;

                // Mettre à jour la disponibilité restante
                heureDisponibiliteRestante -= dureeTodo;
            }
            if(dureeTacheRestante != 0 || heureDisponibiliteRestante == 0)
            {
                // Passer au jour suivant
                date = date.AddDays(1);
            }
        }
        // Mettre à zéro la durée de la tâche après la génération des Todos
        tache.Duree = 0;
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

    // Trouver la Todo correspondante dans le contexte de la base de données
    var todoEntity = _context.Todos.FirstOrDefault(t => t.TodoId == todoId);

    if (todoEntity == null)
        return NotFound("Todo non trouvé");

    // Mettre à jour l'entier Rates de la Todo dans le contexte de la base de données
    todoEntity.Rates = nouveauRates;

    // Mettre à jour le nombre de ratés dans la liste de TodoGET de l'utilisateur
    todo.Rates = nouveauRates;

    // Enregistrer les modifications dans la base de données
    _context.SaveChanges();

    return Ok("Rates de la Todo mis à jour avec succès");
}



[HttpPut("todos/{todoId}/marquerRéalisé")]
public IActionResult MarquerTodoRéalisé(int todoId)
{
    // Rechercher la Todo correspondante dans la base de données
    var todo = _context.Todos.Find(todoId);

    if (todo == null)
        return NotFound("Todo non trouvé");

    // Mettre à jour le booléen Realisation de la Todo à true
    todo.Realisation = true;

    // Mettre à jour le booléen Realisation dans la liste de TodoGET de l'utilisateur
    var utilisateur = _context.Utilisateurs
        .Include(u => u.Todos)
        .FirstOrDefault(u => u.Todos.Any(t => t.TodoId == todoId));

    if (utilisateur != null)
    {
        var todoGET = utilisateur.Todos.FirstOrDefault(t => t.TodoId == todoId);
        if (todoGET != null)
        {
            todoGET.Realisation = true;
        }
    }

    // Enregistrer les modifications dans la base de données
    _context.SaveChanges();

    return Ok("Todo marqué comme réalisé avec succès");
}


[HttpDelete("{utilisateurId}/supprimerTodos")]
public IActionResult SupprimerTodos(int utilisateurId)
{
    var utilisateur = _context.Utilisateurs
        .Include(u => u.Todos)
        .FirstOrDefault(u => u.UtilisateurId == utilisateurId);

    if (utilisateur == null)
        return NotFound("Utilisateur non trouvé");

    // Supprimer chaque TodoGET individuellement
    foreach (var todoGET in utilisateur.Todos.ToList())
    {
        utilisateur.Todos.Remove(todoGET);

        // Rechercher la Todo correspondante dans le contexte de la base de données et la supprimer
        var todoToRemove = _context.Todos.Find(todoGET.TodoId);
        if (todoToRemove != null)
        {
            _context.Todos.Remove(todoToRemove);
        }
    }
    
    _context.SaveChanges();

    return Ok("Tous les Todos de l'utilisateur ont été supprimés avec succès");
}





[HttpPost("regenererTodos/{todoId}")]
public IActionResult RegenererTodos(int todoId)
{
    // Récupérer la Todo à partir de son ID
    var todo = _context.Todos
        .Include(t => t.Tache) // Charger la tâche associée à la Todo
        .FirstOrDefault(t => t.TodoId == todoId);

    if (todo == null)
        return NotFound("Todo non trouvée");

    // Récupérer la date de la Todo
    var dateDemarrage = todo.Date.Date.AddDays(1); // Lendemain de la date de la Todo (en ignorant l'heure)

    // Récupérer l'utilisateur associé à cette Todo
    var utilisateur = _context.Utilisateurs
        .Include(u => u.Disponibilites)
        .Include(u => u.Taches)
        .Include(u => u.Todos)
        .FirstOrDefault(u => u.UtilisateurId == todo.UtilisateurId);

    if (utilisateur == null)
        return NotFound("Utilisateur non trouvé");

    var todos = utilisateur.Todos
        .Where(t => t.Date.Date >= dateDemarrage.AddDays(-1).Date) // Comparer uniquement les dates (ignorer l'heure)
        .ToList();

    foreach (var todoToRegenerate in todos)
    {
        // Trouver la tâche correspondante ayant le même nom que la Todo
        var tache = utilisateur.Taches.FirstOrDefault(t => t.Nom == todoToRegenerate.Nom);

        if (tache != null)
        {
            // Ajouter la durée de la Todo à la durée de la tâche correspondante
            tache.Duree += todoToRegenerate.Duree;

            // Supprimer la Todo
            var todoToRemove = _context.Todos.Find(todoToRegenerate.TodoId);
            if (todoToRemove != null)
            {
                _context.Todos.Remove(todoToRemove);
                
                // Supprimer également la TodoGET correspondante dans la liste de TodoGET de l'utilisateur
                utilisateur.Todos.Remove(todoToRegenerate);
            }
        }
    }

    _context.SaveChanges(); // Enregistrer les modifications sur les durées des tâches et la suppression des Todos

    // Appeler la méthode GenererTodos pour générer les Todos à partir de la date de la Todo
    var result = GenererTodos(utilisateur.UtilisateurId, dateDemarrage.ToString("dd-MM-yyyy"));

   // Récupérer tous les Todos non réalisés dans la liste TodoGET de l'utilisateur
var todosToIncrementRates = utilisateur.Todos
    .Where(t => t.Date.Date == dateDemarrage.Date && !t.Realisation)
    .ToList();

// Incrémenter le nombre de ratés des Todos dans la liste TodoGET de l'utilisateur et dans la table Todos
foreach (var todoToIncrementRates in todosToIncrementRates)
{
    todoToIncrementRates.Rates++;

    // Récupérer la Todo correspondante dans la table Todos et incrémenter son nombre de ratés
    var todoEntity = _context.Todos.Find(todoToIncrementRates.TodoId);
    if (todoEntity != null)
    {
        todoEntity.Rates++;
    }
}

// Enregistrer les modifications sur les nombres de ratés dans la table Todos
_context.SaveChanges();

    return result;
}





    }

}
