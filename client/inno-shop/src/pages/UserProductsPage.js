import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getUserProducts, createProduct, updateProduct, deleteProduct } from '../services/api';
import './UserProductsPage.css'; 

const ProductManagementPage = () => {
  const navigate = useNavigate();
  const [allProducts, setAllProducts] = useState([]); 
  const [displayedProducts, setDisplayedProducts] = useState([]); 
  const [isEditing, setIsEditing] = useState(false);
  const [currentProduct, setCurrentProduct] = useState(null);
  const [formData, setFormData] = useState({
    id: '',
    name: '',
    description: '',
    price: '',
    isAvailable: true,
    quantity: '',
    imageId: '',
    imageData: '',
    imageType: '',
  });
  const [errors, setErrors] = useState({}); 
  const [imagePreview, setImagePreview] = useState(null); 
  const [pageIndex, setPageIndex] = useState(1);
  const [pageSize] = useState(3);
  const [role, setRole] = useState(null);

  const loadProducts = async () => {
    try {
      const storedUser = localStorage.getItem('user');
      if (storedUser) {
        const user = JSON.parse(storedUser);
        const products = await getUserProducts(user.id);
        setAllProducts(products || []);
      } else {
        console.error('Пользователь не найден');
        navigate('/auth');
      }
    } catch (error) {
      console.error('Error loading products:', error);
    }
  };

  useEffect(() => {
    const storedRole = localStorage.getItem('role');
    setRole(storedRole);
    if (storedRole === "Admin" ||  "User") {
      loadProducts();
    } else {
      navigate('/auth');
    }
  }, [navigate]);

  useEffect(() => {
    const startIndex = (pageIndex - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    setDisplayedProducts(allProducts.slice(startIndex, endIndex));
  }, [allProducts, pageIndex, pageSize]);

  const totalPages = Math.ceil(allProducts.length / pageSize) || 1;

  const validateForm = () => {
    const newErrors = {};
    if (!formData.name) newErrors.name = 'Name is required';
    if (!formData.description) newErrors.description = 'Description is required';
    if (!formData.price) newErrors.price = 'Price is required';
    if (!formData.quantity) newErrors.quantity = 'Quantity is required';
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleDelete = async (productId) => {
    try {
      await deleteProduct(productId);
      setAllProducts(allProducts.filter(product => product.id !== productId));
    } catch (error) {
      console.error('Error deleting product:', error);
    }
  };

  const handleEdit = (product) => {
    setIsEditing(true);
    setCurrentProduct(product);
    setFormData({
      id: product ? product.id : '',
      name: product ? product.name : '',
      description: product ? product.description : '',
      price: product ? product.price : '',
      isAvailable: product?.isAvailable ?? true,
      quantity: product ? product.quantity : '',
      imageId: product ? product.imageId : '',
      imageData: product ? product.imageData : '',
      imageType: product ? product.imageType : '',
    });
    setImagePreview(product ? `data:${product.imageType};base64,${product.imageData}` : null);
    console.log(product);
  };

  const handleCancelEdit = () => {
    setIsEditing(false);
    setCurrentProduct(null);
    setFormData({
      id: '',
      name: '',
      description: '',
      price: '',
      isAvailable: true,
      quantity: '',
      imageId: '',
      imageData: '',
      imageType: '',
    });
    setImagePreview(null);
    setErrors({});
  };

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value,
    }));
  };

  const handleImageChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = () => {
        const base64Data = reader.result.split(',')[1];
        setFormData((prevFormData) => ({
          ...prevFormData,
          imageData: base64Data,
          imageType: file.type,
        }));
        setImagePreview(reader.result);
      };
      reader.readAsDataURL(file);
    } else {
      setImagePreview(null);
    }
  };

  const handleSaveProduct = async (e) => {
    e.preventDefault();
    if (!validateForm()) {
      console.error('Validation failed');
      return;
    }
  
    const price = parseFloat(formData.price);
    const quantity = parseInt(formData.quantity, 10);
  
    if (isNaN(price)) {
      setErrors((prev) => ({ ...prev, price: 'Price must be a valid number' }));
      return;
    }
    if (isNaN(quantity)) {
      setErrors((prev) => ({ ...prev, quantity: 'Quantity must be a valid number' }));
      return;
    }
  
    const updatedFormData = {
      ...formData,
      price,
      quantity
    };
  
    console.log('Updating product with ID:', updatedFormData.id);
  
    try {
      if (currentProduct) {
        await updateProduct(updatedFormData);
      } else {
        await createProduct(updatedFormData);
      }
      handleCancelEdit();
      await loadProducts();
      setPageIndex(1);
    } catch (error) {
      console.error('Error saving product:', error.response?.data || error.message);
    }
  };

  const handlePageChange = (newPageIndex) => {
    if (newPageIndex > 0 && newPageIndex <= totalPages) {
      setPageIndex(newPageIndex);
    }
  };

  return (
    <div className="product-management-page">
      <h2>Product Management</h2>
      <p className="product-description">Welcome to the Product Management page! 🎉</p>
      <button onClick={() => handleEdit(null)} className="create-product-btn">Create Product</button>

      <div className="content-wrapper">
        {isEditing && (
          <form onSubmit={handleSaveProduct} className="product-form">
            <div>
              <label>Name</label>
              <input
                type="text"
                name="name"
                value={formData.name}
                onChange={handleChange}
              />
              {errors.name && <span className="error">{errors.name}</span>}
            </div>
            <div>
              <label>Description</label>
              <textarea
                name="description"
                value={formData.description}
                onChange={handleChange}
              />
              {errors.description && <span className="error">{errors.description}</span>}
            </div>
            <div>
              <label>Price</label>
              <input
                type="number"
                name="price"
                value={formData.price}
                onChange={handleChange}
              />
              {errors.price && <span className="error">{errors.price}</span>}
            </div>
            <div>
              <label>Available</label>
              <input
                type="checkbox"
                name="isAvailable"
                checked={formData.isAvailable}
                onChange={handleChange}
              />
            </div>
            <div>
              <label>Quantity</label>
              <input
                type="number"
                name="quantity"
                value={formData.quantity}
                onChange={handleChange}
              />
              {errors.quantity && <span className="error">{errors.quantity}</span>}
            </div>
            <div>
              <label>Product Image</label>
              <input
                type="file"
                accept="image/*"
                onChange={handleImageChange}
              />
              {imagePreview && <img src={imagePreview} alt="Image Preview" />}\n            </div>
            <button type="submit">{currentProduct ? 'Save Changes' : 'Create Product'}</button>
            <button type="button" onClick={handleCancelEdit}>Cancel</button>
          </form>
        )}

        <div className="product-list">
          {displayedProducts.length > 0 ? (
            displayedProducts.map((product) => (
              <div key={product.id} className="product-card">
                <h3>{product.name}</h3>
                <p>{product.description}</p>
                {product.imageData && (
                  <img
                    src={`data:${product.imageType};base64,${product.imageData}`}
                    alt="Product"
                  />
                )}
                <p>{new Date(product.createdAt).toLocaleString()}</p>
                <button onClick={() => handleEdit(product)}>Edit</button>
                <button onClick={() => handleDelete(product.id)}>Delete</button>
              </div>
            ))
          ) : (
            <p>No products found. Please create a product.</p>
          )}
        </div>

        <div className="pagination">
          <button onClick={() => handlePageChange(pageIndex - 1)} disabled={pageIndex === 1}>Prev</button>
          <span>{pageIndex} / {totalPages}</span>
          <button onClick={() => handlePageChange(pageIndex + 1)} disabled={pageIndex === totalPages}>Next</button>
        </div>
      </div>
    </div>
  );
};

export default ProductManagementPage;
