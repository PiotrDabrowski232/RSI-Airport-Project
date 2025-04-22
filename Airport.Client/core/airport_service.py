import uuid
from zeep.exceptions import Fault, ValidationError, TransportError
import base64

class AirportApiService:
    def __init__(self, flight_svc, passenger_svc, ticket_svc, client):
        if not flight_svc: raise ValueError("flight_svc nie może być None")
        if not passenger_svc: raise ValueError("passenger_svc nie może być None")
        if not ticket_svc: raise ValueError("ticket_svc nie może być None")
        if not client: raise ValueError("client nie może być None")
        self.flight_service = flight_svc
        self.passenger_service = passenger_svc
        self.ticket_service = ticket_svc
        self.client = client

    def search_flights(self, from_loc, to_loc, departure_date):
        try:
            print(f"API: Wyszukiwanie lotów: Od={from_loc}, Do={to_loc}, Data={departure_date}")
            search_args = {
                'from': from_loc if from_loc else None,
                'to': to_loc if to_loc else None,
                'departureDate': departure_date
            }
            result = self.flight_service.SearchFlights(**search_args)
            print(f"API: Otrzymano {len(result) if result else 0} wyników wyszukiwania.")
            return result if result else []
        except Fault as f: print(f"Błąd SOAP podczas SearchFlights: {f.message}"); raise
        except ValidationError as ve: print(f"Błąd walidacji danych wejściowych SearchFlights: {ve}"); raise
        except AttributeError as ae: print(f"Błąd metody SearchFlights: {ae}"); raise RuntimeError("Wewnętrzny błąd: Metoda SearchFlights niedostępna.") from ae
        except TypeError as te: print(f"Błąd typu argumentów SearchFlights: {te}"); raise RuntimeError(f"Wewnętrzny błąd: Niezgodność argumentów wywołania SearchFlights. {te}") from te
        except Exception as e: print(f"Nieoczekiwany błąd podczas SearchFlights: {e}"); raise

    def get_all_flights(self):
        try:
            print("API: Pobieranie wszystkich lotów...")
            result = self.flight_service.GetFlights()
            print(f"API: Otrzymano {len(result) if result else 0} wszystkich lotów.")
            return result if result else []
        except Fault as f: print(f"Błąd SOAP podczas GetFlights: {f.message}"); raise
        except AttributeError as ae: print(f"Błąd metody GetFlights: {ae}"); raise RuntimeError("Wewnętrzny błąd: Metoda GetFlights niedostępna.") from ae
        except Exception as e: print(f"Nieoczekiwany błąd podczas GetFlights: {e}"); raise

    def create_passenger(self, name, surname, pesel):
        if not name or not surname or not pesel: raise ValueError("Imię, nazwisko i PESEL są wymagane.")
        try:
            print(f"API: Tworzenie pasażera: {name} {surname}")
            passenger_id = self.passenger_service.CreatePassenger(name=name, surname=surname, pesel=pesel)
            print(f"API: Utworzono pasażera, ID: {passenger_id}")
            return passenger_id
        except Fault as f: print(f"Błąd SOAP podczas CreatePassenger: {f.message}"); raise
        except ValidationError as ve: print(f"Błąd walidacji danych wejściowych CreatePassenger: {ve}"); raise
        except AttributeError as ae: print(f"Błąd metody CreatePassenger: {ae}"); raise RuntimeError("Wewnętrzny błąd: Metoda CreatePassenger niedostępna.") from ae
        except Exception as e: print(f"Nieoczekiwany błąd podczas CreatePassenger: {e}"); raise

    def get_all_passengers(self):
        try:
            print("API: Pobieranie listy pasażerów...")
            result = self.passenger_service.GetPassengers()
            print(f"API: Otrzymano {len(result) if result else 0} pasażerów.")
            return result if result else []
        except Fault as f: print(f"Błąd SOAP podczas GetPassengers: {f.message}"); raise
        except AttributeError as ae: print(f"Błąd metody GetPassengers: {ae}"); raise RuntimeError("Wewnętrzny błąd: Metoda GetPassengers niedostępna.") from ae
        except Exception as e: print(f"Nieoczekiwany błąd podczas GetPassengers: {e}"); raise

    def reserve_ticket(self, flight_id: uuid.UUID, name: str, surname: str, pesel: str):
        if not all([flight_id, name, surname, pesel]):
            raise ValueError("ID Lotu, Imię, Nazwisko i PESEL są wymagane do rezerwacji.")
        try:
            print(f"API: Rezerwacja biletu dla lotu {flight_id} przez {name} {surname} ({pesel})")
            qname = '{http://schemas.datacontract.org/2004/07/Airport.Server.DTOs}TicketPurchaseDTO'
            try:
                TicketPurchaseDTO_Type = self.client.get_type(qname)
            except ValueError as e_gettype:
                print(f"Błąd: Nie znaleziono typu DTO za pomocą get_type('{qname}'): {e_gettype}")
                raise RuntimeError(f"Wewnętrzny błąd: Typ '{qname}' niedostępny w WSDL.") from e_gettype
            dto = TicketPurchaseDTO_Type(FlightId=str(flight_id), PassengerName=name, PassengerSurname=surname, PassengerPesel=pesel)
            ticket_id = self.ticket_service.PurchaseTicket(ticketPurchaseDto=dto)
            print(f"API: Zarezerwowano bilet, ID: {ticket_id}")
            return ticket_id
        except AttributeError as ae:
             print(f"Błąd metody lub typu (reserve_ticket): {ae}")
             raise RuntimeError(f"Wewnętrzny błąd: Metoda 'PurchaseTicket' lub typ '{qname}' niedostępny w odpowiednim serwisie/porcie. {ae}") from ae
        except Fault as f: print(f"Błąd SOAP podczas PurchaseTicket: {f.message}"); raise
        except ValidationError as ve: print(f"Błąd walidacji danych wejściowych PurchaseTicket: {ve}"); raise
        except Exception as e: print(f"Nieoczekiwany błąd podczas reserve_ticket: {e}"); raise

    def get_passenger_tickets(self, passenger_id):
        try:
             print(f"API: Pobieranie biletów dla pasażera ID: {passenger_id}")
             tickets = self.ticket_service.GetPassengerTickets(passengerId=str(passenger_id))
             print(f"API: Otrzymano {len(tickets) if tickets else 0} biletów.")
             return tickets if tickets else []
        except AttributeError as ae: print(f"Błąd metody (get_passenger_tickets): {ae}"); raise RuntimeError(f"Wewnętrzny błąd: Metoda 'GetPassengerTickets' niedostępna. {ae}") from ae
        except Fault as f: print(f"Błąd SOAP podczas GetPassengerTickets: {f.message}"); raise
        except Exception as e: print(f"Nieoczekiwany błąd podczas get_passenger_tickets: {e}"); raise

    def get_ticket_by_id(self, ticket_id: uuid.UUID):
        if not ticket_id:
            raise ValueError("ID biletu jest wymagane.")
        try:
            print(f"API: Pobieranie biletu o ID: {ticket_id}")
            ticket_dto = self.ticket_service.GetTicketById(ticketId=str(ticket_id))
            if ticket_dto:
                 print(f"API: Znaleziono bilet.")
            else:
                 print(f"API: Nie znaleziono biletu o ID: {ticket_id}")
            return ticket_dto
        except AttributeError as ae:
             print(f"Błąd metody (get_ticket_by_id): {ae}")
             raise RuntimeError(f"Wewnętrzny błąd: Metoda 'GetTicketById' niedostępna. {ae}") from ae
        except Fault as f:
            print(f"Błąd SOAP podczas GetTicketById: {f.message}")
            raise
        except Exception as e:
             print(f"Nieoczekiwany błąd podczas get_ticket_by_id: {e}")
             raise

    def get_ticket_pdf(self, ticket_id: uuid.UUID):
        if not ticket_id:
            raise ValueError("ID biletu jest wymagane.")
        try:
            print(f"API: Żądanie PDF dla biletu ID: {ticket_id}")
            pdf_base64 = self.flight_service.GetTicketConfirmationPdf(ticketId=str(ticket_id))
            if pdf_base64:
                print(f"API: Otrzymano dane PDF (base64).")
                pdf_bytes = base64.b64decode(pdf_base64)
                return pdf_bytes
            else:
                 print(f"API: Nie otrzymano danych PDF dla biletu ID: {ticket_id}")
                 return None
        except AttributeError as ae:
             print(f"Błąd metody (get_ticket_pdf): {ae}")
             raise RuntimeError(f"Wewnętrzny błąd: Metoda 'GetTicketConfirmationPdf' niedostępna w serwisie lotów (flight_service). {ae}") from ae
        except Fault as f:
            print(f"Błąd SOAP podczas GetTicketConfirmationPdf: {f.message}")
            raise
        except (TransportError, ConnectionError) as te:
             print(f"Błąd transportu/połączenia podczas GetTicketConfirmationPdf: {te}")
             raise ConnectionError(f"Błąd połączenia podczas pobierania PDF. {te}") from te
        except Exception as e:
             print(f"Nieoczekiwany błąd podczas get_ticket_pdf: {e}")
             raise