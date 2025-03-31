import React, { useState, useEffect } from 'react';
import { getAllProducts } from '../services/api';
import { useNavigate } from 'react-router-dom';
import './ProductListPage.css';

const ProductListPage = () => {
  const [products, setProducts] = useState([]);
  const [pagination, setPagination] = useState({
    currentPage: 1,
    totalPages: 1,
    pageSize: 3,
    totalCount: 0,
    hasPrevious: false,
    hasNext: false,
  });

  const navigate = useNavigate();

  const fetchProducts = async (pageIndex = 1, pageSize = 2) => {
    try {
      const response = await getAllProducts(pageIndex, pageSize);
      setProducts(response.items || []);
      setPagination({
        currentPage: response.currentPage,
        totalPages: response.totalPages,
        pageSize: response.pageSize,
        totalCount: response.totalCount,
        hasPrevious: response.hasPrevious,
        hasNext: response.hasNext,
      });
    } catch (error) {
      console.error('Error fetching products:', error);
    }
  };

  useEffect(() => {
    fetchProducts(pagination.currentPage, pagination.pageSize);
  }, [pagination.currentPage]);

  const handlePageChange = (pageIndex) => {
    setPagination((prevState) => ({
      ...prevState,
      currentPage: pageIndex,
    }));
  };

  const handleProductClick = (id) => {
    navigate(`/products/${id}`); 
  };

  const generatePageNumbers = () => {
    const pageNumbers = [];
    for (let i = 1; i <= pagination.totalPages; i++) {
      pageNumbers.push(i);
    }
    return pageNumbers;
  };

  return (
    <div className="product-list-page">
      <h1 className="page-title">Our Products</h1>

      {products && products.length === 0 ? (
        <p className="no-products">No products available.</p>
      ) : (
        <div className="product-cards">
          {products.map((product) => (
            <div
              key={product.id}
              className="product-card"
              onClick={() => handleProductClick(product.id)} 
            >
              <img
                    src={`data:${product.imageType};base64,${product.imageData}`}
                    alt={product.name}
                    className="product-image"
                />
              <div className="product-details">
                <h3 className="product-title">{product.name}</h3>
                <p className="product-description">{product.description}</p>
                <div className="product-info">
                  <p><strong>Price:</strong> {product.price}</p>
                  <p><strong>Created At:</strong> {new Date(product.createdAt).toLocaleString()}</p>
                  <p><strong>Quantity:</strong> {product.quantity}</p>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}


      <div className="pagination-controls">
        <div className="page-numbers">
          {generatePageNumbers().map((pageNumber) => (
            <button
              key={pageNumber}
              onClick={() => handlePageChange(pageNumber)}
              className={`page-button ${pagination.currentPage === pageNumber ? 'active' : ''}`}
            >
              {pageNumber}
            </button>
          ))}
        </div>
      </div>
    </div>
  );
};

export default ProductListPage;
