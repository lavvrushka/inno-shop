import axios from 'axios';

const APIProducts = axios.create({
  baseURL: 'http://localhost:5019/api',
  headers: { 'Content-Type': 'application/json' },
});

const APIUsers = axios.create({
    baseURL: 'http://localhost:5195/api',
    headers: { 'Content-Type': 'application/json' },
  });

  APIUsers.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('accessToken'); 
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`; 
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);
APIProducts.interceptors.request.use(
    (config) => {
      const token = localStorage.getItem('accessToken');
      if (token) {
        config.headers['Authorization'] = `Bearer ${token}`;
      }
      return config;
    },
    (error) => Promise.reject(error)
  );

APIUsers.interceptors.response.use(
  (response) => response, 
  async (error) => {
    const originalRequest = error.config;

    if (error.response && error.response.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const refreshToken = localStorage.getItem('refreshToken'); 

        if (!refreshToken) {
          console.error('Refresh token is missing');
          throw new Error('Refresh token is missing');
        }

        const response = await APIUsers.post('/refresh-token', { refreshToken });
        localStorage.setItem('accessToken', response.data.accessToken);
        localStorage.setItem('refreshToken', response.data.refreshToken);

        originalRequest.headers['Authorization'] = `Bearer ${response.data.accessToken}`;
        return APIUsers(originalRequest);
      } catch (refreshError) {
        console.error('Ошибка при обновлении токенов:', refreshError);
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);

export const refreshToken = async () => {
    try {
      const refreshToken = localStorage.getItem('refreshToken'); 
      if (!refreshToken) {
        throw new Error('Refresh token is missing');
      }
  
      const response = await APIUsers.post('/user/refresh-token', { refreshToken });
      localStorage.setItem('accessToken', response.data.accessToken);
      localStorage.setItem('refreshToken', response.data.refreshToken);
  
      console.log('Токены обновлены:', response.data);
  
      return response.data; 
    } catch (error) {
      console.error('Ошибка при обновлении токенов:', error);
      throw error;
    }
  };



  export const loginUser = async (userData) => {
    try {
      console.log('Попытка логина пользователя:', userData);
      const response = await APIUsers.post('/user/login', userData);

      localStorage.setItem('accessToken', response.data.accessToken);
      localStorage.setItem('refreshToken', response.data.refreshToken);
      console.log('Токены сохранены в localStorage:', response.data.accessToken, response.data.refreshToken);
  
      const currentUser = await getCurrentUser();
      console.log('Текущий пользователь:', currentUser);
      localStorage.setItem('role', JSON.stringify(currentUser.roleName));
      localStorage.setItem('user', JSON.stringify(currentUser));
      console.log('Данные пользователя сохранены:', currentUser.id, currentUser.roleName);
  
      return response.data;
    } catch (error) {
      console.error('Ошибка при логине пользователя:', error);
      throw error;
    }
  };

  export const getCurrentUser = async () => {
    try {
      const response = await APIUsers.get('/user/current');
      return response.data;
    } catch (error) {
      console.error('Ошибка при получении текущего пользователя:', error);
      throw error;
    }
};
export const registerUser = async (userData) => {
    try {
      console.log("Отправка данных для регистрации:", userData); 
      const response = await APIUsers.post('/user/register', userData);
      localStorage.setItem('accessToken', response.data.accessToken);
      localStorage.setItem('refreshToken', response.data.refreshToken);
      console.log('Токены сохранены в localStorage:', response.data.accessToken, response.data.refreshToken);
  
      const currentUser = await getCurrentUser();
      console.log('Текущий пользователь после регистрации:', currentUser);
  
      localStorage.setItem('user', JSON.stringify(currentUser));
      console.log('Данные пользователя сохранены:', currentUser.id, currentUser.roleName);
  
      return response.data;
    } catch (error) {
      console.error('Ошибка при регистрации пользователя:', error);
      if (error.response) {
        console.error('Ответ сервера:', error.response.data);
        alert(`Ошибка регистрации: ${error.response.data.message || error.response.statusText}`);
      } else {
        alert('Ошибка отправки запроса. Пожалуйста, попробуйте снова позже.');
      }
      throw error;
    }
  };
  
export const logoutUser = async () => {
    try {
      await APIUsers.post('/user/logout');
      localStorage.removeItem('accessToken'); 
      localStorage.removeItem('userId'); 
    } catch (error) {
      console.error('Error during user logout:', error);
      throw error;
    }
  };


  export const getAllProducts = async (pageIndex = 1, pageSize = 2) => {
    try {
      const response = await APIProducts.get(`/product/page?PageIndex=${pageIndex}&PageSize=${pageSize}`);
      console.log('API Response:', response.data); 
  
      if (Array.isArray(response.data.items)) {
        return response.data;
      } else {
        throw new Error('Полученные данные не являются массивом');
      }
    } catch (error) {
      console.error('Ошибка при получении событий:', error);
      throw error;
    }
  };

  export const getProductById = async (id) => {
    console.log("Fetching product with ID:", id); // <-- Лог перед запросом
    try {
      const response = await APIProducts.get(`/product/by-id/${id}`);
      return response.data;
    } catch (error) {
      console.error('Ошибка при получении события по ID:', error);
      throw error;
    }
  };

  export const getUserProducts = async (userId) => {
    try {
      const token = localStorage.getItem('accessToken');
      const response = await APIProducts.get('/product/user-products', {
        params: { userId }, 
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      console.log('API Response:', response.data); 
      
      if (Array.isArray(response.data)) {
        return response.data; 
      } else if (response.data.items && Array.isArray(response.data.items)) {
        return response.data.items; 
      } else {
        throw new Error('Неверная структура данных');
      }
    } catch (error) {
      console.error('Ошибка при получении событий пользователя:', error);
      throw error;
    }
  };
  
  export const filterProducts = async (params) => {
    try {
      const response = await APIProducts.get('/product/filter-products', { params });
      return response.data;
    } catch (error) {
      console.error('Ошибка при фильтрации событий:', error);
      throw error;
    }
  };

  export const deleteProduct = async (id) => {
    try {
      await APIProducts.delete(`/product/delete/${id}`);
    } catch (error) {
      console.error('Ошибка при удалении события:', error);
      throw error;
    }
  };

  export const updateProduct = async (updatedData) => {
    try {
      await APIProducts.put(`/product/update`, updatedData);
    } catch (error) {
      console.error('Ошибка при обновлении события:', error);
      throw error;
    }
  };

  export const createProduct = async (eventData) => {
    try {
      await APIProducts.post('/product/create', eventData);
    } catch (error) {
      console.error('Ошибка при создании события:', error);
      throw error;
    }
  };

  // Новая функция для поиска продуктов по строке searchTerm
export const searchProducts = async (searchTerm) => {
  try {
    const response = await APIProducts.get('/product/search-products', {
      params: { searchTerm },
    });
    return response.data;
  } catch (error) {
    console.error('Ошибка при поиске продуктов:', error);
    throw error;
  }
};


// Деактивация пользователя
export const deactivateUser = async () => {
  try {
    const response = await APIUsers.put('/user/deactivate');
    return response.data;
  } catch (error) {
    console.error('Ошибка при деактивации пользователя:', error);
    throw error;
  }
};

// Отправка письма для восстановления аккаунта (например, отправка токена для восстановления)
export const sendRecoveryEmailAccount = async (requestData) => {
  try {
    const response = await APIUsers.post('/user/send-recovery-email', requestData);
    return response.data;
  } catch (error) {
    console.error('Ошибка при отправке email для восстановления аккаунта:', error);
    throw error;
  }
};

// Восстановление аккаунта по токену (передаётся как query-параметр)
export const recoverAccount = async (token) => {
  try {
    const response = await APIUsers.post(`/user/recover?token=${encodeURIComponent(token)}`);
    return response.data;
  } catch (error) {
    console.error('Ошибка при восстановлении аккаунта:', error);
    throw error;
  }
};

// Обновление данных пользователя
export const updateUser = async (updatedData) => {
  try {
    const response = await APIUsers.put('/user/update', updatedData);
    return response.data;
  } catch (error) {
    console.error('Ошибка при обновлении пользователя:', error);
    throw error;
  }
};

export const confirmEmail = async (token) => {
  try {
    const response = await APIUsers.get(`/user/confirm-email/${encodeURIComponent(token)}`);
    return response.data;
  } catch (error) {
    console.error('Ошибка при подтверждении email:', error);
    throw error;
  }
};

export const sendPasswordRecoveryEmail = async (requestData) => {
  try {
    const response = await APIUsers.post('/PasswordRecovery/send-password-recovery-email', requestData);
    return response.data;
  } catch (error) {
    console.error('Ошибка при отправке email для восстановления пароля:', error);
    throw error;
  }
};

// Сброс пароля
export const resetPassword = async (requestData) => {
  try {
    const response = await APIUsers.post('/PasswordRecovery/reset-password', requestData);
    return response.data;
  } catch (error) {
    console.error('Ошибка при сбросе пароля:', error);
    throw error;
  }
};

export const getAllUsers = async () => {
  try {
    const response = await APIUsers.get('/user/all');
    return response.data;
  } catch (error) {
    console.error('Ошибка при получении списка пользователей:', error);
    throw error;
  }
};

export const deleteUser = async (id) => {
  try {
    await APIUsers.delete(`/user/delete/${id}`);
  } catch (error) {
    console.error('Ошибка при удалении пользователя:', error);
    throw error;
  }
};

