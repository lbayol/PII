import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

const Deconnexion = () => {
  const navigate = useNavigate();

  useEffect(() => {
    // Effacez les informations de connexion du localStorage
    localStorage.removeItem('prenom');
    localStorage.removeItem('nom');

    // Redirigez vers la page de connexion après la déconnexion
    navigate('/connexion');
  }, [navigate]);

  return (
    <div>
      <p>Déconnexion en cours...</p>
    </div>
  );
};

export default Deconnexion;
