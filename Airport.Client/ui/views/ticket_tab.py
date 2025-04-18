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

        self._create_widgets()

    def _create_widgets(self):
        frame_purchase = ttk.LabelFrame(self, text="Kup Bilet")
        frame_purchase.pack(pady=10, padx=10, fill='x')

        ttk.Label(frame_purchase, text="ID Lotu (GUID):").grid(row=0, column=0, padx=5, pady=5, sticky='w')
        self.entry_ticket_flight_id = ttk.Entry(frame_purchase, width=36)
        self.entry_ticket_flight_id.grid(row=0, column=1, padx=5, pady=5)

        ttk.Label(frame_purchase, text="ID Pasażera (GUID):").grid(row=1, column=0, padx=5, pady=5, sticky='w')
        self.entry_ticket_passenger_id = ttk.Entry(frame_purchase, width=36)
        self.entry_ticket_passenger_id.grid(row=1, column=1, padx=5, pady=5)

        btn_purchase = ttk.Button(frame_purchase, text="Kup Bilet", command=self._on_purchase_click, state=tk.DISABLED)
        btn_purchase.grid(row=2, column=0, columnspan=2, pady=10)
        ttk.Label(frame_purchase, text="Funkcjonalność niedostępna (brak metody w WSDL)").grid(row=3, column=0, columnspan=2, pady=2, sticky='w')

        frame_ticket_list = ttk.LabelFrame(self, text="Bilety Pasażera")
        frame_ticket_list.pack(pady=10, padx=10, fill='both', expand=True)

        btn_list_tickets = ttk.Button(frame_ticket_list, text="Pokaż Bilety Pasażera", command=self._on_list_tickets_click, state=tk.DISABLED)
        btn_list_tickets.pack(pady=5)
        ttk.Label(frame_ticket_list, text="Funkcjonalność niedostępna (brak metody w WSDL)").pack(pady=2)

        cols_tickets = ('ID Biletu', 'Pasażer', 'Lot', 'Data Odlotu', 'Status')
        self.tree_tickets = ttk.Treeview(frame_ticket_list, columns=cols_tickets, show='headings', height=10)
        for col in cols_tickets:
            self.tree_tickets.heading(col, text=col)
            self.tree_tickets.column(col, width=120, anchor='w')
        self.tree_tickets.column('ID Biletu', width=200)
        self.tree_tickets.column('Pasażer', width=150)
        self.tree_tickets.column('Lot', width=180)
        self.tree_tickets.pack(fill='both', expand=True)

    def _clear_tickets_tree(self):
        """Czyści tabelę biletów."""
        for i in self.tree_tickets.get_children():
            self.tree_tickets.delete(i)

    def _update_tickets_tree(self, tickets):
        """Aktualizuje tabelę biletów danymi (obecnie nieużywane)."""
        self._clear_tickets_tree()
        if not tickets:
            return

        for ticket in tickets:
            if ticket and all(hasattr(ticket, attr) for attr in ['Id', 'Name', 'Surname', 'FlightFrom', 'FlightTo', 'DepartureDate', 'ArrivalDate', 'Status']):
                 dep_date_str = utils.format_datetime(ticket.DepartureDate)
                 arr_date_str = utils.format_datetime(ticket.ArrivalDate)
                 pax_name = f"{ticket.Name} {ticket.Surname}"
                 flight_info = f"{ticket.FlightFrom} -> {ticket.FlightTo}"
                 self.tree_tickets.insert('', tk.END, values=(
                     ticket.Id, pax_name, flight_info, dep_date_str, ticket.Status
                 ))
            else:
                 print("Ostrzeżenie UI: Pominięto niekompletny obiekt AirplaneTicketDto.")


    def _on_purchase_click(self):
        """Obsługuje kliknięcie przycisku zakupu biletu."""
        flight_id_str = self.entry_ticket_flight_id.get()
        passenger_id_str = self.entry_ticket_passenger_id.get()

        if not flight_id_str or not passenger_id_str:
            utils.show_warning("Brak danych", "ID lotu i ID pasażera są wymagane.")
            return

        try:
            flight_id = str(uuid.UUID(flight_id_str))
            passenger_id = str(uuid.UUID(passenger_id_str))
        except ValueError:
            utils.show_error("Błąd formatu ID", "Wprowadź poprawne identyfikatory GUID dla lotu i pasażera.")
            return

        try:
            # Ta metoda rzuci NotImplementedError zdefiniowany w AirportApiService
            ticket_id = self.api_service.purchase_ticket(flight_id, passenger_id)
            # Ten kod poniżej się nie wykona, jeśli purchase_ticket rzuca błąd
            # utils.show_info("Sukces", f"Pomyślnie zakupiono bilet.\nNowe ID biletu: {ticket_id}")
            # self.entry_ticket_flight_id.delete(0, tk.END)
            # self.entry_ticket_passenger_id.delete(0, tk.END)
        except NotImplementedError as nie:
            utils.show_error("Funkcja niedostępna", str(nie))
        except Fault as f:
            utils.show_error("Błąd SOAP", f"Błąd serwisu WCF podczas próby zakupu biletu:\n{f.message}")
        except Exception as e:
            utils.show_error("Błąd", f"Wystąpił nieoczekiwany błąd podczas próby zakupu biletu:\n{e}")

    def _on_list_tickets_click(self):
        """Obsługuje kliknięcie przycisku pokazania biletów pasażera."""
        passenger_id_str = utils.ask_string("Pobierz bilety", "Podaj ID pasażera (GUID):")
        if not passenger_id_str:
            return # Użytkownik anulował

        try:
            passenger_id = str(uuid.UUID(passenger_id_str))
        except ValueError:
            utils.show_error("Błąd formatu ID", "Wprowadzono niepoprawny format GUID.")
            return

        try:
             # Ta metoda rzuci NotImplementedError zdefiniowany w AirportApiService
            tickets = self.api_service.get_passenger_tickets(passenger_id)
             # Ten kod poniżej się nie wykona
             # self._update_tickets_tree(tickets)
             # if not tickets:
             #     utils.show_info("Informacja", f"Nie znaleziono biletów dla pasażera o ID: {passenger_id}")
        except NotImplementedError as nie:
             utils.show_error("Funkcja niedostępna", str(nie))
        except Fault as f:
             utils.show_error("Błąd SOAP", f"Błąd serwisu WCF podczas próby pobrania biletów:\n{f.message}")
        except Exception as e:
             utils.show_error("Błąd", f"Wystąpił nieoczekiwany błąd podczas próby pobrania biletów:\n{e}")