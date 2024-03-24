// Fichier .js de la page du compte, permettant de savoir sur quel compte on est et de se déconneter.

import React, {useEffect} from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Navbar from "./Navbar";
import "../Styles/Compte.css";
import forme from '../img/forme-1.png';
import forme2 from '../img/forme1-1.png';
import forme3 from '../img/forme3.png';
import forme4 from '../img/forme4.png';

export const Compte = () => {
  const prenom = localStorage.getItem('prenom');
  const nom = localStorage.getItem('nom');
  const disponibilites = JSON.parse(localStorage.getItem('disponibilites'));
  const todos = JSON.parse(localStorage.getItem('todos'));
  const taches = JSON.parse(localStorage.getItem('taches'));
  const idutilisateur = localStorage.getItem('idutilisateur');
  const email = localStorage.getItem('email');
  const navigate = useNavigate();

  useEffect(() => {
    if (!prenom || !nom) {
      navigate('/connexion');
    }
  }, []);

  return (
    
    <div className='compte'>
      <div className="text-wrapper">PlanIt</div>
      <div className='text-wrapper-2'>Bienvenue</div>
      <div className='text-wrapper-3'>{prenom}</div>
      <div className='text-wrapper-3'>{nom}</div>
      <div className='text-wrapper-4'>Parfaitement organisé ? Déconnectez-vous juste ici.</div>
      <Link to="/deconnexion" className='vector-wrapper'>Se déconnecter</Link>
      <img className="forme" alt="Forme" src={forme4} />
      <Navbar/>
    </div>
  );
};
