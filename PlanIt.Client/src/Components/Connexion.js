import "../Styles/Connexion.css";
import forme from '../img/forme-1.png';
import forme2 from '../img/forme1-1.png';
import imuser from '../img/useroutlined.svg';
import imnom from '../img/nom.svg';
import immail from '../img/mailoutlined.svg';
import imcadenas from '../img/cadenas.svg';
import imfleche from '../img/arrowrightoutlined.svg';
import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import axios from 'axios';


export const Connexion = () => {
  const [email, setMail] = useState('');
  const [password, setMdp] = useState('');
  const [nom, setNom] = useState('');
  const [prenom, setPrenom] = useState('');
  
  const [errors, setErrors] = useState({
    email: '',
    password: '',
  });
  const navigate = useNavigate();

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
      return;
    }

    try {
      const response = await axios.post('http://localhost:5035/api/utilisateur/Connexion', {
        Email: email,
        Password: password,
      });
      console.log(response.data);
      const userInfoResponse = await axios.get(`http://localhost:5035/api/utilisateur/Utilisateur/${email}`);
      const Prenom = userInfoResponse.data.prenom;
      const Nom = userInfoResponse.data.nom;
      console.log(userInfoResponse);
      console.log(userInfoResponse.data);
      console.log(Prenom);
      console.log(Nom);
      if (Prenom && Nom) {
      setMail('');
      setMdp('');
      setErrors({ email: '', password: '' });
      navigate('/compte', { state: { prenom: Prenom, nom: Nom } });
      console.log({ prenom: Prenom, nom: Nom });
      } else{
        console.error("Les données de prénom et de nom ne sont pas disponibles.");
      } 
    } catch (error) {
      if (error.response && error.response.status === 400) {
        setErrors((prevErrors) => ({ ...prevErrors, password: 'L\'adresse e-mail ou le mot de passe est incorrect' }));
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
        {errors.email && <div className="error-message">{errors.email}</div>}
      </div>

      <div className="bouton-mail-mdp">
        <div className="overlap-group-2">
          <input
            type="password"
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

