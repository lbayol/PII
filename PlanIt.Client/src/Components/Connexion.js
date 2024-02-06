import "../Styles/Connexion.css";
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


export const Connexion = () => {
  const [email, setMail] = useState('');
  const [password, setMdp] = useState('');
  
  const [errors, setErrors] = useState({
    email: '',
    password: '',
  });
const [showError, setShowError] = useState(false);


  const handleRegistration = async () => {
    const errorsCopy = {
      email: '',
      password: '',
    };

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
      const response = await axios.post('http://localhost:5035/api/utilisateur/Connexion', {
        Email: email,
        Password: password,
      });

      console.log(response.data);
      setMail('');
      setMdp('');
      setShowError(false);
    } catch (error) {
      if (error.response && error.response.status === 400) {
        setErrors((prevErrors) => ({ ...prevErrors, email: 'Un utilisateur avec cet email est déjà inscrit' }));
        setShowError(true);
      } else {
        console.error("Erreur lors de la connexion", error);
      }
    }
  };
  return (
    <div className="connexion">
      <div className="div">
        <div className="overlap">
          <div className="text-wrapper">PlanIt</div>
          <div className="overlap-group">
            <div className="text-wrapper-2">Bienvenue</div>
            <img className="forme" alt="Forme" src={forme} />
          </div>
        </div>
        <div className="text-wrapper-3">Je me connecte</div>
        <div className="overlap-2">
          <div className="overlap-3">
            <div className="text-wrapper-4">Se connecter</div>
            <img className="img" alt="Forme" src={forme2} />
            <div className="frame">
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
        <Link to="/" className="text-wrapper-7">S'inscrire</Link>
        <div className="text-wrapper-8">Pas de compte ?</div>
      </div>
    </div>
  );
};

