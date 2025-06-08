import { useState } from 'react';
import { ticketService } from '../apiService';
import './TicketCheck.css';

const TicketCheck = () => {
  const [ticketId, setTicketId] = useState('');
  const [ticket, setTicket] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleSearch = async (e) => {
    e.preventDefault();
    if (!ticketId.trim()) {
      setError('Proszę wprowadzić numer biletu');
      return;
    }

    setLoading(true);
    setError(null);
    setTicket(null);

    try {
      const response = await ticketService.getTicketById(ticketId.trim());
      console.log("Otrzymane dane biletu:", response.data); // Debugging line
      setTicket(response.data);
    } catch (err) {
      if (err.response?.status === 404) {
        setError('Nie znaleziono biletu o podanym numerze');
      } else {
        setError('Błąd podczas sprawdzania biletu: ' + err.message);
      }
    } finally {
      setLoading(false);
    }
  };

  const handleClear = () => {
    setTicketId('');
    setTicket(null);
    setError(null);
  };

  return (
    <div className="ticket-check">
      <h2>Sprawdź rezerwację</h2>
      
      <form onSubmit={handleSearch} className="search-form">
        <div className="form-group">
          <label>Numer biletu:</label>
          <input
            type="text"
            value={ticketId}
            onChange={(e) => setTicketId(e.target.value)}
            placeholder="Wprowadź numer biletu (GUID)"
            className="ticket-input"
          />
        </div>
        
        <div className="form-actions">
          <button type="submit" disabled={loading} className="search-button">
            {loading ? 'Sprawdzanie...' : 'Sprawdź bilet'}
          </button>
          <button type="button" onClick={handleClear} className="clear-button">
            Wyczyść
          </button>
        </div>
      </form>

      {error && <div className="error">{error}</div>}

      {ticket && (
        <div className="ticket-details">
          <h3>Szczegóły biletu</h3>
          <div className="ticket-card">
            <div className="ticket-header">
              <h4>Bilet nr: {ticket.id}</h4>
              <div className="ticket-status">
                <span className="status-badge">Potwierdzony</span>
              </div>
            </div>
            
            <div className="ticket-info">
              <div className="info-row">
                <span className="label">Pasażer:</span>
                <span className="value">{ticket.name} {ticket.surname}</span>
              </div>
              
              <div className="info-row">
                <span className="label">PESEL:</span>
                <span className="value">{ticket.pesel}</span>
              </div>
              
              <div className="info-row">
                <span className="label">Lot:</span>
                <span className="value">{ticket.flightFrom} → {ticket.flightTo}</span>
              </div>
              
              <div className="info-row">
                <span className="label">Data wylotu:</span>
                <span className="value">
                  {ticket.departureDate}
                </span>
              </div>
              
              <div className="info-row">
                <span className="label">Data przylotu</span>
                <span className="value">
                  {ticket.arrivalDate}
                </span>
              </div>
              
            </div>
            
            <div className="ticket-actions">
              <button className="download-button">
                📄 Pobierz PDF
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default TicketCheck;