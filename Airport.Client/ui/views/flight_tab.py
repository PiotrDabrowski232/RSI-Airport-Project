import tkinter as tk
from tkinter import ttk
from datetime import datetime
from core.airport_service import AirportApiService
from ui import utils
from zeep.exceptions import Fault

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

        frame_results = ttk.LabelFrame(self, text="Wyniki Wyszukiwania / Lista Lotów")
        frame_results.pack(pady=10, padx=10, fill='both', expand=True)
        cols_flights = ('Skąd', 'Dokąd', 'Odlot', 'Przylot')
        self.tree_flights = ttk.Treeview(frame_results, columns=cols_flights, show='headings', height=10)
        for col in cols_flights:
            self.tree_flights.heading(col, text=col)
            self.tree_flights.column(col, width=150)
        self.tree_flights.pack(fill='both', expand=True)

    def _clear_flights_tree(self):
        """Czyści tabelę lotów."""
        for i in self.tree_flights.get_children():
            self.tree_flights.delete(i)

    def _update_flights_tree(self, flights):
        """Aktualizuje tabelę lotów danymi."""
        self._clear_flights_tree()
        if not flights:
            utils.show_info("Informacja", "Nie znaleziono lotów.")
            return

        for flight in flights:
             if flight and hasattr(flight, 'DepartureDate') and hasattr(flight, 'ArrivalDate') \
               and hasattr(flight, 'FlightFrom') and hasattr(flight, 'FlightTo'):
                 dep_date_str = utils.format_datetime(flight.DepartureDate)
                 arr_date_str = utils.format_datetime(flight.ArrivalDate)
                 self.tree_flights.insert('', tk.END, values=(
                     flight.FlightFrom, flight.FlightTo, dep_date_str, arr_date_str
                 ))
             else:
                  print("Ostrzeżenie UI: Pominięto niekompletny obiekt FlightDTO.")

    def _on_search_click(self):
        from_val = self.entry_from.get()
        to_val = self.entry_to.get()
        date_str = self.entry_date.get()
        departure_date = None
        if date_str:
            try:
                departure_date = datetime.strptime(date_str, '%Y-%m-%d')
            except ValueError:
                utils.show_error("Błąd formatu daty", "Wprowadź datę w formacie RRRR-MM-DD.")
                return
        try:
            results = self.api_service.search_flights(from_val, to_val, departure_date)
            self._update_flights_tree(results)
            if not results:
                 utils.show_info("Informacja", "Nie znaleziono lotów spełniających kryteria.")
        except Fault as f:
            utils.show_error("Błąd SOAP", f"Błąd serwisu WCF:\n{f.message}")
        except Exception as e:
            utils.show_error("Błąd", f"Wystąpił nieoczekiwany błąd podczas wyszukiwania lotów:\n{e}")

    def _on_show_all_click(self):
        try:
            results = self.api_service.get_all_flights()
            self._update_flights_tree(results)
            if not results:
                 utils.show_info("Informacja", "Nie znaleziono żadnych lotów w systemie.")
        except Fault as f:
            utils.show_error("Błąd SOAP", f"Błąd serwisu WCF:\n{f.message}")
        except Exception as e:
            utils.show_error("Błąd", f"Wystąpił nieoczekiwany błąd podczas pobierania lotów:\n{e}")