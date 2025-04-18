from datetime import datetime
import uuid
from zeep.exceptions import Fault, ValidationError

class AirportApiService:
    def __init__(self, flight_svc, passenger_svc):
        """Inicjalizuje serwis API z powiązanymi portami SOAP."""
        if not flight_svc:
            raise ValueError("flight_svc nie może być None")
        if not passenger_svc:
             raise ValueError("passenger_svc nie może być None")
        self.flight_service = flight_svc
        self.passenger_service = passenger_svc

    def search_flights(self, from_loc, to_loc, departure_date):
        """Wyszukuje loty."""
        try:
            print(f"API: Wyszukiwanie lotów: Od={from_loc}, Do={to_loc}, Data={departure_date}")
            result = self.flight_service.SearchFlights(
                from_=from_loc if from_loc else None,
                to=to_loc if to_loc else None,
                departureDate=departure_date
            )
            print(f"API: Otrzymano {len(result) if result else 0} wyników wyszukiwania.")
            return result if result else []
        except Fault as f:
            print(f"Błąd SOAP podczas SearchFlights: {f.message}")
            raise
        except ValidationError as ve:
             print(f"Błąd walidacji danych wejściowych SearchFlights: {ve}")
             raise
        except AttributeError as ae:
             print(f"Błąd metody SearchFlights (prawdopodobnie zły port?): {ae}")
             raise RuntimeError("Wewnętrzny błąd: Metoda SearchFlights niedostępna.") from ae
        except Exception as e:
            print(f"Nieoczekiwany błąd podczas SearchFlights: {e}")
            raise

    def get_all_flights(self):
        """Pobiera wszystkie loty."""
        try:
            print("API: Pobieranie wszystkich lotów...")
            result = self.flight_service.GetFlights()
            print(f"API: Otrzymano {len(result) if result else 0} wszystkich lotów.")
            return result if result else []
        except Fault as f:
            print(f"Błąd SOAP podczas GetFlights: {f.message}")
            raise
        except AttributeError as ae:
             print(f"Błąd metody GetFlights (prawdopodobnie zły port?): {ae}")
             raise RuntimeError("Wewnętrzny błąd: Metoda GetFlights niedostępna.") from ae
        except Exception as e:
            print(f"Nieoczekiwany błąd podczas GetFlights: {e}")
            raise

    def create_passenger(self, name, surname, pesel):
        """Tworzy nowego pasażera."""
        if not name or not surname or not pesel:
            raise ValueError("Imię, nazwisko i PESEL są wymagane.")
        try:
            print(f"API: Tworzenie pasażera: {name} {surname}")
            passenger_id = self.passenger_service.CreatePassenger(name=name, surname=surname, pesel=pesel)
            print(f"API: Utworzono pasażera, ID: {passenger_id}")
            return passenger_id # Zwracamy ID
        except Fault as f:
            print(f"Błąd SOAP podczas CreatePassenger: {f.message}")
            raise
        except ValidationError as ve:
             print(f"Błąd walidacji danych wejściowych CreatePassenger: {ve}")
             raise
        except AttributeError as ae:
             print(f"Błąd metody CreatePassenger (prawdopodobnie zły port?): {ae}")
             raise RuntimeError("Wewnętrzny błąd: Metoda CreatePassenger niedostępna.") from ae
        except Exception as e:
            print(f"Nieoczekiwany błąd podczas CreatePassenger: {e}")
            raise

    def get_all_passengers(self):
        """Pobiera listę wszystkich pasażerów."""
        try:
            print("API: Pobieranie listy pasażerów...")
            result = self.passenger_service.GetPassengers()
            print(f"API: Otrzymano {len(result) if result else 0} pasażerów.")
            return result if result else []
        except Fault as f:
            print(f"Błąd SOAP podczas GetPassengers: {f.message}")
            raise
        except AttributeError as ae:
             print(f"Błąd metody GetPassengers (prawdopodobnie zły port?): {ae}")
             raise RuntimeError("Wewnętrzny błąd: Metoda GetPassengers niedostępna.") from ae
        except Exception as e:
            print(f"Nieoczekiwany błąd podczas GetPassengers: {e}")
            raise

    def purchase_ticket(self, flight_id, passenger_id):
         raise NotImplementedError("Funkcjonalność zakupu biletu nie jest dostępna w serwisie WCF.")

    def get_passenger_tickets(self, passenger_id):
         raise NotImplementedError("Funkcjonalność pobierania biletów pasażera nie jest dostępna w serwisie WCF.")