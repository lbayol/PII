// Connexion.js
import React, { useState } from 'react';
import axios from 'axios';

const Connexion = () => {
  const [loginData, setLoginData] = useState({});

  const handleLogin = async () => {
    try {
      const response = await axios.post('http://votre-backend.com/api/utilisateur/Connexion', loginData);
      console.log(response.data); // Afficher ou traiter la réponse
    } catch (error) {
      console.error('Erreur lors de la connexion', error);
    }
  };

  return (
    <div>
      <h1>Page de Connexion</h1>
      <input type="email" placeholder="Email" onChange={(e) => setLoginData({ ...loginData, email: e.target.value })} />
      <input type="password" placeholder="Mot de passe" onChange={(e) => setLoginData({ ...loginData, password: e.target.value })} />
      <button onClick={handleLogin}>Se connecter</button>
    </div>
  );
};

export default Connexion;
