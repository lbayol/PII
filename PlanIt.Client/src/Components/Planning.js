import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Navbar from "./Navbar";
import axios from 'axios';
import moment from 'moment';
import 'moment-locale-fr';

export const Planning = () => {
    const prenom = localStorage.getItem('prenom');
    const nom = localStorage.getItem('nom');
    const disponibilites = JSON.parse(localStorage.getItem('disponibilites')); // Convertir en objet JavaScript
    const idutilisateur = parseInt(localStorage.getItem('idutilisateur'));
    const email = localStorage.getItem('email');
    const navigate = useNavigate();
    const [dateTodos, setDateTodos] = useState([]);
    const [note, setNote] = useState(localStorage.getItem('note'));
    const [todos, setTodos] = useState(JSON.parse(localStorage.getItem('todos')));
    const [taches, setTaches] = useState(JSON.parse(localStorage.getItem('taches')));
    const [selectedDate, setSelectedDate] = useState(moment().format("YYYY-MM-DD"));
    const [showConfirmation, setShowConfirmation] = useState(false);
    const [showPreviousDaysMessage, setShowPreviousDaysMessage] = useState(false); // Nouveau état pour afficher le message
    const todosBeforeSelectedDate = todos.filter(todo => moment(todo.date).isBefore(selectedDate, 'day'));

    const handleLeftArrowClick = () => {
        const newDate = moment(selectedDate).subtract(1, 'day').format("YYYY-MM-DD");
        setSelectedDate(newDate);
    };

    const handleRightArrowClick = () => {
        const newDate = moment(selectedDate).add(1, 'day').format("YYYY-MM-DD");
        setSelectedDate(newDate);
    };

    const handleCompleteTodo = async (todoId) => {
        try {
            // Rafraîchir les heures réalisées
            console.log(todoId);
            await rafraichirHeuresRealisees(idutilisateur, todoId);
            
            // Mettre à jour les tâches
            const userResponse = await axios.get(`http://localhost:5035/api/utilisateur/infos/${idutilisateur}`);
            const updatedTaches = userResponse.data.taches;
            setTaches(updatedTaches);
            console.log(todos);
            // Définir l'ID de la tâche terminée
            setCompletedTodoId(todoId);
    
            // Déclencher l'effet secondaire en modifiant completeTodoTrigger
            setCompleteTodoTrigger(true);
        } catch (error) {
            console.error('Une erreur est survenue : ', error);
        }
    };
    
    
    
    const [completeTodoTrigger, setCompleteTodoTrigger] = useState(false);
    const [completedTodoId, setCompletedTodoId] = useState(null); // Ajout d'un état pour stocker l'ID de la tâche terminée

useEffect(() => {
    const fetchData = async (todoId) => { // Passer todoId en paramètre
        try {
            // Trouver la todo dans la liste dateTodos en fonction de son ID
        const todo = dateTodos.find(todo => todo.todoId === todoId);
        // Extraire le nom de la todo
        const nomTodo = todo.nom;
            // Supprimer la todo avec l'ID correspondant
            console.log(todos);
            await axios.delete(`http://localhost:5035/api/utilisateur/${idutilisateur}/todos/${todoId}/supprimerTodo`);
            console.log(todos);
            await rafraichirNoteRatesDisponibles(idutilisateur, nomTodo);
            console.log(todos);

            // Rafraîchir les notes et les disponibilités
            console.log(taches);
            console.log(todoId);
            const response = await axios.get(`http://localhost:5035/api/utilisateur/infos/${idutilisateur}`);
            const updatedTodos = response.data.todos;
            const updatedNote = response.data.note;

            // Mettre à jour le localStorage avec les données mises à jour
            localStorage.setItem('taches', JSON.stringify(response.data.taches));
            localStorage.setItem('todos', JSON.stringify(updatedTodos));
            localStorage.setItem('note', updatedNote);
            console.log(todos);

            // Mettre à jour les états avec les données mises à jour
            setTaches(response.data.taches);
            setTodos(updatedTodos);
            setNote(updatedNote);

            // Mettre à jour dateTodos
            const updatedDateTodos = dateTodos.filter(todo => todo.todoId !== todoId);
            setDateTodos(updatedDateTodos);
        } catch (error) {
            console.error('Une erreur est survenue : ', error);
        }
        console.log("taches après avoir cliqué sur oui : ",taches);
    };

    if (completeTodoTrigger) {
        fetchData(completedTodoId); // Passer completedTodoId à fetchData
        setCompleteTodoTrigger(false);
    }
}, [completeTodoTrigger, completedTodoId]); // Ajouter completedTodoId comme dépendance


    
    
    
    
    
    
    const handleNonButtonClick = (todoId) => {
        // Afficher la question et les nouveaux boutons si nécessaire
        if (dateTodos.length > 1) {
            setShowConfirmation(true);
        }
        else{
            handleOuiButtonClick(todoId);
        }
    };    
    
    const handleOuiButtonClick = async (todoId) => {
        console.log(todos);
        await rafraichirRates(idutilisateur, todoId);
        console.log(todos);
        try {
            // Appeler la méthode RegenererTodos pour régénérer les Todos
            console.log(todos);
            await axios.post(`http://localhost:5035/api/utilisateur/regenererTodos/${todoId}`);
            console.log(todos);
            
            axios.put(`http://localhost:5035/api/utilisateur/UpdateNote/${idutilisateur}`)
            .then(noteResponse => {
                console.log(todos);
                // Effectuer une requête GET pour récupérer les informations de l'utilisateur mises à jour
                axios.get(`http://localhost:5035/api/utilisateur/infos/${idutilisateur}`)
                .then(userResponse => {
                    console.log(todos);
                    // Mettre à jour la note dans le localStorage avec la nouvelle note récupérée
                    localStorage.setItem('note', userResponse.data.note);
                    setNote(userResponse.data.note);
                    localStorage.setItem('todos', JSON.stringify(userResponse.data.todos));
                    setTodos(userResponse.data.todos);
                    console.log(todos);
                })
                .catch(error => {
                    console.error('Une erreur est survenue lors de la récupération des informations de l\'utilisateur : ', error);
                });
            })
            .catch(error => {
                console.error('Une erreur est survenue lors de la mise à jour de la note de l\'utilisateur : ', error);
            });
            console.log(todos);
            // Cacher la confirmation après avoir traité la réponse
            setShowConfirmation(false);
        } catch (error) {
            console.error('Une erreur est survenue : ', error);
        }
        console.log("taches après avoir cliqué sur oui : ",taches);
    };

    // Fonction pour calculer le nombre total d'heures par tâche
const calculerHeureTotaleTache = (taskName) => {
    let totalHours = 0;
    todos.forEach(todo => {
        if (todo.nom === taskName) {
            totalHours += todo.duree;
        }
    });
    return totalHours;
};

const rafraichirRates = (idUtilisateur, idTodo) => {
    return new Promise((resolve, reject) => {
      axios.put(`http://localhost:5035/api/utilisateur/RafraichirRates?idUtilisateur=${idUtilisateur}&idTodo=${idTodo}`)
        .then(response => {
          console.log("Les ratés et la note de l'utilisateur ont été mis à jour : ", response.data);
          resolve(response.data);
        })
        .catch(error => {
          console.error("Erreur lors de la mise à jour des ratés et de la note de l'utilisateur :", error);
          reject(error);
        });
    });
  };

  const rafraichirHeuresRealisees = (idUtilisateur, idTodo) => {
    return new Promise((resolve, reject) => {
      axios.put(`http://localhost:5035/api/utilisateur/RafraichirHeuresRealisees?idUtilisateur=${idUtilisateur}&idTodo=${idTodo}`)
        .then(response => {
          console.log("Les heures réalisées de la tâche ont été mis à jour : ", response.data);
          resolve(response.data);
        })
        .catch(error => {
          console.error("Erreur lors de la mise à jour des heures réalisées de la tache :", error);
          reject(error);
        });
    });
  };

  const rafraichirNoteRatesDisponibles = (idUtilisateur, nomTodo) => {
    return new Promise((resolve, reject) => {
      axios.put(`http://localhost:5035/api/utilisateur/RafraichirNoteRatesDisponibles?idUtilisateur=${idUtilisateur}&nomTodo=${nomTodo}`)
        .then(response => {
          console.log("La note, les ratés et les heures disponibles de l'utilisateur ont bien été mis à jour : ", response.data);
          resolve(response.data);
        })
        .catch(error => {
          console.error("Erreur lors de la mise à jour de la note, des ratés et des heures disponibles de l'utilisateur :", error);
          reject(error);
        });
    });
  };

  const calculerDureeTotaleAvantDateSelectionnee = (todo) => {
    return todosBeforeSelectedDate
        .filter(t => t.nom === todo.nom)
        .reduce((total, t) => total + t.duree, 0);
    };


    useEffect(() => {
        if (!prenom || !nom) {
            navigate('/connexion');
        } else {
            const filteredTodos = todos.filter(todo => moment(todo.date).format("YYYY-MM-DD") === selectedDate && todo.realisation == false);
            setDateTodos(filteredTodos);

            // Vérifier s'il y a des Todos non complétées avec une date inférieure à aujourd'hui
            const hasUncompletedPastTodos = todos.some(todo => moment(todo.date).isBefore(moment(), 'day') && !todo.realisation);
            setShowPreviousDaysMessage(hasUncompletedPastTodos);
        }
    }, [selectedDate, todos]);
    if(note<0) {
        return(
            <div>
                <h3>Votre planning</h3>
                <p>Vous n'avez pas respecté votre planning, il n'est désormais plus réalisable avec les données que vous avez rentré.</p>
                <Link to="/creerplanning">Veuillez en créer un nouveau</Link>
                <Navbar />
            </div>
        );
    }
    else if (todos.length === 0) {
        return (
            <div>
                <h3>Vous n'avez pas encore créé de planning</h3>
                <Link to="/creerplanning">Créer un planning</Link>
                <Navbar />
            </div>
        );
    } else if (dateTodos.length === 0) {
        return (
            <div>
                <h1>Votre planning</h1>
                <div>
                    <h2>Pas de todo pour le {moment(selectedDate).locale('fr').format("dddd DD/MM/YYYY").replace(/^\w/, (c) => c.toUpperCase())}</h2>
                    <button onClick={handleLeftArrowClick}>←</button>
                    <button onClick={handleRightArrowClick}>→</button>
                </div>
                {showPreviousDaysMessage && (
                    <p>Vous n’avez pas précisé si vous avez effectué vos todos les jours précédents. Veuillez l’indiquer pour mettre à jour votre planning si nécessaire.</p>
                )}
                <br />
                <h4>Note : {note}/100</h4>
                <Navbar />       
            </div>
        );
    } else {
        return (
            <div>
                <h1>Votre planning</h1>
                <div>
                <h2>{moment(selectedDate).locale('fr').format("dddd DD/MM/YYYY").replace(/^\w/, (c) => c.toUpperCase())}</h2>
                    <button onClick={handleLeftArrowClick}>←</button>
                    <button onClick={handleRightArrowClick}>→</button>
                </div>
                <h4>Todos du jour : </h4>
                <ul>
                {dateTodos.map((todo, index) => (
                    <li key={index}>
                        {todo.nom} ({todo.duree + taches.find(tache => tache.nom === todo.nom)?.nombreHeuresRealisees + calculerDureeTotaleAvantDateSelectionnee(todo)} / {taches.find(tache => tache.nom === todo.nom)?.duree}), {todo.duree} heures
                        <br />
                        Avez-vous complété cette todo ?
                        <button onClick={() => handleCompleteTodo(todo.todoId)}>Oui</button>
                        <button onClick={() => handleNonButtonClick(todo.todoId)}>Non</button>
                        {showConfirmation && index === dateTodos.length - 1 && (
                            <div>
                                <p>Attention, avez-vous bien coché toutes les Todos effectuées cette journée ?</p>
                                <button onClick={() => handleOuiButtonClick(todo.todoId)}>Oui</button>
                                <button onClick={() => setShowConfirmation(false)}>Non</button>
                            </div>
                        )}
                    </li>
                ))}

                </ul>

        
                {showPreviousDaysMessage && (
                    <p>Vous n’avez pas précisé si vous avez effectué vos todos les jours précédents. Veuillez l’indiquer pour mettre à jour votre planning si nécessaire.</p>
                )}
        
                <br />
                <h4>Note : {note}/100</h4>
                <Navbar />
            </div>
        );                  
    }
};

export default Planning;
