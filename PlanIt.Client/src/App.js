// App.js
import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import { Inscription } from './Components/Inscription';
import { Connexion } from './Components/Connexion';
import { Compte } from './Components/Compte';

const App = () => {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Inscription />} />
        <Route path="/connexion" element={<Connexion />} />
        <Route path="/compte" element={<Compte />} />
      </Routes>
    </Router>
  );
};

export default App;
