import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { getProductById } from '../services/api';
import './ProductDetailsPage.css';

const ProductDetailsPage = () => {
  const { productId } = useParams();
  const [product, setProduct] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchProduct = async () => {
      try {
        const data = await getProductById(productId);
        setProduct(data);
      } catch (err) {
        setError('Failed to load product details');
      } finally {
        setLoading(false);
      }
    };

    fetchProduct();
  }, [productId]);

  if (loading) return <div className="loader">Loading...</div>;
  if (error) return <div className="error">{error}</div>;
  if (!product) return <div className="error">Product not found</div>;

  return (
    <div className="product-details-page">
      <div className="product-details-content animate-fade-in">
        <div className="product-details-card glass-card">
          <h1 className="product-title">{product.name}</h1>
          
          {product.imageData && (
            <img
              src={`data:${product.imageType};base64,${product.imageData}`}
              alt={product.name}
              className="productdete-image"
            />
          )}
          
          <p className="product-description">{product.description}</p>
          <div className="product-meta">
            <p><strong>Quantity:</strong> {product.quantity}</p>
            <p><strong>Price:</strong> {product.price}</p>
            <p><strong>Created At:</strong> {new Date(product.createdAt).toLocaleString()}</p>
            <p><strong>Is Available:</strong> {product.isAvailable ? 'Yes' : 'No'}</p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProductDetailsPage;