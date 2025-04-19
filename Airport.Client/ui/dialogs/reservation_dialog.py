import tkinter as tk
from tkinter import ttk
import uuid
from core.airport_service import AirportApiService
from ui import utils
from zeep.exceptions import Fault

class ReservationDialog(tk.Toplevel):
    def __init__(self, parent, flight_id: uuid.UUID, flight_details: dict, api_service: AirportApiService):
        super().__init__(parent)
        self.title("Rezerwacja Biletu")
        self.parent = parent
        self.flight_id = flight_id
        self.flight_details = flight_details
        self.api_service = api_service
        self.resizable(False, False)
        self._create_widgets()
        self.grab_set()
        self.transient(parent)
        self.wait_window(self)

    def _create_widgets(self):
        main_frame = ttk.Frame(self, padding="10")
        main_frame.pack(expand=True, fill="both")
        details_frame = ttk.LabelFrame(main_frame, text="Szczegóły Lotu")
        details_frame.pack(fill="x", pady=5)
        ttk.Label(details_frame, text=f"Skąd: {self.flight_details.get('Skąd', 'N/A')}").pack(anchor='w')
        ttk.Label(details_frame, text=f"Dokąd: {self.flight_details.get('Dokąd', 'N/A')}").pack(anchor='w')
        ttk.Label(details_frame, text=f"Odlot: {self.flight_details.get('Odlot', 'N/A')}").pack(anchor='w')
        ttk.Label(details_frame, text=f"ID Lotu: {self.flight_details.get('ID', 'N/A')}").pack(anchor='w')

        passenger_frame = ttk.LabelFrame(main_frame, text="Dane Pasażera")
        passenger_frame.pack(fill="x", pady=5)
        ttk.Label(passenger_frame, text="Imię:").grid(row=0, column=0, padx=5, pady=2, sticky='w')
        self.entry_name = ttk.Entry(passenger_frame, width=30)
        self.entry_name.grid(row=0, column=1, padx=5, pady=2)
        ttk.Label(passenger_frame, text="Nazwisko:").grid(row=1, column=0, padx=5, pady=2, sticky='w')
        self.entry_surname = ttk.Entry(passenger_frame, width=30)
        self.entry_surname.grid(row=1, column=1, padx=5, pady=2)
        ttk.Label(passenger_frame, text="PESEL:").grid(row=2, column=0, padx=5, pady=2, sticky='w')
        self.entry_pesel = ttk.Entry(passenger_frame, width=30)
        self.entry_pesel.grid(row=2, column=1, padx=5, pady=2)

        button_frame = ttk.Frame(main_frame)
        button_frame.pack(pady=10)
        reserve_button = ttk.Button(button_frame, text="Rezerwuj", command=self._on_reserve_click)
        reserve_button.pack(side=tk.LEFT, padx=5)
        cancel_button = ttk.Button(button_frame, text="Anuluj", command=self.destroy)
        cancel_button.pack(side=tk.LEFT, padx=5)
        self.entry_name.focus_set()

    def _on_reserve_click(self):
        name = self.entry_name.get().strip(); surname = self.entry_surname.get().strip(); pesel = self.entry_pesel.get().strip()
        if not all([name, surname, pesel]): utils.show_warning("Brak danych", "Wszystkie dane pasażera (Imię, Nazwisko, PESEL) są wymagane.", parent=self); return
        if len(pesel) != 11 or not pesel.isdigit(): utils.show_warning("Błędny PESEL", "PESEL musi składać się z 11 cyfr.", parent=self); return
        try:
            ticket_id = self.api_service.reserve_ticket(self.flight_id, name, surname, pesel)
            utils.show_info("Rezerwacja Zakończona", f"Pomyślnie zarezerwowano bilet!\nID Biletu: {ticket_id}", parent=self.parent.master)
            self.destroy()
        except ValueError as ve: utils.show_warning("Błąd danych", str(ve), parent=self)
        except NotImplementedError as nie: utils.show_error("Funkcja niedostępna", str(nie), parent=self)
        except Fault as f: utils.show_error("Błąd SOAP", f"Błąd serwisu WCF podczas rezerwacji:\n{f.message}", parent=self)
        except Exception as e: utils.show_error("Błąd", f"Wystąpił nieoczekiwany błąd podczas rezerwacji:\n{e}", parent=self)