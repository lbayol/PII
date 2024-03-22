import React, { useState, useEffect } from 'react';
import axios from 'axios';
import moment from 'moment';
import Navbar from "./Navbar";
import { Link, useNavigate } from 'react-router-dom';
import "../Styles/CreerPlanning.css";

const TacheInput = ({ index }) => (
  <div>
    <div className='text-wrapper-5'>
    Tâche {index} :
    </div>
    <div className='bouton-nom'>
    <input id={`nomTache${index}`} placeholder='Nom' className='text-wrapper-9'/>
    </div>
    <div className='bouton-duree'>
    <input id={`dureeTache${index}`} placeholder='Durée (en h)' className='text-wrapper-9'/>
    </div>
    <div className='bouton-deadline'>
    <input id={`deadlineTache${index}`} placeholder=" Deadline (dd-mm-yyyy)" className='text-wrapper-9'/>
    </div>
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
  </div>
);

const isValidDate = (dateString) => {
  const dateRegex = /^\d{2}-\d{2}-\d{4}$/;

  if (!dateRegex.test(dateString)) {
    return false;
  }

  const [day, month, year] = dateString.split('-').map(Number);

  if (month < 1 || month > 12 || day < 1) {
    return false;
  }

  const daysInMonth = new Date(year, month, 0).getDate();

  return day <= daysInMonth;
};

export const CreerPlanning = () => {
  const [errorMessage, setErrorMessage] = useState("");
  const [nombreTaches, setNombreTaches] = useState(1);
  const [dateDemarrage, setDateDemarrage] = useState("");
  const prenom = localStorage.getItem('prenom');
  const nom = localStorage.getItem('nom');
  const email = localStorage.getItem('email');
  const idutilisateur = parseInt(localStorage.getItem('idutilisateur'));
  const navigate = useNavigate();

  useEffect(() => {
    if (!prenom || !nom || !idutilisateur) {
      // Rediriger vers la page de connexion si les informations ne sont pas disponibles
      window.location.href = '/connexion';
    }
  }, []);

  const updateDisponibilites = (disponibilitesData) => {
    return new Promise((resolve, reject) => {
        axios.put(`http://localhost:5035/api/utilisateur/UpdateDisponibilités/${idutilisateur}`, disponibilitesData)
            .then(response => {
                console.log("Disponibilités mises à jour avec succès :", response.data);
                resolve(response.data);
            })
            .catch(error => {
                console.error("Erreur lors de la mise à jour des disponibilités :", error);
                reject(error);
            });
    });
};


  const creerTache = (tachesData) => {
    console.log("tacheData :", tachesData);
    return axios.post(`http://localhost:5035/api/utilisateur/${idutilisateur}/tache`, tachesData)
      .then(response => {
        console.log("Tâches créées avec succès :", response.data);
        return response.data; // retourner les données de la tâche créée
      })
      .catch(error => {
        console.error("Erreur lors de la création des tâches :", error);
        throw error; // propager l'erreur pour la gérer plus tard si nécessaire
      });
  };

  const genererTodos = (dateDemarrage) => {
    return new Promise((resolve, reject) => {
        const formattedDate = moment(dateDemarrage, "DD-MM-YYYY").format("DD-MM-YYYY");
        axios.post(`http://localhost:5035/api/utilisateur/${idutilisateur}/genererTodos`, `"${formattedDate}"`, {
            headers: {
                'Content-Type': 'application/json'
            }
        })
        .then(response => {
            console.log("Todos générés avec succès :", response.data);
            resolve(response.data);
        })
        .catch(error => {
            console.error('Erreur lors de la génération des Todos:', error);
            reject(error);
        });
    });
};

const calculerHeuresDisponibles = (idUtilisateur) => {
  return new Promise((resolve, reject) => {
    axios.put(`http://localhost:5035/api/utilisateur/CalculerHeuresDisponibles?idUtilisateur=${idUtilisateur}`)
      .then(response => {
        console.log("Le nombre d'heures disponibles de l'utilisateur a été mis à jour :", response.data);
        resolve(response.data);
      })
      .catch(error => {
        console.error("Erreur lors de la mise à jour des heures disponibles de l'utilisateur :", error);
        reject(error);
      });
  });
};

const rafraichirNoteRates = (idUtilisateur) => {
  return new Promise((resolve, reject) => {
    axios.put(`http://localhost:5035/api/utilisateur/RafraichirNoteRates?idUtilisateur=${idUtilisateur}`)
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



const supprimerTodos = async (utilisateurId) => {
  try {
    const response = await axios.delete(`http://localhost:5035/api/utilisateur/${utilisateurId}/supprimerTodos`);
    console.log(response.data);
  } catch (error) {
    console.error("Erreur lors de la suppression des Todos :", error);
    throw error;
  }
};

  const handleCreate = async () => {
    const lundiInput = document.getElementById('lundi');
    const mardiInput = document.getElementById('mardi');
    const mercrediInput = document.getElementById('mercredi');
    const jeudiInput = document.getElementById('jeudi');
    const vendrediInput = document.getElementById('vendredi');
    const samediInput = document.getElementById('samedi');
    const dimancheInput = document.getElementById('dimanche');
    const inputsNomTache = document.querySelectorAll('[id^=nomTache]');
    const inputsDuréeTache = document.querySelectorAll('[id^=dureeTache]');
    const inputsDeadlineTache = document.querySelectorAll('[id^=deadlineTache]');
    let dureeTotaleTaches = 0;

    const validateInputs = (inputs) => {
        for (let i = 0; i < inputs.length; i++) {
            if (!Number.isInteger(parseInt(inputs[i].value))) {
                setErrorMessage("Veuillez saisir des nombres entiers dans tous les champs.");
                return false;
            }
        }
        return true;
    };

    if (!dateDemarrage) {
        setErrorMessage("Veuillez saisir une date de démarrage.");
        return;
    }

    if (!isValidDate(dateDemarrage)) {
        setErrorMessage("Format de date incorrect pour la date de démarrage. Veuillez utiliser le format dd-mm-yyyy.");
        return;
    } else {
        setErrorMessage("");
    }

    if (!validateInputs([lundiInput, mardiInput, mercrediInput, jeudiInput, vendrediInput, samediInput, dimancheInput, ...inputsDuréeTache,])) {
        setErrorMessage("Veuillez saisir des nombres entiers dans tous les champs.");
        return;
    } else {
        setErrorMessage("");
    }

    for (let i = 1; i <= nombreTaches; i++) {
        const nomInput = document.getElementById(`nomTache${i}`);
        if (!nomInput.value.trim()) {
            setErrorMessage("Veuillez remplir tous les champs 'Nom' des tâches.");
            return;
        }
    }

    const tachesTriees = [];
    for (let i = 1; i <= nombreTaches; i++) {
        const tache = {
            nom: document.getElementById(`nomTache${i}`).value,
            duree: parseInt(document.getElementById(`dureeTache${i}`).value),
            deadline: moment(document.getElementById(`deadlineTache${i}`).value, "DD-MM-YYYY").toDate()
        };
        tachesTriees.push(tache);
    }
    tachesTriees.sort((a, b) => a.deadline - b.deadline);
    console.log("taches triées : ",tachesTriees);

    const disponibilites = [parseInt(lundiInput.value), parseInt(mardiInput.value), parseInt(mercrediInput.value), parseInt(jeudiInput.value), parseInt(vendrediInput.value), parseInt(samediInput.value), parseInt(dimancheInput.value)];
    for (let i = 0; i < disponibilites.length; i++) {
      if (disponibilites[i] < 0) {
          setErrorMessage(`Les disponibilités du jour ${i + 1} ne sont pas suffisantes pour les tâches.`);
          return;
      }
  }

  // Mise à jour des disponibilités
  await updateDisponibilites(disponibilites);

  for (let i = 1; i <= nombreTaches; i++) {
    const deadlineTache = moment(document.getElementById(`deadlineTache${i}`).value, "DD-MM-YYYY").toDate();
    const deadlineTacheSansHeure = new Date(deadlineTache.getFullYear(), deadlineTache.getMonth(), deadlineTache.getDate());
    const dateDemarrageObjet = moment(dateDemarrage, "DD-MM-YYYY").toDate();
    const dateDemarrageSansHeure = new Date(dateDemarrageObjet.getFullYear(), dateDemarrageObjet.getMonth(), dateDemarrageObjet.getDate());

    if (deadlineTacheSansHeure < dateDemarrageSansHeure) {
        setErrorMessage(`La deadline de la tâche ${i} est antérieure à la date de démarrage.`);
        return; // Ajout d'un retour pour sortir de la fonction en cas d'erreur
    }
}
    var jourParcouru = moment(dateDemarrage, "DD-MM-YYYY").toDate();
    var jourParcouruSansHeure = new Date(jourParcouru.getFullYear(), jourParcouru.getMonth(), jourParcouru.getDate());
    for (let i = 0; i < tachesTriees.length; i++) {
      const tache = tachesTriees[i];
      console.log(tache);
      console.log("parseInt dureetache : ", parseInt(tache.duree));
      dureeTotaleTaches += parseInt(tache.duree);
      const deadlineTache = moment(tache.deadline, "DD-MM-YYYY").toDate();
      const deadlineTacheSansHeure = new Date(deadlineTache.getFullYear(), deadlineTache.getMonth(), deadlineTache.getDate());
  }  
  for (let i = 0; i < tachesTriees.length; i++) {
    const tache = tachesTriees[i];
    const dureeTache = parseInt(tache.duree);
    const deadlineTache = moment(tache.deadline, "DD-MM-YYYY").toDate();
    var dureeRestanteTache = dureeTache;

    while (dureeRestanteTache > 0 && jourParcouruSansHeure <= deadlineTache) {
        console.log("jour : ", jourParcouruSansHeure);
        var jourSemaine = (jourParcouruSansHeure.getDay() + 6) % 7; // Ajustement pour obtenir l'index correct du Lundi au Dimanche
        console.log(jourSemaine);
        var disponibiliteJour = disponibilites[jourSemaine];
        dureeRestanteTache -= parseInt(disponibiliteJour);
        dureeTotaleTaches -= parseInt(disponibiliteJour);
        jourParcouruSansHeure.setDate(jourParcouruSansHeure.getDate() + 1);
    }
}

    console.log("dureeTotaleTaches : ",dureeTotaleTaches);

    if(dureeTotaleTaches>0)
    {
      setErrorMessage(`Les disponibilités, les deadlines ainsi que la date de démarrage rentrés ne permettent pas de créer un planning réalisable`);
      return; // Ajout d'un retour pour sortir de la fonction en cas d'erreur
    }

    // Suppression des tâches
    axios.delete(`http://localhost:5035/api/utilisateur/${idutilisateur}/taches`)
        .then(response => {
            console.log("Toutes les tâches ont été supprimées avec succès");
        })
        .catch(error => {
            console.error("Erreur lors de la suppression des tâches :", error);
        });

    const tachesDataArray = [];

    for (let i = 1; i <= nombreTaches; i++) {
        const nomInput = document.getElementById(`nomTache${i}`);
        const dureeInput = document.getElementById(`dureeTache${i}`);
        const deadlineInput = document.getElementById(`deadlineTache${i}`);

        if (nomInput && dureeInput && deadlineInput) {
            const tacheData = {
                nom: nomInput.value,
                duree: parseInt(dureeInput.value),
                deadline: deadlineInput.value,
                utilisateurId: idutilisateur
            };

            if (!isValidDate(tacheData.deadline)) {
                setErrorMessage("Format de date incorrect pour la tâche. Veuillez utiliser le format dd-mm-yyyy.");
                return;
            }

            tachesDataArray.push(tacheData);
        }
    }

    if (tachesDataArray.length !== nombreTaches) {
        setErrorMessage("Veuillez remplir toutes les deadlines des tâches.");
        return;
    }

    for (let i = 0; i < tachesDataArray.length; i++) {
        const tacheData = tachesDataArray[i];
        try {
            await creerTache(tacheData);
        } catch (error) {
            console.error("Erreur lors de la création de la tâche :", error);
            return;
        }
    }

    try {
      await supprimerTodos(idutilisateur);
    } catch (error) {
      console.error("Erreur lors de la suppression des Todos :", error);
      // Gérer l'erreur de suppression des Todos ici
    }
    
    await genererTodos(dateDemarrage);

    try {
        await calculerHeuresDisponibles(idutilisateur);
        console.log("Les heures disponibles ont été calculées avec succès");
    } catch (error) {
        console.error("Erreur lors du calcul des heures disponibles :", error);
    }

    await rafraichirNoteRates(idutilisateur);

    try {
        const userInfoResponse = await axios.get(`http://localhost:5035/api/utilisateur/infosConnexion?email=${email}`);
        const { disponibilites, todos, taches, note } = userInfoResponse.data;
        localStorage.setItem('disponibilites', JSON.stringify(disponibilites));
        localStorage.setItem('todos', JSON.stringify(todos));
        localStorage.setItem('taches', JSON.stringify(taches));
        localStorage.setItem('note', note);
        navigate('/planning');
    } catch (error) {
        console.error("Erreur lors de la récupération des informations de l'utilisateur :", error);
    }
};



  const ajouterTache = () => {
    setNombreTaches(nombreTaches + 1);
  };

  const supprimerTache = () => {
    setNombreTaches(nombreTaches - 1);
  };

  const handleDateChange = (event) => {
    setDateDemarrage(event.target.value);
  };

  return (
    <div className='creerplanning'>
      <div className='text-wrapper'>PlanIt</div>
      <div className='text-wrapper-2'>Générer un planning</div>
      <div>
        <div className='text-wrapper-3'>
        Veuillez rentrer le nombre d'heure disponible pour chaque jour de la semaine :
        </div>
        <div className="bouton-lundi">
          <input id="lundi" className={`text-wrapper-9`} placeholder='Lundi'/>
        </div>
        <div className="bouton-mardi">
          <input id="mardi" className={`text-wrapper-9`} placeholder='Mardi'/>
        </div>
        <div className="bouton-mercredi">
          <input id="mercredi" className={`text-wrapper-9`} placeholder='Mercredi'/>
        </div>
        <div className="bouton-jeudi">
          <input id="jeudi" className={`text-wrapper-9`} placeholder='Jeudi'/>
        </div>
        <div className="bouton-vendredi">
          <input id="vendredi" className={`text-wrapper-9`} placeholder='Vendredi'/>
        </div>
        <div className="bouton-samedi">
          <input id="samedi" className={`text-wrapper-9`} placeholder='Samedi'/>
        </div>
        <div className="bouton-dimanche">
          <input id="dimanche" className={`text-wrapper-9`} placeholder='Dimanche'/>
        </div>
        <div className='text-wrapper-4'>
        Veuillez rentrer les informations de vos différentes tâches : 
        </div>
        {[...Array(nombreTaches)].map((_, index) => (
          <TacheInput key={index} index={index + 1} />
        ))}
        <button onClick={ajouterTache} className='vector-wrapper-add'>+</button>
        {nombreTaches > 1 && <button onClick={supprimerTache} className='vector-wrapper-delete'>-</button>}
        <div className='text-wrapper-4'>
        Sélectionnez une date de démarrage :
        </div>
        <div className='bouton-demarrage'>
        <input
          type="text"
          value={dateDemarrage}
          onChange={handleDateChange}
          placeholder="dd-mm-yyyy"
          className='text-wrapper-9'
        />
        </div>
        <br />
        
        <button onClick={handleCreate} className='vector-wrapper-creer'>Créer</button>
        <div className='text-wrapper-6'>
        Attention, si vous possédez déjà un planning, le planning actuel sera écrasé
        </div>
        {errorMessage && <div className="error-message">{errorMessage}</div>}
      </div>
      <Navbar />
    </div>
  );
};

export default CreerPlanning;
