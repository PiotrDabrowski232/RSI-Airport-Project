import tkinter as tk
from tkinter import ttk
import uuid
from core.airport_service import AirportApiService
from ui import utils
from zeep.exceptions import Fault

class TicketTab(ttk.Frame):
    def __init__(self, parent, api_service: AirportApiService, **kwargs):
        super().__init__(parent, **kwargs)
        self.api_service = api_service
        self._ticket_details_labels = {}
        self._create_widgets()

    def _create_widgets(self):
        frame_verify = ttk.LabelFrame(self, text="Weryfikacja Biletu")
        frame_verify.pack(pady=10, padx=10, fill='x')

        ttk.Label(frame_verify, text="ID Biletu (GUID):").grid(row=0, column=0, padx=5, pady=5, sticky='w')
        self.entry_verify_ticket_id = ttk.Entry(frame_verify, width=36)
        self.entry_verify_ticket_id.grid(row=0, column=1, padx=5, pady=5, sticky='ew')

        btn_verify = ttk.Button(frame_verify, text="Sprawdź Bilet", command=self._on_verify_ticket_click)
        btn_verify.grid(row=0, column=2, padx=5, pady=5)

        frame_verify.columnconfigure(1, weight=1)

        self.details_display_frame = ttk.LabelFrame(self, text="Szczegóły Biletu")
        self.details_display_frame.pack(pady=10, padx=10, fill='both', expand=True)

    def _clear_ticket_details(self):
        for widget in self.details_display_frame.winfo_children():
            widget.destroy()
        self._ticket_details_labels = {}

    def _display_ticket_details(self, ticket_dto):
        self._clear_ticket_details()
        if not ticket_dto:
            ttk.Label(self.details_display_frame, text="Nie znaleziono biletu o podanym ID.").pack(padx=5, pady=5)
            return

        details_map = {
            "ID Biletu": ticket_dto.Id,
            "Pasażer": f"{getattr(ticket_dto, 'Name', 'Brak')} {getattr(ticket_dto, 'Surname', 'Brak')}",
            "PESEL": getattr(ticket_dto, 'Pesel', 'N/A'),
            "Lot": f"{getattr(ticket_dto, 'FlightFrom', 'N/A')} -> {getattr(ticket_dto, 'FlightTo', 'N/A')}",
            "Odlot": utils.format_datetime(getattr(ticket_dto, 'DepartureDate', None)),
            "Przylot": utils.format_datetime(getattr(ticket_dto, 'ArrivalDate', None)),
            "Status": getattr(ticket_dto, 'Status', 'N/A')
        }

        row_num = 0
        for label_text, value_text in details_map.items():
             lbl_name = ttk.Label(self.details_display_frame, text=f"{label_text}:", font=('TkDefaultFont', 9, 'bold'))
             lbl_name.grid(row=row_num, column=0, padx=5, pady=2, sticky='nw')
             lbl_value = ttk.Label(self.details_display_frame, text=str(value_text), wraplength=350) # wraplength dla długich ID
             lbl_value.grid(row=row_num, column=1, padx=5, pady=2, sticky='nw')
             self._ticket_details_labels[label_text] = lbl_value # Zapisz referencję
             row_num += 1
        self.details_display_frame.columnconfigure(1, weight=1)


    def _on_verify_ticket_click(self):
        ticket_id_str = self.entry_verify_ticket_id.get().strip()
        if not ticket_id_str:
            utils.show_warning("Brak danych", "Wprowadź ID biletu do weryfikacji.", parent=self)
            return

        try:
            ticket_id = uuid.UUID(ticket_id_str)
        except ValueError:
            utils.show_error("Błędny format ID", "Wprowadź poprawny identyfikator GUID biletu.", parent=self)
            self._clear_ticket_details()
            return

        try:
            self._clear_ticket_details()
            ticket_dto = self.api_service.get_ticket_by_id(ticket_id)
            self._display_ticket_details(ticket_dto) # Wyświetl wynik (lub info o braku)
        except ValueError as ve:
             utils.show_warning("Błąd danych", str(ve), parent=self)
        except Fault as f:
             utils.show_error("Błąd SOAP", f"Błąd serwisu WCF podczas weryfikacji biletu:\n{f.message}", parent=self)
             self._clear_ticket_details()
        except NotImplementedError as nie:
             utils.show_error("Funkcja niedostępna", str(nie), parent=self)
             self._clear