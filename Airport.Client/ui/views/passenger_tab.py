import tkinter as tk
from tkinter import ttk
from core.airport_service import AirportApiService
from ui import utils
from zeep.exceptions import Fault

class PassengerTab(ttk.Frame):
    def __init__(self, parent, api_service: AirportApiService, **kwargs):
        super().__init__(parent, **kwargs)
        self.api_service = api_service

        self._create_widgets()
        #self._initial_load()

    def _create_widgets(self):
        frame_add_pax = ttk.LabelFrame(self, text="Dodaj Pasażera")
        frame_add_pax.pack(pady=10, padx=10, fill='x')

        ttk.Label(frame_add_pax, text="Imię:").grid(row=0, column=0, padx=5, pady=5, sticky='w')
        self.entry_pax_name = ttk.Entry(frame_add_pax, width=20)
        self.entry_pax_name.grid(row=0, column=1, padx=5, pady=5)

        ttk.Label(frame_add_pax, text="Nazwisko:").grid(row=1, column=0, padx=5, pady=5, sticky='w')
        self.entry_pax_surname = ttk.Entry(frame_add_pax, width=20)
        self.entry_pax_surname.grid(row=1, column=1, padx=5, pady=5)

        ttk.Label(frame_add_pax, text="PESEL:").grid(row=2, column=0, padx=5, pady=5, sticky='w')
        self.entry_pax_pesel = ttk.Entry(frame_add_pax, width=20)
        self.entry_pax_pesel.grid(row=2, column=1, padx=5, pady=5)

        btn_add_pax = ttk.Button(frame_add_pax, text="Dodaj Pasażera", command=self._on_add_passenger_click)
        btn_add_pax.grid(row=3, column=0, columnspan=2, pady=10)

        frame_pax_list = ttk.LabelFrame(self, text="Lista Pasażerów")
        frame_pax_list.pack(pady=10, padx=10, fill='both', expand=True)

        btn_list_pax = ttk.Button(frame_pax_list, text="Odśwież Listę Pasażerów", command=self._on_list_passengers_click)
        btn_list_pax.pack(pady=5)

        cols_passengers = ('Imię', 'Nazwisko', 'PESEL')
        self.tree_passengers = ttk.Treeview(frame_pax_list, columns=cols_passengers, show='headings', height=10)
        for col in cols_passengers:
            self.tree_passengers.heading(col, text=col)
            self.tree_passengers.column(col, width=150)
        self.tree_passengers.pack(fill='both', expand=True)

    def _clear_passenger_tree(self):
        """Czyści tabelę pasażerów."""
        for i in self.tree_passengers.get_children():
            self.tree_passengers.delete(i)

    def _update_passenger_tree(self, passengers):
        """Aktualizuje tabelę pasażerów danymi."""
        self._clear_passenger_tree()
        if not passengers:
            return

        for pax in passengers:
             if pax and hasattr(pax, 'Name') and hasattr(pax, 'Surname') and hasattr(pax, 'Pesel'):
                 self.tree_passengers.insert('', tk.END, values=(
                     pax.Name,
                     pax.Surname,
                     pax.Pesel
                 ))
             else:
                  print("Ostrzeżenie UI: Pominięto niekompletny obiekt PassengerDTO.")

    def _clear_add_passenger_fields(self):
        """Czyści pola formularza dodawania pasażera."""
        self.entry_pax_name.delete(0, tk.END)
        self.entry_pax_surname.delete(0, tk.END)
        self.entry_pax_pesel.delete(0, tk.END)

    def _on_add_passenger_click(self):
        """Obsługuje kliknięcie przycisku dodania pasażera."""
        name = self.entry_pax_name.get()
        surname = self.entry_pax_surname.get()
        pesel = self.entry_pax_pesel.get()

        if not name or not surname or not pesel:
            utils.show_warning("Brak danych", "Wszystkie pola (Imię, Nazwisko, PESEL) są wymagane.")
            return

        try:
            passenger_id = self.api_service.create_passenger(name, surname, pesel)
            utils.show_info("Sukces", f"Pomyślnie utworzono pasażera.\nNowe ID: {passenger_id}")
            self._clear_add_passenger_fields()
            self._on_list_passengers_click()
        except ValueError as ve:
             utils.show_warning("Błąd danych", str(ve))
        except Fault as f:
            utils.show_error("Błąd SOAP", f"Błąd serwisu WCF podczas tworzenia pasażera:\n{f.message}")
        except Exception as e:
            utils.show_error("Błąd", f"Wystąpił nieoczekiwany błąd podczas tworzenia pasażera:\n{e}")

    def _on_list_passengers_click(self):
        """Obsługuje kliknięcie przycisku odświeżenia listy pasażerów."""
        try:
            passengers = self.api_service.get_all_passengers()
            self._update_passenger_tree(passengers)
            if not passengers:
                utils.show_info("Informacja", "Brak zarejestrowanych pasażerów w systemie.")
        except Fault as f:
            utils.show_error("Błąd SOAP", f"Błąd serwisu WCF podczas pobierania pasażerów:\n{f.message}")
        except Exception as e:
            utils.show_error("Błąd", f"Wystąpił nieoczekiwany błąd podczas pobierania pasażerów:\n{e}")

    def _initial_load(self):
        """Ładuje początkową listę pasażerów."""
        print("Ładowanie początkowej listy pasażerów...")
        self._on_list_passengers_click()