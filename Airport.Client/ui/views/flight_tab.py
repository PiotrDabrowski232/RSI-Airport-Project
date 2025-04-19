import tkinter as tk
from tkinter import ttk
from datetime import datetime
import uuid
from core.airport_service import AirportApiService
from ui import utils
from zeep.exceptions import Fault
from ui.dialogs.reservation_dialog import ReservationDialog

class FlightTab(ttk.Frame):
    def __init__(self, parent, api_service: AirportApiService, **kwargs):
        super().__init__(parent, **kwargs)
        self.api_service = api_service
        self._create_widgets()

    def _create_widgets(self):
        frame_search = ttk.LabelFrame(self, text="Wyszukaj Lot")
        frame_search.pack(pady=10, padx=10, fill='x')

        ttk.Label(frame_search, text="Skąd:").grid(row=0, column=0, padx=5, pady=5, sticky='w')
        self.entry_from = ttk.Entry(frame_search, width=15)
        self.entry_from.grid(row=0, column=1, padx=5, pady=5)
        ttk.Label(frame_search, text="Dokąd:").grid(row=0, column=2, padx=5, pady=5, sticky='w')
        self.entry_to = ttk.Entry(frame_search, width=15)
        self.entry_to.grid(row=0, column=3, padx=5, pady=5)
        ttk.Label(frame_search, text="Data (RRRR-MM-DD):").grid(row=1, column=0, padx=5, pady=5, sticky='w')
        self.entry_date = ttk.Entry(frame_search, width=15)
        self.entry_date.grid(row=1, column=1, padx=5, pady=5)

        btn_search = ttk.Button(frame_search, text="Szukaj", command=self._on_search_click)
        btn_search.grid(row=1, column=2, padx=(5, 2), pady=10)
        btn_list_all = ttk.Button(frame_search, text="Pokaż wszystkie", command=self._on_show_all_click)
        btn_list_all.grid(row=1, column=3, padx=(2, 5), pady=10)

        frame_results = ttk.LabelFrame(self, text="Lista Lotów (Kliknij dwukrotnie, aby zarezerwować)")
        frame_results.pack(pady=10, padx=10, fill='both', expand=True)

        self.cols_flights_display = ('Skąd', 'Dokąd', 'Odlot', 'Przylot')
        self.cols_flights_all = ('Skąd', 'Dokąd', 'Odlot', 'Przylot', 'ID')
        self.tree_flights = ttk.Treeview(
            frame_results,
            columns=self.cols_flights_display,
            displaycolumns=self.cols_flights_display,
            show='headings',
            height=10
        )
        for col in self.cols_flights_display:
            self.tree_flights.heading(col, text=col)
            self.tree_flights.column(col, width=150)

        self.tree_flights.bind("<Double-1>", self._on_flight_selected)
        self.tree_flights.pack(fill='both', expand=True)

    def _clear_flights_tree(self):
        for i in self.tree_flights.get_children(): self.tree_flights.delete(i)

    def _update_flights_tree(self, flights):
        self._clear_flights_tree()
        if not flights: return
        for flight in flights:
             if flight and all(hasattr(flight, attr) for attr in ['FlightFrom', 'FlightTo', 'DepartureDate', 'ArrivalDate', 'Id']):
                 dep_date_str = utils.format_datetime(flight.DepartureDate)
                 arr_date_str = utils.format_datetime(flight.ArrivalDate)
                 flight_data = (
                     flight.FlightFrom, flight.FlightTo, dep_date_str, arr_date_str, str(flight.Id)
                 )
                 self.tree_flights.insert('', tk.END, values=flight_data)
             else:
                 print("Ostrzeżenie UI: Pominięto niekompletny obiekt FlightDTO (brak ID?).")

    def _on_search_click(self):
        from_val = self.entry_from.get(); to_val = self.entry_to.get(); date_str = self.entry_date.get()
        departure_date = None
        if date_str:
            try: departure_date = datetime.strptime(date_str, '%Y-%m-%d')
            except ValueError: utils.show_error("Błąd formatu daty", "Wprowadź datę w formacie RRRR-MM-DD."); return
        try:
            results = self.api_service.search_flights(from_val, to_val, departure_date)
            self._update_flights_tree(results)
            if not results: utils.show_info("Informacja", "Nie znaleziono lotów spełniających kryteria.")
        except Fault as f: utils.show_error("Błąd SOAP", f"Błąd serwisu WCF:\n{f.message}")
        except Exception as e: utils.show_error("Błąd", f"Wystąpił błąd podczas wyszukiwania: {e}")

    def _on_show_all_click(self):
        try:
            results = self.api_service.get_all_flights()
            self._update_flights_tree(results)
            if not results: utils.show_info("Informacja", "Nie znaleziono żadnych lotów w systemie.")
        except Fault as f: utils.show_error("Błąd SOAP", f"Błąd serwisu WCF:\n{f.message}")
        except Exception as e: utils.show_error("Błąd", f"Wystąpił błąd podczas pobierania lotów: {e}")

    def _on_flight_selected(self, event):
        selected_item = self.tree_flights.focus()
        if not selected_item: return
        item_values = self.tree_flights.item(selected_item, 'values')
        if not item_values or len(item_values) < len(self.cols_flights_all):
             utils.show_error("Błąd", "Nie można pobrać danych dla wybranego lotu.")
             return
        try:
            flight_id = uuid.UUID(item_values[-1])
            flight_details_display = {
                 "Skąd": item_values[0], "Dokąd": item_values[1],
                 "Odlot": item_values[2], "Przylot": item_values[3],
                 "ID": str(flight_id)
            }
            print(f"Wybrano lot: {flight_details_display}")
            dialog = ReservationDialog(self, flight_id, flight_details_display, self.api_service)
        except (ValueError, IndexError) as e: utils.show_error("Błąd danych", f"Nie można przetworzyć danych wybranego lotu: {e}")
        except Exception as e: utils.show_error("Błąd", f"Wystąpił nieoczekiwany błąd: {e}")