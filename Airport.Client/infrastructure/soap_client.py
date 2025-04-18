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

        if not client.wsdl.services:
             raise Exception("WSDL nie zawiera definicji żadnego serwisu.")

        try:
            passenger_service = client.bind(config.SERVICE_NAME, config.PASSENGER_SERVICE_PORT_NAME)
            print(f"Pomyślnie powiązano z portem: {config.PASSENGER_SERVICE_PORT_NAME}")
        except Exception as bind_error:
             print(f"OSTRZEŻENIE: Nie udało się jawnie powiązać z portem '{config.PASSENGER_SERVICE_PORT_NAME}': {bind_error}")
             raise Exception(f"Krytyczny błąd: Nie można powiązać z portem {config.PASSENGER_SERVICE_PORT_NAME}. {bind_error}") from bind_error

        print("Inicjalizacja klienta SOAP zakończona.")
        return flight_service, passenger_service

    except (TransportError, ConnectionError) as e:
        print(f"Błąd połączenia SOAP: {e}")
        raise ConnectionError(f"Nie można połączyć się z serwisem WCF lub pobrać WSDL.\nURL: {config.BASE_WSDL}\nSzczegóły: {e}") from e
    except Exception as e:
        print(f"Błąd inicjalizacji SOAP lub bindowania: {e}")
        raise RuntimeError(f"Błąd inicjalizacji klienta SOAP lub bindowania portu:\n{e}") from e