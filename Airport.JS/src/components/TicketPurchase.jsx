import { useState } from "react";
import { flightService, passengerService, ticketService } from "../apiService";
import "./TicketPurchase.css";
import { set } from "react-hook-form";

const TicketPurchase = ({ flight, onBack, onPurchaseComplete }) => {
  const [passengerData, setPassengerData] = useState({
    name: "",
    surname: "",
    pesel: "",
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);
  const [purchasedTicketId, setPurchasedTicketId] = useState(null);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setPassengerData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handlePurchase = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      // Najpierw stwórz pasażera
      const passengerResponse = await passengerService.createPassenger(
        passengerData
      );
      const passengerId = passengerResponse.data.id;

      // Następnie kup bilet
      const ticketData = {
        flightId: flight.id,
        passengerName: passengerData.name,
        passengerSurname: passengerData.surname,
        passengerPesel: passengerData.pesel,
      };

      const ticketResponse = await ticketService.purchaseTicket(ticketData);
      setPurchasedTicketId(ticketResponse.data);
      setSuccess(true);
      if (onPurchaseComplete) {
        onPurchaseComplete(ticketResponse.data);
      }
    } catch (err) {
      setError("Błąd podczas zakupu biletu: " + err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleDownloadPdf = () => {
    flightService
      .getFlightPdf(purchasedTicketId)
      .then((response) => {
        const blob = new Blob([response.data], { type: "application/pdf" });
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement("a");
        link.href = url;
        link.setAttribute("download", `ticket-${purchasedTicketId}.pdf`);
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
      })
      .catch((err) => {
        console.error("Błąd podczas pobierania PDF: ", err);
        alert("Nie udało się pobrać biletu w formacie PDF.");
      });
  };

  const copyToClipboard = (text) => {
    navigator.clipboard
      .writeText(text)
      .then(() => {
        alert("ID biletu skopiowane do schowka!");
      })
      .catch((err) => {
        console.error("Błąd kopiowania do schowka: ", err);
        alert("Nie udało się skopiować ID biletu.");
      });
  };

  if (success) {
    return (
      <div className="ticket-purchase">
        <div className="success-message">
          <h2>✅ Bilet został zakupiony pomyślnie!</h2>
          {purchasedTicketId && (
            <div className="ticket-id-section">
              <p>
                <strong>ID Twojego biletu:</strong>
              </p>
              <div className="ticket-id-display">
                <span>{purchasedTicketId}</span>
                <button
                  onClick={() => copyToClipboard(purchasedTicketId)}
                  className="copy-button"
                  title="Kopiuj ID biletu"
                >
                  📋
                </button>
              </div>
              <p className="info-text">
                Zapisz to ID, aby móc sprawdzić swoją rezerwację.
              </p>
            </div>
          )}
          <p>
            Dziękujemy za zakup. Szczegóły zostały wysłane na Twój email
          </p>
          <div className="success-actions">
            <button onClick={handleDownloadPdf} className="download-pdf-button">
              📄 Pobierz bilet (PDF)
            </button>
            <button onClick={onBack} className="back-button">
              Powrót do wyszukiwania
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="ticket-purchase">
      <h2>Zakup biletu</h2>

      <div className="flight-summary">
        <h3>Szczegóły lotu</h3>
        <p>
          <strong>Trasa:</strong> {flight.flightFrom} → {flight.flightTo}
        </p>
        <p>
          <strong>Data:</strong>{" "}
          {new Date(flight.departureDate).toLocaleDateString()}
        </p>
        <p>
          <strong>Godzina:</strong>{" "}
          {new Date(flight.departureDate).toLocaleTimeString()}
        </p>
      </div>

      <form onSubmit={handlePurchase} className="purchase-form">
        <h3>Dane pasażera</h3>

        <div className="form-group">
          <label>Imię:</label>
          <input
            type="text"
            name="name"
            value={passengerData.name}
            onChange={handleInputChange}
            required
            placeholder="Wprowadź imię"
          />
        </div>

        <div className="form-group">
          <label>Nazwisko:</label>
          <input
            type="text"
            name="surname"
            value={passengerData.surname}
            onChange={handleInputChange}
            required
            placeholder="Wprowadź nazwisko"
          />
        </div>

        <div className="form-group">
          <label>PESEL:</label>
          <input
            type="text"
            name="pesel"
            value={passengerData.pesel}
            onChange={handleInputChange}
            required
            placeholder="Wprowadź PESEL"
            maxLength="11"
            pattern="[0-9]{11}"
          />
        </div>

        {error && <div className="error">{error}</div>}

        <div className="form-actions">
          <button type="button" onClick={onBack} className="cancel-button">
            Anuluj
          </button>
          <button type="submit" disabled={loading} className="purchase-button">
            {loading ? "Przetwarzanie..." : "Kup bilet"}
          </button>
        </div>
      </form>
    </div>
  );
};

export default TicketPurchase;
