// Accueil.js
import React from 'react';
import { Link } from 'react-router-dom';

const Accueil = () => {
  return (
    <div>
      <h1>Page d'Accueil</h1>
      <Link to="/inscription">S'inscrire</Link>
      <br />
      <Link to="/connexion">Se connecter</Link>
    </div>
  );
};

export default Accueil;
