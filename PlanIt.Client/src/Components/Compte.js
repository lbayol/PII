import React, {useEffect} from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Navbar from "./Navbar";

export const Compte = () => {
  const prenom = localStorage.getItem('prenom');
  const nom = localStorage.getItem('nom');
  const disponibilites = localStorage.getItem('disponibilites');
  const todos = localStorage.getItem('todos');
  const taches = localStorage.getItem('taches');
  const idutilisateur = localStorage.getItem('idutilisateur');
  const navigate = useNavigate();

  useEffect(() => {
    if (!prenom || !nom) {
      navigate('/connexion');
    }
  }, []);

  return (
    <div>
      <h1>Bienvenue sur votre compte, {prenom} {nom}, id : {idutilisateur}!</h1>
      <Link to="/deconnexion">Se déconnecter</Link>
      <Navbar/>
    </div>
  );
};
