import { useState } from "react";
import { flightService, passengerService, ticketService } from "../apiService";
import { API_USERNAME, API_PASSWORD } from "../../config.js";
import "./TicketPurchase.css";

const TicketPurchase = ({ flight, onBack, onPurchaseComplete }) => {
  const [passengerData, setPassengerData] = useState({
    name: "",
    surname: "",
    pesel: "",
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);
  const [purchasedTicket, setPurchasedTicket] = useState(null);

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
      const passengerResponse = await passengerService.createPassenger(
        passengerData
      );
      const passengerId = passengerResponse.data.id;

      const ticketData = {
        flightId: flight.id,
        passengerName: passengerData.name,
        passengerSurname: passengerData.surname,
        passengerPesel: passengerData.pesel,
      };

      const ticketResponse = await ticketService.purchaseTicket(ticketData);
      setPurchasedTicket(ticketResponse.data);
      setSuccess(true);
      if (onPurchaseComplete) {
        onPurchaseComplete(ticketResponse.data.ticketID);
      }
    } catch (err) {
      setError("Błąd podczas zakupu biletu: " + err.message);
    } finally {
      setLoading(false);
    }
  };

  const getPdfLink = () => {
    if (!purchasedTicket?.links) return null;
    return purchasedTicket.links.find(link => link.rel === "ticket-pdf");
  };

  const getSelfLink = () => {
    if (!purchasedTicket?.links) return null;
    return purchasedTicket.links.find(link => link.rel === "self");
  };

  const getFullUrl = (href) => {
    if (href.startsWith('http')) {
      return href;
    }
    return `/api${href}`;
  };

  const handleDownloadPdf = async () => {
    const pdfLink = getPdfLink();
    if (!pdfLink) {
      alert("Link do PDF nie jest dostępny");
      return;
    }

    try {
      const response = await fetch(getFullUrl(pdfLink.href), {
        method: pdfLink.method || 'GET',
        headers: {
          'Authorization': 'Basic ' + btoa(`${API_USERNAME}:${API_PASSWORD}`)
        }
      });

      if (!response.ok) {
        throw new Error('Błąd pobierania PDF');
      }

      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute("download", `ticket-${purchasedTicket.ticketID}.pdf`);
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (err) {
      console.error("Błąd podczas pobierania PDF: ", err);
      alert("Nie udało się pobrać biletu w formacie PDF.");
    }
  };

  const handleViewTicketDetails = async () => {
    const selfLink = getSelfLink();
    if (!selfLink) {
      alert("Link do szczegółów biletu nie jest dostępny");
      return;
    }

    try {
      const response = await fetch(getFullUrl(selfLink.href), {
        method: selfLink.method || 'GET',
        headers: {
          'Authorization': 'Basic ' + btoa(`${API_USERNAME}:${API_PASSWORD}`),
          'Content-Type': 'application/json'
        }
      });

      if (!response.ok) {
        throw new Error('Błąd pobierania szczegółów biletu');
      }

      const ticketDetails = await response.json();
      console.log("Szczegóły biletu:", ticketDetails);
      alert(`Szczegóły biletu:\nID: ${ticketDetails.id}\nPasażer: ${ticketDetails.name} ${ticketDetails.surname}\nPESEL: ${ticketDetails.pesel}`);
    } catch (err) {
      console.error("Błąd podczas pobierania szczegółów: ", err);
      alert("Nie udało się pobrać szczegółów biletu.");
    }
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
          {purchasedTicket && (
            <div className="ticket-id-section">
              <p>
                <strong>ID Twojego biletu:</strong>
              </p>
              <div className="ticket-id-display">
                <span>{purchasedTicket.ticketID}</span>
                <button
                  onClick={() => copyToClipboard(purchasedTicket.ticketID)}
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
            {getPdfLink() && (
              <button onClick={handleDownloadPdf} className="download-pdf-button">
                📄 Pobierz bilet (PDF)
              </button>
            )}
            {getSelfLink() && (
              <button onClick={handleViewTicketDetails} className="download-pdf-button">
                🔍 Zobacz szczegóły
              </button>
            )}
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