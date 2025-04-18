import tkinter as tk
from tkinter import ttk
from infrastructure.soap_client import initialize_soap_client
from core.airport_service import AirportApiService
from ui.views.flight_tab import FlightTab
from ui.views.passenger_tab import PassengerTab
from ui.views.ticket_tab import TicketTab
from ui import utils

class AirportClientApp:
    def __init__(self):
        self.root = tk.Tk()
        self.root.title("Klient Serwisu Lotniska (SOAP) - Refactored")

        self.api_service = None
        self.flight_service_proxy = None
        self.passenger_service_proxy = None

        if not self._initialize_services():
            self.root.destroy()
            return

        self._create_widgets()

    def _initialize_services(self):
        """Inicjalizuje klienta SOAP i serwis API."""
        try:
            self.flight_service_proxy, self.passenger_service_proxy = initialize_soap_client()
            self.api_service = AirportApiService(self.flight_service_proxy, self.passenger_service_proxy)
            return True
        except (ConnectionError, RuntimeError, Exception) as e:
            utils.show_error("Krytyczny błąd inicjalizacji", str(e))
            return False

    def _create_widgets(self):
        """Tworzy główne widgety aplikacji (notebook i zakładki)."""
        notebook = ttk.Notebook(self.root)

        flight_tab = FlightTab(notebook, self.api_service)
        passenger_tab = PassengerTab(notebook, self.api_service)
        ticket_tab = TicketTab(notebook, self.api_service)

        notebook.add(flight_tab, text='Loty')
        notebook.add(passenger_tab, text='Pasażerowie')
        notebook.add(ticket_tab, text='Bilety')

        notebook.pack(expand=True, fill='both', padx=10, pady=10)

    def run(self):
        """Uruchamia główną pętlę Tkinter."""
        if self.root.winfo_exists():
             self.root.mainloop()