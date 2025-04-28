# soap_client.py
import zeep
from zeep.exceptions import TransportError, Fault
from zeep.transports import Transport
import config
import os
import requests

def initialize_soap_client():
    script_dir = os.path.dirname(__file__)
    ca_cert_path = os.path.normpath(os.path.join(script_dir, 'rootCA.pem'))

    try:
        if not os.path.exists(ca_cert_path):
             raise FileNotFoundError(f"Plik certyfikatu CA nie istnieje: {ca_cert_path}")
        req_session = requests.Session()
        req_session.verify = ca_cert_path
        print(f"Requests Session skonfigurowana do weryfikacji za pomocą CA: {ca_cert_path}")

        session = Transport(timeout=config.REQUEST_TIMEOUT, session=req_session)
        print(f"Zeep Transport używa jawnie skonfigurowanej sesji requests.")
    except FileNotFoundError as fnf_error:
        print(f"KRYTYCZNY BŁĄD: {fnf_error}")
        print("Upewnij się, że plik rootCA.pem znajduje się w podanej ścieżce")
        print("i zaktualizuj zmienną 'ca_cert_path' w pliku soap_client.py.")
        raise RuntimeError(f"Nie można znaleźć certyfikatu CA w '{ca_cert_path}'. Weryfikacja SSL jest wymagana.") from fnf_error
    except Exception as ssl_config_error:
         print(f"KRYTYCZNY BŁĄD podczas konfiguracji SSL: {ssl_config_error}")
         raise RuntimeError(f"Nie udało się skonfigurować weryfikacji SSL: {ssl_config_error}") from ssl_config_error

    try:
        client = zeep.Client(
            wsdl=config.BASE_WSDL,
            transport=session
        )
        print("Pomyślnie połączono z serwisem WCF (HTTPS) i załadowano WSDL.")

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


        print("Inicjalizacja klienta SOAP (HTTPS) zakończona.")
        return client, flight_service, passenger_service, ticket_service

    except (TransportError, ConnectionError) as e:
        print(f"Błąd połączenia SOAP (HTTPS): {e}")
        if 'SSL' in str(e).upper() or 'CERTIFICATE_VERIFY_FAILED' in str(e).upper():
             print(f"Błąd weryfikacji certyfikatu SSL: {e}")
        raise ConnectionError(f"Nie można połączyć się z serwisem WCF (HTTPS) lub pobrać WSDL.\nURL: {config.BASE_WSDL}\nSzczegóły: {e}") from e
    except Exception as e:
        print(f"Błąd inicjalizacji SOAP (HTTPS) lub bindowania: {e}")
        raise RuntimeError(f"Błąd inicjalizacji klienta SOAP (HTTPS) lub bindowania portu:\n{e}") from e