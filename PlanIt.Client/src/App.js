// App.js
import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Accueil from './Accueil';
import Inscription from './Inscription';
import Connexion from './Connexion';

const App = () => {
  return (
    <Router>
      <Routes>
        <Route path="/inscription" element={<Inscription />} />
        <Route path="/connexion" element={<Connexion />} />
        <Route path="/" element={<Accueil />} />
      </Routes>
    </Router>
  );
};

export default App;
