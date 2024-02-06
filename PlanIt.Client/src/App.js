// App.js
import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import { Inscription } from './Components/Inscription';
import { Connexion } from './Components/Connexion';

const App = () => {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Inscription />} />
        <Route path="/connexion" element={<Connexion />} />
      </Routes>
    </Router>
  );
};

export default App;
