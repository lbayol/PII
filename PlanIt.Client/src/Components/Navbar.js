// Navbar effectuée avec mui

import * as React from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import Box from '@mui/material/Box';
import BottomNavigation from '@mui/material/BottomNavigation';
import BottomNavigationAction from '@mui/material/BottomNavigationAction';
import PersonRoundedIcon from '@mui/icons-material/PersonRounded';
import AddCircleRoundedIcon from '@mui/icons-material/AddCircleRounded';
import CalendarMonthRoundedIcon from '@mui/icons-material/CalendarMonthRounded';
import '../Styles/Navbar.css'; 

export default function SimpleBottomNavigation() {
  const navigate = useNavigate();
  const location = useLocation();
  const [value, setValue] = React.useState(location.pathname);

  const handleChange = (event, newValue) => {
    setValue(newValue);
    navigate(newValue);
  };

  // Mettre à jour la valeur lorsque la navigation est confirmée
  React.useEffect(() => {
    setValue(location.pathname);
  }, [location.pathname]);

  return (
    <Box sx={{ width: 500 }}>
      <BottomNavigation
        showLabels
        value={value}
        onChange={handleChange}
        className='bottom-nav'
      >
        <BottomNavigationAction 
          label="Mon planning" 
          icon={<CalendarMonthRoundedIcon />} 
          value="/planning"
          component={Link}
          to="/planning"
          className={value === '/planning' ? 'active' : 'inactive'}
        />
        <BottomNavigationAction 
          label="Créer un planning" 
          icon={<AddCircleRoundedIcon />} 
          value="/creerplanning"
          component={Link}
          to="/creerplanning"
          className={value === '/creerplanning' ? 'active' : 'inactive'}
        />
        <BottomNavigationAction 
          label="Mon Compte" 
          icon={<PersonRoundedIcon />} 
          value="/compte"
          component={Link}
          to="/compte"
          className={value === '/compte' ? 'active' : 'inactive'}
        />
      </BottomNavigation>
    </Box>
  );
}
