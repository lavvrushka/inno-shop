import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { 
  loginUser, 
  registerUser, 
  sendRecoveryEmailAccount, 
  recoverAccount,
  sendPasswordRecoveryEmail,
  resetPassword
} from '../services/api';
import './AuthPage.css'; 
import image from '../images/image.png'; 

const AuthPage = ({ onLogin }) => {
  const [mode, setMode] = useState('auth'); 
  const [isLogin, setIsLogin] = useState(true);
  const [formData, setFormData] = useState({
    firstname: '',
    lastname: '',
    email: '',
    password: '',
    confirmPassword: '',
    birthDate: '',
  });
  const [token, setToken] = useState('');
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  const validateForm = () => {
    if (mode === 'auth') {
      if (!isLogin) {
        if (!/^[a-zA-Z]+$/.test(formData.firstname)) {
          return 'First name should contain only letters';
        }
        if (!/^[a-zA-Z]+$/.test(formData.lastname)) {
          return 'Last name should contain only letters';
        }
        const birthDate = new Date(formData.birthDate);
        const age = new Date().getFullYear() - birthDate.getFullYear();
        if (age < 18) {
          return 'You must be at least 18 years old';
        }
      }
      if (!/^\S+@\S+\.\S+$/.test(formData.email)) {
        return 'Please enter a valid email address';
      }
      if (formData.password.length < 6) {
        return 'Password must be at least 6 characters long';
      }
      if (!isLogin && formData.password !== formData.confirmPassword) {
        return 'Passwords do not match';
      }
    } else if (mode === 'recoveryEmail' || mode === 'passwordRecoveryEmail') {
      if (!/^\S+@\S+\.\S+$/.test(formData.email)) {
        return 'Please enter a valid email address';
      }
    } else if (mode === 'recoveryToken') {
      if (!token) {
        return 'Enter your account recovery token';
      }
    } else if (mode === 'passwordReset') {
      if (!token) {
        return 'Enter the token to reset your password';
      }
      if (formData.password.length < 6) {
        return 'The new password must contain at least 6 characters.';
      }
      if (formData.password !== formData.confirmPassword) {
        return 'The passwords do not match';
      }
    }
    return null;
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    const validationError = validateForm();
    if (validationError) {
      setError(validationError);
      return;
    }

    
    if (mode === 'auth') {
      try {
        const requestBody = {
          firstname: formData.firstname,
          lastname: formData.lastname,
          password: formData.password,
          email: formData.email,
          birthDate: isLogin ? undefined : new Date(formData.birthDate).toISOString(),
        };

        if (isLogin) {
          const response = await loginUser({
            email: formData.email,
            password: formData.password,
          });

          if (response?.data?.accessToken) {
            localStorage.setItem('accessToken', response.data.accessToken);
          }

          onLogin(response);
          navigate('/');
        } else {
          await registerUser(requestBody);
          alert('Registration successful! Please log in.');
          setIsLogin(true);
        }
      } catch (err) {
        setError(err.response?.data?.message || 'An error occurred');
      }
    } 

    else if (mode === 'recoveryEmail') {
      try {
        await sendRecoveryEmailAccount({ email: formData.email });
        alert('Account recovery instructions have been sent to your email.');
        setMode('recoveryToken');
      } catch (err) {
        setError(err.response?.data?.message || 'Error sending email to recover account');
      }
    } 
   
    else if (mode === 'recoveryToken') {
      try {
        await recoverAccount(token);
        alert('Аккаунт успешно восстановлен! Теперь вы можете войти в систему.');
        setMode('auth');
        navigate('/auth');
      } catch (err) {
        setError(err.response?.data?.message || 'Error while restoring account');
      }
    } 

    else if (mode === 'passwordRecoveryEmail') {
      try {
        await sendPasswordRecoveryEmail({ email: formData.email });
        alert('Password reset instructions have been sent to your email.');
        setMode('passwordReset');
      } catch (err) {
        setError(err.response?.data?.message || 'Error sending email for password recovery');
      }
    } 
    else if (mode === 'passwordReset') {
      try {
        await resetPassword({ 
          token, 
          newPassword: formData.password, 
          confirmPassword: formData.confirmPassword 
        });
        alert('Password reset successfully! You can now log in.');
        setMode('auth');
        navigate('/auth');
      } catch (err) {
        setError(err.response?.data?.message || 'Ошибка при сбросе пароля');
      }
    }
    
  };

  const handleToggle = () => {
    if (mode === 'auth') {
      setIsLogin((prev) => {
        const newIsLogin = !prev;
        if (newIsLogin) {
          setFormData((prevData) => ({ ...prevData, birthDate: '' }));
        }
        return newIsLogin;
      });
    }
  };

  return (
    <div className="authContainer">
      <img src={image} alt="Auth" className="authImage" />
      <form onSubmit={handleSubmit} className="authForm">
        {mode === 'auth' && (
          <>
            <h2>{isLogin ? 'Log In' : 'Register'}</h2>
            {!isLogin && (
              <>
                <div className="inputGroup">
                  <label>First Name:</label>
                  <input
                    type="text"
                    name="firstname"
                    value={formData.firstname}
                    onChange={handleChange}
                    required
                  />
                </div>
                <div className="inputGroup">
                  <label>Last Name:</label>
                  <input
                    type="text"
                    name="lastname"
                    value={formData.lastname}
                    onChange={handleChange}
                    required
                  />
                </div>
                <div className="inputGroup">
                  <label>Birth Date:</label>
                  <input
                    type="date"
                    name="birthDate"
                    value={formData.birthDate}
                    onChange={handleChange}
                    required
                  />
                </div>
              </>
            )}
            <div className="inputGroup">
              <label>Email:</label>
              <input
                type="email"
                name="email"
                value={formData.email}
                onChange={handleChange}
                required
              />
            </div>
            <div className="inputGroup">
              <label>Password:</label>
              <input
                type="password"
                name="password"
                value={formData.password}
                onChange={handleChange}
                required
              />
            </div>
            {!isLogin && (
              <div className="inputGroup">
                <label>Confirm Password:</label>
                <input
                  type="password"
                  name="confirmPassword"
                  value={formData.confirmPassword}
                  onChange={handleChange}
                  required
                />
              </div>
            )}
          </>
        )}

        {mode === 'recoveryEmail' && (
          <>
            <h2>Account recovery</h2>
            <div className="inputGroup">
              <label>Email:</label>
              <input
                type="email"
                name="email"
                value={formData.email}
                onChange={handleChange}
                required
              />
            </div>
          </>
        )}

        {mode === 'recoveryToken' && (
          <>
            <h2> Enter your account recovery token</h2>
            <div className="inputGroup">
              <label>Токен:</label>
              <input
                type="text"
                name="token"
                value={token}
                onChange={(e) => setToken(e.target.value)}
                required
              />
            </div>
          </>
        )}

        {mode === 'passwordRecoveryEmail' && (
          <>
            <h2>Password recovery</h2>
            <div className="inputGroup">
              <label>Email:</label>
              <input
                type="email"
                name="email"
                value={formData.email}
                onChange={handleChange}
                required
              />
            </div>
          </>
        )}

        {mode === 'passwordReset' && (
          <>
            <h2>Password reset</h2>
            <div className="inputGroup">
              <label>Token:</label>
              <input
                type="text"
                name="token"
                value={token}
                onChange={(e) => setToken(e.target.value)}
                required
              />
            </div>
            <div className="inputGroup">
              <label>New password:</label>
              <input
                type="password"
                name="password"
                value={formData.password}
                onChange={handleChange}
                required
              />
            </div>
            <div className="inputGroup">
              <label>Confirm password:</label>
              <input
                type="password"
                name="confirmPassword"
                value={formData.confirmPassword}
                onChange={handleChange}
                required
              />
            </div>
          </>
        )}

        {error && <p className="errorMessage">{error}</p>}
        <button type="submit">
          {mode === 'auth'
            ? isLogin
              ? 'Log In'
              : 'Register'
            : mode === 'recoveryEmail'
            ? 'Отправить инструкции'
            : mode === 'recoveryToken'
            ? 'Восстановить аккаунт'
            : mode === 'passwordRecoveryEmail'
            ? 'Отправить инструкции'
            : 'Сбросить пароль'}
        </button>

        {mode === 'auth' && (
          <>
            <button onClick={handleToggle} className="toggleButton" type="button">
              {isLogin ? 'Register' : 'Log In'}
            </button>
            {isLogin && (
              <>
                <button
                  type="button"
                  className="toggleButton"
                  onClick={() => setMode('recoveryEmail')}
                >
                  Recover account
                </button>
                <button
                  type="button"
                  className="toggleButton"
                  onClick={() => setMode('passwordRecoveryEmail')}
                >
                  Recover password
                </button>
              </>
            )}
          </>
        )}

        {mode !== 'auth' && (
          <button
            type="button"
            className="toggleButton"
            onClick={() => setMode('auth')}
          >
            Back
          </button>
        )}
      </form>
    </div>
  );
};

export default AuthPage;
