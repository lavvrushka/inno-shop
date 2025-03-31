import React, { useState, useEffect } from 'react';
import { getAllUsers, deleteUser } from '../services/api';
import { useNavigate } from 'react-router-dom';
import './ManagementPage.css';

const ManagementPage = () => {
  const navigate = useNavigate();
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [role, setRole] = useState(null);

  useEffect(() => {
   
    const storedRole = localStorage.getItem('role');
    setRole(storedRole);
    if (storedRole === "Admin") {
      fetchUsers();
    } else {
      navigate('/auth');
    }

  }, []);
  const fetchUsers = async () => {
    try {
      const data = await getAllUsers();
      setUsers(data);
    } catch (err) {
      console.error('Ошибка при получении списка пользователей:', err);
      setError('Ошибка при получении списка пользователей');
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteUser = async (id) => {
    if (window.confirm('Вы уверены, что хотите удалить этого пользователя?')) {
      try {
        await deleteUser(id);
        setUsers(users.filter(user => user.id !== id));
      } catch (err) {
        console.error('Ошибка при удалении пользователя:', err);
        alert('Не удалось удалить пользователя.');
      }
    }
  };

  if (loading) {
    return <div className="managementPage__loading">Загрузка...</div>;
  }

  if (error) {
    return <div className="managementPage__error">{error}</div>;
  }

  return (
    <div className="managementPage">
      <h1 className="managementPage__header">Users List</h1>
      <div className="managementPage__tableContainer">
        {users.length === 0 ? (
          <p>Users not found.</p>
        ) : (
          <table className="managementPage__table">
            <thead>
              <tr>
                <th>ID</th>
                <th>FirstName</th>
                <th>LastName</th>
                <th>Email</th>
                <th>birthDate</th>
                <th>Active</th>
                <th>Email confirm</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {users.map((user) => (
                <tr key={user.id}>
                  <td>{user.id}</td>
                  <td>{user.firstName}</td>
                  <td>{user.lastName}</td>
                  <td>{user.email}</td>
                  <td>{new Date(user.birthDate).toLocaleDateString()}</td>
                  <td>{user.isActive ? 'Да' : 'Нет'}</td>
                  <td>
                    {user.emailVerifiedAt
                      ? new Date(user.emailVerifiedAt).toLocaleDateString()
                      : '-'}
                  </td>
                  <td>
                    <button 
                      className="deleteButton" 
                      onClick={() => handleDeleteUser(user.id)}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
};

export default ManagementPage;