import React, {useEffect} from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Navbar from "./Navbar";

export const CreerPlanning = () => {
    const prenom = localStorage.getItem('prenom');
    const nom = localStorage.getItem('nom');
    const navigate = useNavigate();

    useEffect(() => {
        if (!prenom || !nom) {
        navigate('/connexion');
        }
    }, []);
    return (
        <div>
          <h1>Générer un planning</h1>
          <Navbar/>
        </div>
    );
};