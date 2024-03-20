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
    var suppressionResult = SupprimerTodos(utilisateurId);
    if (suppressionResult is NotFoundResult)
        return suppressionResult; // Retourner le NotFound si l'utilisateur n'est pas trouvé
    else if (suppressionResult is OkResult)
        Console.WriteLine("Tous les Todos de l'utilisateur ont été supprimés avec succès");
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

    // Tri des tâches par ordre croissant de deadline
    var tachesTriees = utilisateur.Taches.OrderBy(t => t.Deadline);

    foreach (var tache in tachesTriees)
    {
        Console.WriteLine("tache.Nom : ");
        Console.WriteLine(tache.Nom);
        Console.WriteLine("tache.Duree : ");
        Console.WriteLine(tache.Duree);
        Console.WriteLine("tache.NombreHeuresRealisees : ");
        Console.WriteLine(tache.NombreHeuresRealisees);
        if (tache.Realisation == true)
        {
            continue;
        }

        var dureeTacheRestante = tache.Duree;

        // Tant qu'il reste de la durée à attribuer à la tâche et qu'on n'a pas dépassé la deadline
        while (dureeTacheRestante > 0 && date <= tache.Deadline)
        {
            // Obtenir l'index du jour de la semaine correspondant à la date actuelle
            var jourSemaine = ((int)date.DayOfWeek + 6) % 7; // Ajustement pour obtenir l'index correct du Lundi au Dimanche
            var disponibiliteJour = utilisateur.Disponibilites.ElementAt(jourSemaine);
            var heureDisponibiliteRestante = disponibiliteJour.NbHeure;
            foreach(var todo in todos)
            {
                if(todo.Date == date)
                {
                    heureDisponibiliteRestante -= todo.Duree;
                }
            }
            // Vérifier si l'utilisateur a de la disponibilité pour ce jour
            if (heureDisponibiliteRestante > 0)
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
                    Realisation = todo.Realisation
                };
                todos.Add(todo);
                utilisateur.Todos.Add(todoGET);
                // Mettre à jour la durée restante de la tâche
                dureeTacheRestante -= dureeTodo;
            }
            if(heureDisponibiliteRestante == 0)
            {
                // Passer au jour suivant
                date = date.AddDays(1);
            }
        }
    }

    // Tri des Todos par ordre croissant de date
    todos = todos.OrderBy(t => t.Date).ToList();

    // Ajouter les Todos à la base de données
    _context.Todos.AddRange(todos);
    _context.SaveChanges();

    return Ok("Todos générés avec succès");
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

    // Stocker la durée initiale de chaque tâche
    var dureesInitiales = new Dictionary<int, int>(); // Clé: TâcheId, Valeur: Durée initiale
    foreach (var tache in utilisateur.Taches)
    {
        Console.WriteLine("tache.Nom ligne 221 : ");
        Console.WriteLine(tache.Nom);
        Console.WriteLine("tache.Duree : ");
        Console.WriteLine(tache.Duree);
        Console.WriteLine("tache.NombreHeuresRealisees : ");
        dureesInitiales.Add(tache.TacheId, tache.Duree);
    }


    var todos = utilisateur.Todos
        .Where(t => t.Date.Date >= dateDemarrage.AddDays(-1).Date) // Comparer uniquement les dates (ignorer l'heure)
        .ToList();

    // Calculer la somme des durées des Todos pour chaque tâche
    var dureesTotales = new Dictionary<int, int>(); // Clé: TâcheId, Valeur: Somme des durées des Todos
    foreach (var todoToRegenerate in todos)
    {
        var tacheId = utilisateur.Taches.FirstOrDefault(t => t.Nom == todoToRegenerate.Nom)?.TacheId;
        if (tacheId != null)
        {
            if (!dureesTotales.ContainsKey(tacheId.Value))
            {
                dureesTotales.Add(tacheId.Value, 0);
            }
            dureesTotales[tacheId.Value] += todoToRegenerate.Duree;
        }
    }


    // Modifier la durée des tâches
    foreach (var tache in utilisateur.Taches)
    {
        var tacheId = tache.TacheId;
        if (dureesTotales.ContainsKey(tacheId))
        {
            tache.Duree = dureesTotales[tacheId];
            Console.WriteLine("tache.Nom ligne 257 : ");
            Console.WriteLine(tache.Nom);
            Console.WriteLine("tache.Duree : ");
            Console.WriteLine(tache.Duree);
            Console.WriteLine("tache.NombreHeuresRealisees : ");
        }
    }

    _context.SaveChanges(); // Enregistrer les modifications sur les durées des tâches

    

    // Appeler la méthode GenererTodos pour générer les Todos à partir de la date de la Todo
    //Là on fait générer todos alors qu'on a pas encore redonner les vraies durées des tâches
    var result = GenererTodos(utilisateur.UtilisateurId, dateDemarrage.ToString("dd-MM-yyyy"));

    // Restaurer les durées initiales des tâches
    foreach (var tache in utilisateur.Taches)
    {
        tache.Duree = dureesInitiales[tache.TacheId];
    }

    _context.SaveChanges(); // Enregistrer les modifications sur les durées des tâches

    return result;
}


[HttpDelete("{utilisateurId}/todos/{todoId}/supprimerTodo")]
public IActionResult SupprimerTodo(int utilisateurId, int todoId)
{
    var utilisateur = _context.Utilisateurs
        .Include(u => u.Todos)
        .FirstOrDefault(u => u.UtilisateurId == utilisateurId);

    if (utilisateur == null)
        return NotFound("Utilisateur non trouvé");

    // Rechercher la Todo correspondante dans la liste de Todos de l'utilisateur
    var todoToRemove = utilisateur.Todos.FirstOrDefault(t => t.TodoId == todoId);
    if (todoToRemove == null)
        return NotFound("Todo non trouvé dans la liste de l'utilisateur");

    foreach(var tache in utilisateur.Taches)
    {
        if(tache.Nom == todoToRemove.Nom)
        {
            tache.NombreHeuresRealisees += todoToRemove.Duree;
            Console.WriteLine("nom tache : ");
            Console.WriteLine(tache.Nom);
            Console.WriteLine("nombreheuresrealisees tache : ");
            Console.WriteLine(tache.NombreHeuresRealisees);
            Console.WriteLine("duree tache : ");
            Console.WriteLine(tache.Duree);
            if(tache.NombreHeuresRealisees == tache.Duree)
            {
                tache.Realisation = true;
            }
        }
    }
    // Supprimer la Todo de la liste de Todos de l'utilisateur
    utilisateur.Todos.Remove(todoToRemove);

    // Rechercher la Todo correspondante dans le contexte de la base de données
    var todoEntityToRemove = _context.Todos.Find(todoId);
    if (todoEntityToRemove == null)
        return NotFound("Todo non trouvé dans la base de données");

    // Supprimer la Todo du contexte de la base de données
    _context.Todos.Remove(todoEntityToRemove);

    // Enregistrer les modifications dans la base de données
    _context.SaveChanges();

    var todosMisesAJour = utilisateur.Todos.ToList();
    return Ok(todosMisesAJour);

}

    }
}
