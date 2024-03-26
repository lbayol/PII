// Ce fichier .js est celui de la page où le planning est affiché. Sur cette page, on peut voir 
// pour chaque jour les todos à faire et dire si on les a faite ou non.

// Différents imports
import "../Styles/Planning.css";
import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Navbar from "./Navbar";
import axios from 'axios';
import moment from 'moment';
import 'moment-locale-fr';

export const Planning = () => {
    // Définition des constantes
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

    // Action lorsque l'on appuie sur la flèche gauche, on recule d'un jour

    const handleLeftArrowClick = () => {
        const newDate = moment(selectedDate).subtract(1, 'day').format("YYYY-MM-DD");
        setSelectedDate(newDate);
    };

    // Action lorsque l'on appuie sur la flèche droite, on avance d'un jour

    const handleRightArrowClick = () => {
        const newDate = moment(selectedDate).add(1, 'day').format("YYYY-MM-DD");
        setSelectedDate(newDate);
    };

    // Action lorsque l'on annonce avoir complété une todo

    const handleCompleteTodo = async (todoId) => {
        try {
            // Rafraîchir les heures réalisées
            await rafraichirHeuresRealisees(idutilisateur, todoId);
            
            // Mettre à jour les tâches
            const userResponse = await axios.get(`http://localhost:5035/api/utilisateur/infos/${idutilisateur}`);
            const updatedTaches = userResponse.data.taches;
            setTaches(updatedTaches);
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

    // Utilisation d'un useEffect pour que les changements opérent directement (sans attendre les fonctions asynchrones)
useEffect(() => {
    const fetchData = async (todoId) => { // Passer todoId en paramètre
        try {
            // Trouver la todo dans la liste dateTodos en fonction de son ID
        const todo = dateTodos.find(todo => todo.todoId === todoId);
        // Extraire le nom de la todo
        const nomTodo = todo.nom;
            // Supprimer la todo avec l'ID correspondant
            await axios.delete(`http://localhost:5035/api/utilisateur/${idutilisateur}/todos/${todoId}/supprimerTodo`);
            await rafraichirNoteRatesDisponibles(idutilisateur, nomTodo);

            // Rafraîchir les notes et les disponibilités
            const response = await axios.get(`http://localhost:5035/api/utilisateur/infos/${idutilisateur}`);
            const updatedTodos = response.data.todos;
            const updatedNote = response.data.note;

            // Mettre à jour le localStorage avec les données mises à jour
            localStorage.setItem('taches', JSON.stringify(response.data.taches));
            localStorage.setItem('todos', JSON.stringify(updatedTodos));
            localStorage.setItem('note', updatedNote);

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

    // Action si l'on dit que l'on a pas complété une todo.

    const handleNonButtonClick = (todoId) => {
        // Afficher la question et les nouveaux boutons si nécessaire
        if (dateTodos.length > 1) {
            setShowConfirmation(true);
        }
        else{
            handleOuiButtonClick(todoId);
        }
    };    
    
    // Action si l'on dit que l'on a pas complété de todo et que l'on confirme alors qu'il y avait plusieurs todos ce jour là

    const handleOuiButtonClick = async (todoId) => {
        await rafraichirRates(idutilisateur, todoId);
        try {
            // Appeler la méthode RegenererTodos pour régénérer les Todos
            await axios.post(`http://localhost:5035/api/utilisateur/regenererTodos/${todoId}`);
            
            axios.put(`http://localhost:5035/api/utilisateur/UpdateNote/${idutilisateur}`)
            .then(noteResponse => {
                // Effectuer une requête GET pour récupérer les informations de l'utilisateur mises à jour
                axios.get(`http://localhost:5035/api/utilisateur/infos/${idutilisateur}`)
                .then(userResponse => {
                    // Mettre à jour la note dans le localStorage avec la nouvelle note récupérée
                    localStorage.setItem('note', userResponse.data.note);
                    setNote(userResponse.data.note);
                    localStorage.setItem('todos', JSON.stringify(userResponse.data.todos));
                    setTodos(userResponse.data.todos);
                })
                .catch(error => {
                    console.error('Une erreur est survenue lors de la récupération des informations de l\'utilisateur : ', error);
                });
            })
            .catch(error => {
                console.error('Une erreur est survenue lors de la mise à jour de la note de l\'utilisateur : ', error);
            });
            // Cacher la confirmation après avoir traité la réponse
            setShowConfirmation(false);
        } catch (error) {
            console.error('Une erreur est survenue : ', error);
        }
        console.log("taches après avoir cliqué sur oui : ",taches);
    };

// Rafraîchir le nombre d'heures ratées pour une tâche pour un utilisateur

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

  // Rafraîchir le nombre d'heures réalisées pour une tâche pour un utilisateur

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

  // Rafraîchir la note, les ratés et les heures disponibles pour un utilisateur

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
    // Si la note est strictement inférieure à 0, le planning est terminé et irréalisable
    if(note<0) {
        return(
            <div className="planning">
                <div className="text-wrapper">
                    PlanIt
                </div>
                <div className="text-wrapper-3">Vous n'avez pas respecté votre planning, il n'est désormais plus réalisable avec les données que vous avez rentré.</div>
                <Link to="/creerplanning" className="vector-wrapper-creer">Créer un planning</Link>
                <Navbar />
            </div>
        );
    }
    else if (todos.length === 0) {
        // S'il n'y a pas de planning de créée.
        return (
            <div className="planning">
                <div className="text-wrapper">
                    PlanIt
                </div>
                <div className="text-wrapper-2">Vous n'avez pas encore créé de planning</div>
                <Link to="/creerplanning" className="vector-wrapper-creer">Créer un planning</Link>
                <Navbar />
            </div>
        );
    } else if (dateTodos.length === 0) {
        // S'il n'y a pas de todo pour le jour selectionné
        return (
            <div className="planning">
                <div className="text-wrapper">
                    PlanIt
                </div>
                <div className="text-wrapper-2">Votre planning</div>
                <div>
                    <div className="text-wrapper-3">{moment(selectedDate).locale('fr').format("dddd DD/MM/YYYY").replace(/^\w/, (c) => c.toUpperCase())}</div>
                    <button onClick={handleLeftArrowClick} className="vector-wrapper-left">←</button>
                    <button onClick={handleRightArrowClick} className="vector-wrapper-right">→</button>
                </div>
                <div className="text-wrapper-4">
                    Aucune todo de prévue aujourd'hui
                </div>
                {showPreviousDaysMessage && (
                    <div className="error-message-date">Vous n’avez pas précisé si vous avez effectué vos todos les jours précédents. Veuillez l’indiquer pour mettre à jour votre planning si nécessaire.</div>
                )}
                <div className="text-wrapper-5">Note :</div>
                <div className="text-wrapper-6">{note}/100</div>
                <Navbar />       
            </div>
        );
    } else {
        // S'il y a bien une todo pour la date sélectionnée
        return (
            <div className="planning">
                <div className="text-wrapper">
                    PlanIt
                </div>
                <div className="text-wrapper-2">Votre planning</div>
                <div>
                    <div className="text-wrapper-3">{moment(selectedDate).locale('fr').format("dddd DD/MM/YYYY").replace(/^\w/, (c) => c.toUpperCase())}</div>
                    <button onClick={handleLeftArrowClick} className="vector-wrapper-left">←</button>
                    <button onClick={handleRightArrowClick} className="vector-wrapper-right">→</button>
                </div>

                {dateTodos.map((todo, index) => (
                    <li className="text-wrapper-7" key={index}>
                        {todo.nom} ({todo.duree + taches.find(tache => tache.nom === todo.nom)?.nombreHeuresRealisees + calculerDureeTotaleAvantDateSelectionnee(todo)} / {taches.find(tache => tache.nom === todo.nom)?.duree}), {todo.duree} heures
                        <br />
                        Avez-vous complété cette todo ?
                        <button onClick={() => handleCompleteTodo(todo.todoId)} className="vector-wrapper-oui">Oui</button>
                        <button onClick={() => handleNonButtonClick(todo.todoId)} className="vector-wrapper-non">Non</button>
                        {showConfirmation && index === dateTodos.length - 1 && (
                            <div>
                                <br/>
                                <p>Avez-vous bien validé tous les todos effectués du jour ?</p>
                                <button onClick={() => handleOuiButtonClick(todo.todoId)} className="vector-wrapper-oui-effectue">Oui</button>
                                <button onClick={() => setShowConfirmation(false)} className="vector-wrapper-non-effectue">Non</button>
                            </div>
                        )}
                    </li>
                ))}


        
                {showPreviousDaysMessage && (
                    <div className="error-message-date">Vous n’avez pas précisé si vous avez effectué vos todos les jours précédents. Veuillez l’indiquer pour mettre à jour votre planning si nécessaire.</div>
                )}
        
                <div className="text-wrapper-5">Note :</div>
                <div className="text-wrapper-6">{note}/100</div>
                <Navbar />
            </div>
        );                  
    }
};

export default Planning;
