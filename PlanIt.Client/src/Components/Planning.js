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
    const taches = JSON.parse(localStorage.getItem('taches')); // Convertir en objet JavaScript
    const idutilisateur = parseInt(localStorage.getItem('idutilisateur'));
    const email = localStorage.getItem('email');
    const navigate = useNavigate();
    const [dateTodos, setDateTodos] = useState([]);
    const [note, setNote] = useState(localStorage.getItem('note'));
    const [todos, setTodos] = useState(JSON.parse(localStorage.getItem('todos')));
    const [selectedDate, setSelectedDate] = useState(moment().format("YYYY-MM-DD"));
    const [showConfirmation, setShowConfirmation] = useState(false);
    const [showPreviousDaysMessage, setShowPreviousDaysMessage] = useState(false); // Nouveau état pour afficher le message

    const handleLeftArrowClick = () => {
        const newDate = moment(selectedDate).subtract(1, 'day').format("YYYY-MM-DD");
        setSelectedDate(newDate);
    };

    const handleRightArrowClick = () => {
        const newDate = moment(selectedDate).add(1, 'day').format("YYYY-MM-DD");
        setSelectedDate(newDate);
    };

    const handleCompleteTodo = (todoId) => {
        // Supprimer la todo avec l'ID correspondant
        axios.delete(`http://localhost:5035/api/utilisateur/${idutilisateur}/todos/${todoId}/supprimerTodo`)
        .then(response => {
            // Filtrer la liste des todos pour supprimer la todo supprimée
            const updatedTodos = dateTodos.filter(todo => todo.todoId !== todoId);
            
            // Mise à jour de la variable d'état dateTodos avec la liste de todos mise à jour
            setDateTodos(updatedTodos);
            
            // Mise à jour du localStorage avec la liste de todos mise à jour
            localStorage.setItem('todos', JSON.stringify(response.data));
    
            // Mettre à jour la note de l'utilisateur
            axios.put(`http://localhost:5035/api/utilisateur/UpdateNote/${idutilisateur}`)
            .then(noteResponse => {
                // Effectuer une requête GET pour récupérer les informations de l'utilisateur mises à jour
                axios.get(`http://localhost:5035/api/utilisateur/infos/${idutilisateur}`)
                .then(userResponse => {
                    // Mettre à jour la note dans le localStorage avec la nouvelle note récupérée
                    localStorage.setItem('note', userResponse.data.note);
                    setNote(userResponse.data.note);
                })
                .catch(error => {
                    console.error('Une erreur est survenue lors de la récupération des informations de l\'utilisateur : ', error);
                });
            })
            .catch(error => {
                console.error('Une erreur est survenue lors de la mise à jour de la note de l\'utilisateur : ', error);
            });
        })
        .catch(error => {
            console.error('Une erreur est survenue lors de la suppression de la todo : ', error);
        });
    };
    
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
    };

    useEffect(() => {
        if (!prenom || !nom) {
            navigate('/connexion');
        } else {
            const filteredTodos = todos.filter(todo => moment(todo.date).format("YYYY-MM-DD") === selectedDate);
            setDateTodos(filteredTodos);

            // Vérifier s'il y a des Todos non complétées avec une date inférieure à aujourd'hui
            const hasUncompletedPastTodos = todos.some(todo => moment(todo.date).isBefore(moment(), 'day') && !todo.realisation);
            setShowPreviousDaysMessage(hasUncompletedPastTodos);
        }
    }, [selectedDate, todos]);

    if (todos.length === 0) {
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
                            {todo.nom} - Durée : {todo.duree} heures
                            <br />
                            Avez-vous complété cette todo ?
                            <button onClick={() => handleCompleteTodo(todo.todoId)}>Oui</button>
                            <button onClick={() => handleNonButtonClick(todo.todoId)}>Non</button>
                            {showConfirmation && index===dateTodos.length-1 && (
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
