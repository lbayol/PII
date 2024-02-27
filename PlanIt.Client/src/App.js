// App.js
import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import { Inscription } from './Components/Inscription';
import { Connexion } from './Components/Connexion';
import { Compte } from './Components/Compte';
import { CreerPlanning } from './Components/CreerPlanning';
import { Planning } from './Components/Planning';
import Deconnexion from './Components/Deconnexion';

const App = () => {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Inscription />} />
        <Route path="/connexion" element={<Connexion />} />
        <Route path="/compte" element={<Compte />} />
        <Route path="/creerplanning" element={<CreerPlanning />} />
        <Route path="/planning" element={<Planning />} />
        <Route path="/deconnexion" element={<Deconnexion />} />
      </Routes>
    </Router>
  );
};

export default App;
