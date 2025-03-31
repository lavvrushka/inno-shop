import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Layout from './components/Layout';
import Home from './pages/Home';
import ProductListPage from './pages/ProductListPage';
import ProductDetailsPage from './pages/ProductDetailsPage';
import AuthPage from './pages/AuthPage';
import UserProductsPage from './pages/UserProductsPage';
import UserSettings from './pages/UserSettings';
import ConfirmEmail from './pages/ConfirmEmail';
import ManagementPage from './pages/ManagementPage'; 

const App = () => {
  const [user, setUser] = useState(null); 

  useEffect(() => {
    const storedUser = localStorage.getItem('accessToken');
    if (storedUser) {
      setUser(true); 
    }
  }, []);

  const handleLogin = (userData) => {
    setUser(userData);
    localStorage.setItem('accessToken', userData.accessToken);
  };

  const handleLogout = () => {
    setUser(null);
    localStorage.removeItem('accessToken');
  };

  return (
    <Router>
      <Layout user={user} onLogout={handleLogout}>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/confirm-email/:token" element={<ConfirmEmail />} />
          <Route path="/products" element={<ProductListPage />} />
          <Route path="/products/:productId" element={<ProductDetailsPage />} />
          <Route
            path="/auth"
            element={<AuthPage onLogin={handleLogin} />}
          />
          <Route
            path="/user/products"
            element={user ? <UserProductsPage /> : <AuthPage onLogin={handleLogin} />}
          />

          <Route
            path="/user/settings"
            element={user ? <UserSettings /> : <AuthPage onLogin={handleLogin} />}
          />
           <Route
            path="/admin"
            element={user ? <ManagementPage /> : <AuthPage onLogin={handleLogin} />}
          />

        </Routes>
      </Layout>
    </Router>
  );
};

export default App;
