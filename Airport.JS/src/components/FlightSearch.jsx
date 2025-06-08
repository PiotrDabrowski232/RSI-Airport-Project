import { useState, useEffect } from 'react';
import { flightService } from '../apiService';
import TicketPurchase from './TicketPurchase';
import './FlightSearch.css';

const FlightSearch = () => {
  const [flights, setFlights] = useState([]);
  const [searchFrom, setSearchFrom] = useState('');
  const [searchTo, setSearchTo] = useState('');
  const [searchDate, setSearchDate] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [selectedFlight, setSelectedFlight] = useState(null);
  const [showPurchase, setShowPurchase] = useState(false);

  const handleSearch = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    
    try {
      const response = await flightService.searchFlights(searchFrom, searchTo, searchDate);
      setFlights(response.data);
    } catch (err) {
      setError('Błąd podczas wyszukiwania lotów: ' + err.message);
    } finally {
      setLoading(false);
    }
  };

  const loadAllFlights = async () => {
    setLoading(true);
    try {
      const response = await flightService.getAllFlights();
      console.log('Dane lotów:', response.data); // Debugging line
      setFlights(response.data);
    } catch (err) {
      setError('Błąd podczas ładowania lotów: ' + err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleBookFlight = (flight) => {
    setSelectedFlight(flight);
    setShowPurchase(true);
  };

  const handleBackToSearch = () => {
    setShowPurchase(false);
    setSelectedFlight(null);
  };

  const handlePurchaseComplete = (ticketData) => {
    console.log('Bilet zakupiony:', ticketData);
    loadAllFlights();
  };

  useEffect(() => {
    loadAllFlights();
  }, []);

  if (showPurchase && selectedFlight) {
    return (
      <TicketPurchase 
        flight={selectedFlight}
        onBack={handleBackToSearch}
        onPurchaseComplete={handlePurchaseComplete}
      />
    );
  }

  return (
    <div className="flight-search">
    <h2>Wyszukaj loty</h2>
    
    <form onSubmit={handleSearch} className="search-form">
      <div className="form-group">
        <label>Z miasta:</label>
        <input 
          type="text" 
          value={searchFrom} 
          onChange={(e) => setSearchFrom(e.target.value)}
          placeholder="Wprowadź miasto wylotu"
        />
      </div>
      
      <div className="form-group">
        <label>Do miasta:</label>
        <input 
          type="text" 
          value={searchTo} 
          onChange={(e) => setSearchTo(e.target.value)}
          placeholder="Wprowadź miasto docelowe"
        />
      </div>
      
      <div className="form-group">
        <label>Data wylotu:</label>
        <input 
          type="date" 
          value={searchDate} 
          onChange={(e) => setSearchDate(e.target.value)}
        />
      </div>
      
      <div className="button-group">
        <button type="submit" disabled={loading}>
          {loading ? 'Wyszukiwanie...' : 'Szukaj'}
        </button>
        
        <button type="button" onClick={loadAllFlights} disabled={loading}>
          Pokaż wszystkie
        </button>
      </div>
    </form>

      {error && <div className="error">{error}</div>}

      <div className="flights-list">
        <h3>Dostępne loty ({flights.length})</h3>
        {flights.length === 0 && !loading ? (
          <p>Brak dostępnych lotów</p>
        ) : (
          <div className="flights-grid">
            {flights.map((flight, index) => (
              <div key={index} className="flight-card">
                <h4>{flight.flightFrom} → {flight.flightTo}</h4>
                <p>Data: {new Date(flight.departureDate).toLocaleDateString()}</p>
                <p>Godzina: {new Date(flight.departureDate).toLocaleTimeString()}</p>
                <button 
                  className="book-button"
                  onClick={() => handleBookFlight(flight)}
                >
                  Kup bilet
                </button>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default FlightSearch;