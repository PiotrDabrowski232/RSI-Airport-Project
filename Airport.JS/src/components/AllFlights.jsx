import { useState, useEffect } from 'react';
import { flightService } from '../apiService';
import './AllFlights.css';

const AllFlights = () => {
  const [flights, setFlights] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const loadAllFlights = async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await flightService.getAllFlights();
      setFlights(response.data);
    } catch (err) {
      setError('Błąd podczas ładowania lotów: ' + err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadAllFlights();
  }, []);

  if (loading) {
    return (
      <div className="all-flights">
        <div className="loading">
          <div className="spinner"></div>
          <p>Ładowanie lotów...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="all-flights">
      <div className="header">
        <h2>Wszystkie dostępne loty</h2>
        <button onClick={loadAllFlights} className="refresh-button" disabled={loading}>
          🔄 Odśwież listę
        </button>
      </div>

      {error && <div className="error">{error}</div>}

      <div className="flights-stats">
        <div className="stat-card">
          <h3>{flights.length}</h3>
          <p>Dostępnych lotów</p>
        </div>
        <div className="stat-card">
          <h3>{new Set(flights.map(f => f.from)).size}</h3>
          <p>Miast wylotu</p>
        </div>
        <div className="stat-card">
          <h3>{new Set(flights.map(f => f.to)).size}</h3>
          <p>Miast docelowych</p>
        </div>
      </div>

      {flights.length === 0 && !loading ? (
        <div className="no-flights">
          <h3>Brak dostępnych lotów</h3>
          <p>Obecnie nie ma żadnych lotów w systemie.</p>
        </div>
      ) : (
        <div className="flights-table-container">
          <table className="flights-table">
            <thead>
              <tr>
                <th>Trasa</th>
                <th>Data wylotu</th>
                <th>Godzina</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {flights.map((flight, index) => (
                <tr key={index} className="flight-row">
                  <td className="route">
                    <span className="route-text">
                      {flight.flightFrom} → {flight.flightTo}
                    </span>
                  </td>
                  <td>{new Date(flight.departureDate).toLocaleDateString('pl-PL')}</td>
                  <td>{new Date(flight.departureDate).toLocaleTimeString('pl-PL', { hour: '2-digit', minute: '2-digit' })}</td>
                  <td>
                    <span className="status-badge available">Dostępny</span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

export default AllFlights;