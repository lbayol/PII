import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import moment from 'moment'; // Importer moment.js
import Navbar from "./Navbar";
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';

// Composant pour représenter les entrées d'une tâche
const TacheInput = ({ index }) => (
  <div>
    Tâche {index} :
    <br />
    Nom
    <input id={`nomTache${index}`}></input>
    <br />
    Durée en heure
    <input id={`dureeTache${index}`}></input>
    <br />
    Deadline
    <input id={`deadlineTache${index}`}></input>
    <br />
  </div>
);

export const CreerPlanning = () => {
  const prenom = localStorage.getItem('prenom');
  const nom = localStorage.getItem('nom');
  const idutilisateur = parseInt(localStorage.getItem('idutilisateur'));
  const navigate = useNavigate();

  const [nombreTaches, setNombreTaches] = useState(1);

  useEffect(() => {
    if (!prenom || !nom || !idutilisateur) {
      navigate('/connexion');
    }
  }, []);
  console.log(idutilisateur);
  console.log(prenom);
  console.log(nom);
  const [dateDemarrage, setDateDemarrage] = useState(null);
  const handleDateChange = (date) => {
    setDateDemarrage(date);
  };

  const updateDisponibilites = (disponibilitesData) => {
    axios.put(`http://localhost:5035/api/utilisateur/UpdateDisponibilités/${idutilisateur}`, disponibilitesData)
      .then(response => {
        console.log("Disponibilités mises à jour avec succès :", response.data);
      })
      .catch(error => {
        console.error("Erreur lors de la mise à jour des disponibilités :", error);
      });
  };

  const creerTache = (tachesData) => {
    axios.post(`http://localhost:5035/api/utilisateur/${idutilisateur}/tache`, tachesData)
      .then(response => {
        console.log("Tâches créées avec succès :", response.data);
      })
      .catch(error => {
        console.error("Erreur lors de la création des tâches :", error);
      });
  };

  const genererTodos = (dateDemarrage) => {
    // Formatage de la date au format "dd-MM-yyyy"
    const formattedDate = moment(dateDemarrage).format("dd-MM-yyyy");

    // Envoyer la requête POST avec les données formatées
    axios.post(`http://localhost:5035/api/utilisateur/${idutilisateur}/genererTodos`, formattedDate)
        .then(response => {
            console.log("Todos générés avec succès :", response.data);
            return response.data;
        })
        .catch(error => {
            console.error('Erreur lors de la génération des Todos:', error);
            throw error;
        });
};









  
  

  const handleCreate = () => {
    const lundiInput = document.getElementById('lundi');
    const mardiInput = document.getElementById('mardi');
    const mercrediInput = document.getElementById('mercredi');
    const jeudiInput = document.getElementById('jeudi');
    const vendrediInput = document.getElementById('vendredi');
    const samediInput = document.getElementById('samedi');
    const dimancheInput = document.getElementById('dimanche');

    // Vérifier si les éléments existent avant d'accéder à leurs valeurs

    const disponibilitesData = [
      parseInt(lundiInput.value),
      parseInt(mardiInput.value),
      parseInt(mercrediInput.value),
      parseInt(jeudiInput.value),
      parseInt(vendrediInput.value),
      parseInt(samediInput.value),
      parseInt(dimancheInput.value)
    ];

    updateDisponibilites(disponibilitesData);

    // Appeler la fonction creerTache dans la boucle
    for (let i = 0; i < nombreTaches; i++) {
      const nomInput = document.getElementById(`nomTache${i}`);
      const dureeInput = document.getElementById(`dureeTache${i}`);
      const deadlineInput = document.getElementById(`deadlineTache${i}`);

      // Vérifier si les éléments existent avant d'accéder à leurs valeurs
      if (nomInput && dureeInput && deadlineInput) {
        const tacheData = {
          nom: nomInput.value,
          duree: dureeInput.value,
          deadline: deadlineInput.value
        };

        // Appeler la fonction creerTache avec les données de la tâche actuelle
        creerTache(tacheData);
      }
    }

    if (dateDemarrage) {
      const formattedDate = moment(dateDemarrage).format("dd-MM-yyyy");
      genererTodos(formattedDate);
  }  
  };

  const ajouterTache = () => {
    setNombreTaches(nombreTaches + 1);
  };

  return (
    <div>
      <h1>Générer un planning</h1>
      <div>
        Veuillez rentrer le nombre d'heure disponible pour chaque jour de la semaine :
        <br />
        <br />
        Lundi
        <input id="lundi"></input>
        <br />
        Mardi
        <input id="mardi"></input>
        <br />
        Mercredi
        <input id="mercredi"></input>
        <br />
        Jeudi
        <input id="jeudi"></input>
        <br />
        Vendredi
        <input id="vendredi"></input>
        <br />
        Samedi
        <input id="samedi"></input>
        <br />
        Dimanche
        <input id="dimanche"></input>
        <br />
        <br />
        Veuillez rentrer les informations de vos différentes tâches
        <br />
        <br />
        {/* Entrées pour la première tâche */}
        {[...Array(nombreTaches)].map((_, index) => (
          <TacheInput key={index} index={index + 1} />
        ))}
        {/* Bouton pour ajouter une nouvelle tâche */}
        <button onClick={ajouterTache}>+</button>
        <br />
        <br />
        Sélectionnez une date de démarrage :
        <DatePicker
          selected={dateDemarrage}
          onChange={handleDateChange}
          dateFormat="dd-MM-yyyy"
          placeholderText="Sélectionnez une date de démarrage"
        />
      </div>
      {/* Bouton pour mettre à jour les disponibilités et créer les tâches */}
      <button onClick={handleCreate}>Créer</button>
      <Navbar />
    </div>
  );
};

export default CreerPlanning;
