import axios from 'axios';
import { API_BASE_URL, API_USERNAME, API_PASSWORD } from '../config.js';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  auth: {
    username: API_USERNAME,
    password: API_PASSWORD
  },
  headers: {
    'Content-Type': 'application/json'
  }
});

export const flightService = {
  getAllFlights: () => apiClient.get('/Flight'),
  searchFlights: (from, to, departureDate) => {
    const params = new URLSearchParams();
    if (from) params.append('from', from);
    if (to) params.append('to', to);
    if (departureDate) params.append('departureDate', departureDate);
    
    return apiClient.get(`/Flight/Search?${params.toString()}`);
  },
  getFlightPdf: (flightId) => {
    return apiClient.get(`/Flight/${flightId}/pdf`, { responseType: 'blob' });
  }
};

export const passengerService = {
  getAllPassengers: () => apiClient.get('/Passenger'),
  getPassengerById: (id) => apiClient.get(`/Passenger/${id}`),
  createPassenger: (passengerData) => apiClient.post('/Passenger', passengerData)
};

export const ticketService = {
  purchaseTicket: (ticketData) => apiClient.post('/AirplaneTicket/Purchase', ticketData),
  getPassengerTickets: (passengerId) => apiClient.get(`/AirplaneTicket/Passenger/${passengerId}`),
  getTicketById: (ticketId) => apiClient.get(`/AirplaneTicket/${ticketId}`)
};

export default apiClient;