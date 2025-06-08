import { useState } from 'react';
import Navigation from './components/Navigation';
import FlightSearch from './components/FlightSearch';
import AllFlights from './components/AllFlights';
import TicketCheck from './components/TicketCheck';
import './App.css';

function App() {
  const [currentView, setCurrentView] = useState('search');

  const renderCurrentView = () => {
    switch (currentView) {
      case 'search':
        return <FlightSearch />;
      case 'all-flights':
        return <AllFlights />;
      case 'check':
        return <TicketCheck />;
      default:
        return <FlightSearch />;
    }
  };

  return (
    <div className="App">
      <Navigation 
        currentView={currentView} 
        onViewChange={setCurrentView}
      />
      
      <main className="main-content">
        {renderCurrentView()}
      </main>
      
      <footer className="footer">
        <p>&copy; 2025 System Lotniczy. Wszystkie prawa zastrzeżone.</p>
      </footer>
    </div>
  );
}

export default App;