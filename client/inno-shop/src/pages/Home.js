import React, { useState } from 'react';
import './Home.css';
import { filterProducts, searchProducts } from '../services/api';

const Home = () => {
    const currentDate = new Date();
    const formattedDate = currentDate.toLocaleDateString(undefined, {
        weekday: 'long',
        year: 'numeric',
        month: 'long',
        day: 'numeric',
    });

    const [isAvailable, setIsAvailable] = useState(false);
    const [minPrice, setMinPrice] = useState('');
    const [maxPrice, setMaxPrice] = useState('');
    const [searchTerm, setSearchTerm] = useState('');
    const [products, setProducts] = useState([]);
    const [error, setError] = useState(null);

    const handleFilter = async () => {
        const params = {
            IsAvailable: isAvailable,
            MinPrice: minPrice !== '' ? parseFloat(minPrice) : null,
            MaxPrice: maxPrice !== '' ? parseFloat(maxPrice) : null,
        };

        try {
            const data = await filterProducts(params);
            setProducts(data);
            setError(null);
        } catch (err) {
            setError('Ошибка при фильтрации продуктов');
            console.error(err);
        }
    };

    const handleSearch = async () => {
        try {
            const data = await searchProducts(searchTerm);
            setProducts(data);
            setError(null);
        } catch (err) {
            setError('Ошибка при поиске продуктов');
            console.error(err);
        }
    };

    return (
        <div className="home">
            <h2>Welcome to InnoShop</h2>
            <p>Your go-to platform for easy shopping and selling.</p>
            <p>Explore a wide range of products and discover the best deals.</p>

            <div className="date-time-info">
                <p><strong>Current Date:</strong> {formattedDate}</p>
            </div>

            <div className="search-section">
                <h3>Product Search</h3>
                <input
                    type="text"
                    placeholder="Enter product name"
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                />
                <button onClick={handleSearch}>Search</button>
            </div>

            <div className="filter-section">
                <h3>Product Filtration</h3>
                <div className="filter-field">
                    <label>
                        <input
                            type="checkbox"
                            checked={isAvailable}
                            onChange={(e) => setIsAvailable(e.target.checked)}
                        />
                        Available
                    </label>
                </div>
                <div className="filter-field">
                    <label>
Minimum price:
                        <input
                            type="number"
                            placeholder="0"
                            value={minPrice}
                            onChange={(e) => setMinPrice(e.target.value)}
                        />
                    </label>
                </div>
                <div className="filter-field">
                    <label>
                        Максимальная цена:
                        <input
                            type="number"
                            placeholder="0"
                            value={maxPrice}
                            onChange={(e) => setMaxPrice(e.target.value)}
                        />
                    </label>
                </div>
                <button onClick={handleFilter}>Filter</button>
                {error && <p className="error">{error}</p>}
            </div>

            <div className="products-grid">
                <h3>Results</h3>
                {products.length > 0 ? (
                    <div className="products-grid">
                        {products.map((product) => (
                            <div key={product.id} className="product-card">
                                <h4>{product.name}</h4>
                                <p>{product.description}</p>
                                <p><strong>Price:</strong> {product.price}</p>
                                <p><strong>Available:</strong> {product.isAvailable ? 'Available' : 'Out of stock'}</p>
                            </div>
                        ))}
                    </div>
                ) : (
                    <p>No products found or filtering failed.</p>
                )}
            </div>
        </div>
    );
};

export default Home;
