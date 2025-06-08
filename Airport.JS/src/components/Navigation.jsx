import { useState } from 'react';
import './Navigation.css';

const Navigation = ({ currentView, onViewChange }) => {
  return (
    <nav className="navigation">
      <div className="nav-container">
        <div className="nav-brand">
          <h1>✈️ System Lotniczy</h1>
        </div>
        
        <div className="nav-menu">
          <button 
            className={`nav-item ${currentView === 'search' ? 'active' : ''}`}
            onClick={() => onViewChange('search')}
          >
            🔍 Wyszukaj loty
          </button>
          
          <button 
            className={`nav-item ${currentView === 'all-flights' ? 'active' : ''}`}
            onClick={() => onViewChange('all-flights')}
          >
            📋 Wszystkie loty
          </button>
          
          <button 
            className={`nav-item ${currentView === 'check' ? 'active' : ''}`}
            onClick={() => onViewChange('check')}
          >
            🎫 Sprawdź rezerwację
          </button>
        </div>
      </div>
    </nav>
  );
};

export default Navigation;