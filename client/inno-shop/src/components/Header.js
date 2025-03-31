import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';

import './Header.css';

const Header = ({ user, onLogout }) => {
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem('accessToken');
    onLogout();
    navigate('/');
  };

  return (
    <header className="header">
      <h1>InnoShop</h1>
      <nav>
        <ul>
          <li><Link to="/">Home</Link></li>
          <li><Link to="/products">Catalog</Link></li>
          {!user ? (
            <li><Link to="/auth">Login/Register</Link></li>
          ) : (
            <li>
              <button style={{ display: "block" , textAlign: "center",
    marginTop: "15p",
    background: "none",
    border: "none",
    color: "#3498db",
    cursor: "pointer",
    fontSize: "20px",

  }}onClick={handleLogout}>Logout</button>
            </li>
          )}
          {user && (
            <li><Link to="/admin">Admin</Link></li>
          )}
          {user && (
            <li><Link to="/user/settings">User Settings</Link></li>
          )}
          <li><Link to="/user/products">My Products</Link></li>

        </ul>
      </nav>
    </header>
  );
};


export default Header;
