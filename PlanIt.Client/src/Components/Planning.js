import React, {useEffect} from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Navbar from "./Navbar";

export const Planning = () => {
    const prenom = localStorage.getItem('prenom');
    const nom = localStorage.getItem('nom');
    const disponibilites = localStorage.getItem('disponibilites');
    const todos = localStorage.getItem('todos');
    const taches = localStorage.getItem('taches');
    const idutilisateur = localStorage.getItem('idutilisateur');
    const navigate = useNavigate();

    useEffect(() => {
        if (!prenom || !nom) {
        navigate('/connexion');
        }
    }, []);
    if(todos.length===0)
    {
        return(
            <div>
                <h3>Vous n'avez pas encore créer de planning</h3>
                <Link to="/creerplanning">Créer un planning</Link>
                <Navbar/>
            </div>
        );
    }
    else{
        return (
            <div>
              <h1>Votre planning</h1>
              <Navbar/>
            </div>
        );
    }
};