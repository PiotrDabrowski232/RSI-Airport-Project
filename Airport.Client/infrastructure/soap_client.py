import zeep
from zeep.exceptions import TransportError
from zeep.transports import Transport
import config

def initialize_soap_client():
    session = Transport(timeout=config.REQUEST_TIMEOUT)
    try:
        client = zeep.Client(wsdl=config.BASE_WSDL, transport=session)
        print("Pomyślnie połączono z serwisem WCF i załadowano WSDL.")

        flight_service = client.service
        passenger_service = None
        ticket_service = None

        if not client.wsdl.services:
             raise Exception("WSDL nie zawiera definicji żadnego serwisu.")

        service_name = list(client.wsdl.services.values())[0].name
        print(f"Znaleziono nazwę serwisu: {service_name}")

        try:
            passenger_service = client.bind(service_name, config.PASSENGER_SERVICE_PORT_NAME)
            print(f"Pomyślnie powiązano z portem: {config.PASSENGER_SERVICE_PORT_NAME}")
        except Exception as bind_error:
             raise Exception(f"Krytyczny błąd: Nie można powiązać z portem {config.PASSENGER_SERVICE_PORT_NAME}. {bind_error}") from bind_error

        try:
            ticket_service = client.bind(service_name, config.TICKET_SERVICE_PORT_NAME)
            print(f"Pomyślnie powiązano z portem: {config.TICKET_SERVICE_PORT_NAME}")
        except Exception as bind_error:
             raise Exception(f"Krytyczny błąd: Nie można powiązać z portem {config.TICKET_SERVICE_PORT_NAME}. {bind_error}") from bind_error


        print("Inicjalizacja klienta SOAP zakończona.")
        return client, flight_service, passenger_service, ticket_service

    except (TransportError, ConnectionError) as e:
        print(f"Błąd połączenia SOAP: {e}")
        raise ConnectionError(f"Nie można połączyć się z serwisem WCF lub pobrać WSDL.\nURL: {config.BASE_WSDL}\nSzczegóły: {e}") from e
    except Exception as e:
        print(f"Błąd inicjalizacji SOAP lub bindowania: {e}")
        raise RuntimeError(f"Błąd inicjalizacji klienta SOAP lub bindowania portu:\n{e}") from e