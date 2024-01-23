// Inscription.js
import React, { useState } from 'react';
import axios from 'axios';

const Inscription = () => {
  const [userData, setUserData] = useState({});

  const handleRegistration = async () => {
    try {
      const response = await axios.post('http://localhost:5035/api/utilisateur/Inscription', userData);
      console.log(response.data); // Afficher ou traiter la réponse
    } catch (error) {
      console.error("Erreur lors de l'inscription", error);
    }
  };

  return (
    <div>
      <h1>Page d'Inscription</h1>
      <input type="text" placeholder="Nom" onChange={(e) => setUserData({ ...userData, Nom: e.target.value })} />
      <input type="text" placeholder="Prénom" onChange={(e) => setUserData({ ...userData, Prenom: e.target.value })} />
      <input type="email" placeholder="Email" onChange={(e) => setUserData({ ...userData, Email: e.target.value })} />
      <input type="password" placeholder="Mot de passe" onChange={(e) => setUserData({ ...userData, Password: e.target.value })} />
      <button onClick={handleRegistration}>S'inscrire</button>
    </div>
  );
};

export default Inscription;
