import "../Styles/Compte.css";
import forme from '../img/forme-1.png';
import forme2 from '../img/forme1-1.png';
import React, { useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import axios from 'axios';

export const Compte = (props) => {
    const location = useLocation();
    console.log(location?.state); // Utilisation de l'opérateur ?. pour vérifier si props.location est défini
    const { prenom, nom } = location?.state || {}; // Utilisation de l'opérateur ?. pour vérifier si props.location.state est défini
    console.log(prenom); // Vérifiez la valeur de prenom
    console.log(nom); // Vérifiez la valeur de nom
    return (
      <div>
        <h1>Bienvenue sur votre compte, {prenom} {nom}!</h1>
        {/* Autres contenus de la page Compte */}
      </div>
    );
};

  


