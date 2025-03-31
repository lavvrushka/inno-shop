import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { 
  getCurrentUser, 
  updateUser, 
  deactivateUser,
  sendPasswordRecoveryEmail,
  resetPassword
} from '../services/api';
import './UserSettings.css';

const UserSettings = () => {
  const navigate = useNavigate();
  const [user, setUser] = useState(null);
  const [statusMessage, setStatusMessage] = useState('');


  const [updateForm, setUpdateForm] = useState({
    firstName: '',
    lastName: '',
    email: '',
  });
  const [updateErrors, setUpdateErrors] = useState({});

 
  const [resetPasswordForm, setResetPasswordForm] = useState({
    token: '',
    newPassword: '',
    confirmPassword: ''
  });
  const [resetPasswordErrors, setResetPasswordErrors] = useState({});

  useEffect(() => {
    loadUser();
  }, []);

  const loadUser = async () => {
    try {
      const currentUser = await getCurrentUser();
      setUser(currentUser);
      setUpdateForm({
        firstName: currentUser.firstName || '',
        lastName: currentUser.lastName || '',
        email: currentUser.email || '',
      });
    } catch (error) {
      console.error("Error fetching current user:", error);
    }
  };

 
  const handleUpdateChange = (e) => {
    const { name, value } = e.target;
    setUpdateForm(prev => ({ ...prev, [name]: value }));
  };

  const validateUpdateForm = () => {
    const errors = {};
    if (!updateForm.firstName) errors.firstName = "First name is required";
    if (!updateForm.lastName) errors.lastName = "Last name is required";
    if (!updateForm.email) errors.email = "Email is required";
    setUpdateErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleUpdateSubmit = async (e) => {
    e.preventDefault();
    if (!validateUpdateForm()) return;
    try {
      const response = await updateUser(updateForm);
      setStatusMessage(response.Message || "User data updated successfully");
      loadUser();
    } catch (error) {
      console.error("Error updating user:", error);
      setStatusMessage("Error updating user");
    }
  };


  const handleDeactivate = async () => {
    if (!user || user.emailVerifiedAt === null) {
      setStatusMessage("Please verify your email before deactivating your account.");
      return;
    }
    if (!window.confirm("Are you sure you want to deactivate your account? This action cannot be undone.")) {
      return;
    }
    try {
      const response = await deactivateUser();
      setStatusMessage(response || "Account deactivated");
      localStorage.removeItem("token");
      navigate('/auth');
    } catch (error) {
      console.error("Error deactivating user:", error);
      setStatusMessage("Error deactivating account. Please verify your email before deactivation.");
    }
  };

  const handleSendRecoveryEmail = async () => {
    if (!user || !user.email) {
      setStatusMessage('User not found or email missing');
      return;
    }

    const requestData = { email: user.email };

    try {
      const response = await sendPasswordRecoveryEmail(requestData);
      setStatusMessage(response.Message || 'Password recovery email sent');
    } catch (error) {
      console.error('Error sending password recovery email:', error);
      setStatusMessage('Error sending email');
    }
  };


  const handleResetPasswordChange = (e) => {
    const { name, value } = e.target;
    setResetPasswordForm(prev => ({ ...prev, [name]: value }));
  };

  const validateResetPasswordForm = () => {
    const errors = {};
    if (!resetPasswordForm.token) errors.token = "Token is required";
    if (!resetPasswordForm.newPassword) errors.newPassword = "New password is required";
    if (resetPasswordForm.newPassword !== resetPasswordForm.confirmPassword) errors.confirmPassword = "Passwords do not match";
    setResetPasswordErrors(errors);
    return Object.keys(errors).length === 0;
  };


  const handleResetPasswordSubmit = async (e) => {
    e.preventDefault();
    if (!validateResetPasswordForm()) return;

    const requestData = {
      Token: resetPasswordForm.token,
      NewPassword: resetPasswordForm.newPassword,
      ConfirmPassword: resetPasswordForm.confirmPassword
    };

    try {
      const response = await resetPassword(requestData);
      setStatusMessage(response.Message || "Password successfully reset");
      setResetPasswordForm({
        token: '',
        newPassword: '',
        confirmPassword: ''
      });
    } catch (error) {
      console.error('Error resetting password:', error);
      setStatusMessage('Error resetting password');
    }
  };

  return (
    <div className="user-settings">
      <h2>User Settings</h2>
      {statusMessage && <p className="status-message">{statusMessage}</p>}

      <section className="update-account">
        <h3>Update User Data</h3>
        <form onSubmit={handleUpdateSubmit}>
          <div className="form-group">
            <label>First Name:</label>
            <input
              type="text"
              name="firstName"
              value={updateForm.firstName}
              onChange={handleUpdateChange}
            />
            {updateErrors.firstName && <span className="error">{updateErrors.firstName}</span>}
          </div>
          <div className="form-group">
            <label>Last Name:</label>
            <input
              type="text"
              name="lastName"
              value={updateForm.lastName}
              onChange={handleUpdateChange}
            />
            {updateErrors.lastName && <span className="error">{updateErrors.lastName}</span>}
          </div>
          <div className="form-group">
            <label>Email:</label>
            <input
              type="email"
              name="email"
              value={updateForm.email}
              onChange={handleUpdateChange}
            />
            {updateErrors.email && <span className="error">{updateErrors.email}</span>}
          </div>
          <button type="submit">Save Changes</button>
        </form>
      </section>

      <section className="deactivate-account">
        <h3>Deactivate Account</h3>
        <button onClick={handleDeactivate} className="deactivate-btn">
          Deactivate Account
        </button>
      </section>

      <section className="password-recovery">
        <h3>Password Recovery</h3>
        <button onClick={handleSendRecoveryEmail} className="recovery-btn">
          Send Password Recovery Email
        </button>
      </section>

      <section className="reset-password">
        <h3>Reset Password</h3>
        <form onSubmit={handleResetPasswordSubmit}>
          <div className="form-group">
            <label>Token:</label>
            <input
              type="text"
              name="token"
              value={resetPasswordForm.token}
              onChange={handleResetPasswordChange}
            />
            {resetPasswordErrors.token && <span className="error">{resetPasswordErrors.token}</span>}
          </div>
          <div className="form-group">
            <label>New Password:</label>
            <input
              type="password"
              name="newPassword"
              value={resetPasswordForm.newPassword}
              onChange={handleResetPasswordChange}
            />
            {resetPasswordErrors.newPassword && <span className="error">{resetPasswordErrors.newPassword}</span>}
          </div>
          <div className="form-group">
            <label>Confirm New Password:</label>
            <input
              type="password"
              name="confirmPassword"
              value={resetPasswordForm.confirmPassword}
              onChange={handleResetPasswordChange}
            />
            {resetPasswordErrors.confirmPassword && <span className="error">{resetPasswordErrors.confirmPassword}</span>}
          </div>
          <button type="submit">Reset Password</button>
        </form>
      </section>
    </div>
  );
};

export default UserSettings;
