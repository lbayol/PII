import React, {useEffect} from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Navbar from "./Navbar";
import "../Styles/Compte.css";

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
    <div>
      <div classname="text-wrapper">PlanIt</div>
      <br/>
      <div className='text-wrapper-2'>Bienvenue</div>
      <div>{prenom}</div>
      <br/>
      <div>{nom}</div>
      <br/>
      <div>Organisation parfaite ? Déconnectez-vous juste ici.</div>
      <Link to="/deconnexion">Se déconnecter</Link>
      <Navbar/>
    </div>
  );
};
