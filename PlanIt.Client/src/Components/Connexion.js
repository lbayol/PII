import "../Styles/Connexion.css";
import forme from '../img/forme-1.png';
import forme2 from '../img/forme1-1.png';
import immail from '../img/mailoutlined.svg';
import imcadenas from '../img/cadenas.svg';
import imfleche from '../img/arrowrightoutlined.svg';
import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import axios from 'axios';

export const Connexion = () => {
  const [email, setMail] = useState('');
  const [password, setMdp] = useState('');
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

    setErrors(errorsCopy);

    if (Object.values(errorsCopy).some((error) => error !== '')) {
      return;
    }

    try {
      const response = await axios.post('http://localhost:5035/api/utilisateur/Connexion', {
        Email: email,
        Password: password,
      });
      console.log(response.data);
      const userInfoResponse = await axios.get(`http://localhost:5035/api/utilisateur/infosConnexion?email=${email}`);
      console.log(userInfoResponse.data);
      const Prenom = userInfoResponse.data.prenom;
      const Nom = userInfoResponse.data.nom;
      const Disponibilites = userInfoResponse.data.disponibilites;
      const Todos = userInfoResponse.data.todos;
      const Taches = userInfoResponse.data.taches;
      const IdUtilisateur = userInfoResponse.data.utilisateurId;
      console.log(userInfoResponse);
      console.log(userInfoResponse.data);
      console.log(Prenom);
      console.log(Nom);
      if (Prenom && Nom) {
        setMail('');
        setMdp('');
        setErrors({ email: '', password: '' });

        // Stoker les informations dans le localStorage
        localStorage.setItem('prenom', Prenom);
        localStorage.setItem('nom', Nom);
        localStorage.setItem('disponibilites', Disponibilites);
        localStorage.setItem('todos', Todos);
        localStorage.setItem('taches', Taches);
        localStorage.setItem('idutilisateur', IdUtilisateur);

        navigate('/compte'); // Rediriger vers la page du compte
      } else {
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
                  <input
                    type="password"
                    placeholder="Mot de passe"
                    className={`rectangle text-wrapper-9`}
                    value={password}
                    onChange={(e) => setMdp(e.target.value)}
                  />
                  <img className="img-2" alt="Cadenas" src={imcadenas} />
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
