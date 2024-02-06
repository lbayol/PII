import "../Styles/Inscription.css";
import forme from '../img/forme-1.png';
import forme2 from '../img/forme1-1.png';
import imuser from '../img/useroutlined.svg';
import imnom from '../img/nom.svg';
import immail from '../img/mailoutlined.svg';
import imcadenas from '../img/cadenas.svg';
import imfleche from '../img/arrowrightoutlined.svg';
import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import axios from 'axios';


export const Inscription = () => {
  const [nom, setNom] = useState('');
  const [prenom, setPrenom] = useState('');
  const [email, setMail] = useState('');
  const [password, setMdp] = useState('');
  
  const [errors, setErrors] = useState({
    nom: '',
    prenom: '',
    email: '',
    password: '',
  });
const [showError, setShowError] = useState(false);


  const handleRegistration = async () => {
    // Créer une copie locale des erreurs
    const errorsCopy = {
      nom: '',
      prenom: '',
      email: '',
      password: '',
    };

    // Vérifier les champs et mettre à jour les erreurs dans la copie locale
    if (!nom) {
      errorsCopy.nom = 'Veuillez entrer votre nom';
    }

    if (!prenom) {
      errorsCopy.prenom = 'Veuillez entrer votre prénom';
    }

    if (!email) {
      errorsCopy.email = 'Veuillez entrer votre adresse e-mail';
    } else if (!/\S+@\S+\.\S+/.test(email)) {
      errorsCopy.email = 'Veuillez entrer une adresse e-mail valide';
    }

    if (!password) {
      errorsCopy.password = 'Veuillez entrer votre mot de passe';
    }

    // Mettre à jour les erreurs
    setErrors(errorsCopy);

    // Vérifier s'il y a des erreurs et afficher le message d'erreur si nécessaire
    if (Object.values(errorsCopy).some((error) => error !== '')) {
      setShowError(true);
      return;
    }

    try {
      const response = await axios.post('http://localhost:5035/api/utilisateur/Inscription', {
        Nom: nom,
        Prenom: prenom,
        Email: email,
        Password: password,
      });

      console.log(response.data);
      setNom('');
      setPrenom('');
      setMail('');
      setMdp('');
      setShowError(false);
      window.location.href = '/connexion';
    } catch (error) {
      if (error.response && error.response.status === 400) {
        setErrors((prevErrors) => ({ ...prevErrors, email: 'Un utilisateur avec cet email est déjà inscrit' }));
        setShowError(true);
      } else {
        console.error("Erreur lors de l'inscription", error);
      }
    }
  };
  return (
    <div className="inscription">
      <div className="div">
        <div className="overlap">
          <div className="text-wrapper">PlanIt</div>
          <div className="overlap-group">
            <div className="text-wrapper-2">Bienvenue</div>
            <img className="forme" alt="Forme" src={forme} />
          </div>
        </div>
        <div className="text-wrapper-3">Je m’inscris</div>
        <div className="overlap-2">
          <div className="overlap-3">
            <div className="text-wrapper-4">S’inscrire</div>
            <img className="img" alt="Forme" src={forme2} />
            <div className="frame">
      <div className="bouton-mail-mdp">
        <input
          type="text"
          placeholder="Prénom"
          className={`rectangle text-wrapper-9`}
          value={prenom}
          onChange={(e) => setPrenom(e.target.value)}
        />
        <img className="prenom" alt="Prenom" src={imuser} />
        {errors.prenom && <div className="error-message">{errors.prenom}</div>}
      </div>

      <div className="bouton-mail-mdp">
        <input
          type="text"
          placeholder="Nom"
          className={`rectangle text-wrapper-9`}
          value={nom}
          onChange={(e) => setNom(e.target.value)}
        />
        <img className="img-2" alt="Nom" src={imnom} />
        {errors.nom && <div className="error-message">{errors.nom}</div>}
      </div>

      <div className="bouton-mail-mdp">
        <input
          type="text"
          placeholder="votre.email@exemple.com"
          className={`rectangle text-wrapper-9`}
          value={email}
          onChange={(e) => setMail(e.target.value)}
        />
        <img className="mail-outlined" alt="Mail outlined" src={immail} />
        {showError && <div className="error-message">{errors.email}</div>}
      </div>

      <div className="bouton-mail-mdp">
        <div className="overlap-group-2">
          <input
            type="text"
            placeholder="Mot de passe"
            className={`rectangle text-wrapper-9`}
            value={password}
            onChange={(e) => setMdp(e.target.value)}
          />
          <img className="img-2" alt="Cadenas" src={imcadenas} />
        </div>
        {errors.password && <div className="error-message2">{errors.password}</div>}
      </div>
    </div>
          </div>
          <button className="vector-wrapper" onClick={handleRegistration}>
            <img className="vector" alt="Vector" src={imfleche} />
          </button>
        </div>
        <Link to="/connexion" className="text-wrapper-7">Se connecter</Link>
        <div className="text-wrapper-8">Déjà un compte ?</div>
      </div>
    </div>
  );
};

