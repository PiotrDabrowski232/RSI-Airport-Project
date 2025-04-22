# ticket_tab.py
import tkinter as tk
from tkinter import ttk, filedialog
import uuid
import os
from core.airport_service import AirportApiService
from ui import utils
from zeep.exceptions import Fault

class TicketTab(ttk.Frame):
    def __init__(self, parent, api_service: AirportApiService, **kwargs):
        super().__init__(parent, **kwargs)
        self.api_service = api_service
        self._ticket_details_labels = {}
        self.current_ticket_id = None
        self._create_widgets()

    def _create_widgets(self):
        frame_verify = ttk.LabelFrame(self, text="Weryfikacja Biletu")
        frame_verify.pack(pady=10, padx=10, fill='x')

        ttk.Label(frame_verify, text="ID Biletu (GUID):").grid(row=0, column=0, padx=5, pady=5, sticky='w')
        self.entry_verify_ticket_id = ttk.Entry(frame_verify, width=36)
        self.entry_verify_ticket_id.grid(row=0, column=1, padx=5, pady=5, sticky='ew')

        self.btn_verify = ttk.Button(frame_verify, text="Sprawdź Bilet", command=self._on_verify_ticket_click)
        self.btn_verify.grid(row=0, column=2, padx=(5, 0), pady=5)

        self.btn_download_pdf = ttk.Button(frame_verify, text="Pobierz PDF", command=self._on_download_pdf_click, state=tk.DISABLED)
        self.btn_download_pdf.grid(row=0, column=3, padx=(5, 5), pady=5)

        frame_verify.columnconfigure(1, weight=1)

        self.details_display_frame = ttk.LabelFrame(self, text="Szczegóły Biletu")
        self.details_display_frame.pack(pady=10, padx=10, fill='both', expand=True)


    def _clear_ticket_details(self):
        for widget in self.details_display_frame.winfo_children():
            widget.destroy()
        self._ticket_details_labels = {}
        self.current_ticket_id = None
        self.btn_download_pdf.config(state=tk.DISABLED)


    def _display_ticket_details(self, ticket_dto):
        self._clear_ticket_details()
        if not ticket_dto:
            ttk.Label(self.details_display_frame, text="Nie znaleziono biletu o podanym ID.").pack(padx=5, pady=5)
            return

        self.current_ticket_id = ticket_dto.Id
        self.btn_download_pdf.config(state=tk.NORMAL)

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
             lbl_value = ttk.Label(self.details_display_frame, text=str(value_text), wraplength=350)
             lbl_value.grid(row=row_num, column=1, padx=5, pady=2, sticky='nw')
             self._ticket_details_labels[label_text] = lbl_value
             row_num += 1
        self.details_display_frame.columnconfigure(1, weight=1)


    def _on_verify_ticket_click(self):
        ticket_id_str = self.entry_verify_ticket_id.get().strip()
        if not ticket_id_str:
            utils.show_warning("Brak danych", "Wprowadź ID biletu do weryfikacji.", parent=self)
            return

        try:
            ticket_id_uuid = uuid.UUID(ticket_id_str)
        except ValueError:
            utils.show_error("Błędny format ID", "Wprowadź poprawny identyfikator GUID biletu.", parent=self)
            self._clear_ticket_details()
            return

        try:
            self._clear_ticket_details()
            ticket_dto = self.api_service.get_ticket_by_id(ticket_id_uuid)
            self._display_ticket_details(ticket_dto)
        except ValueError as ve:
             utils.show_warning("Błąd danych", str(ve), parent=self)
        except Fault as f:
             utils.show_error("Błąd SOAP", f"Błąd serwisu WCF podczas weryfikacji biletu:\n{f.message}", parent=self)
             self._clear_ticket_details()
        except NotImplementedError as nie:
             utils.show_error("Funkcja niedostępna", str(nie), parent=self)
             self._clear_ticket_details()
        except ConnectionError as ce:
             utils.show_error("Błąd Połączenia", f"Nie można połączyć się z serwisem: {ce}", parent=self)
             self._clear_ticket_details()
        except RuntimeError as re:
             utils.show_error("Błąd Wewnętrzny", f"Błąd wykonania: {re}", parent=self)
             self._clear_ticket_details()
        except Exception as e:
             utils.show_error("Nieoczekiwany Błąd", f"Wystąpił nieoczekiwany błąd: {e}", parent=self)
             self._clear_ticket_details()

    def _on_download_pdf_click(self):
        if not self.current_ticket_id:
            utils.show_warning("Brak Biletu", "Najpierw wyszukaj i wyświetl szczegóły biletu.", parent=self)
            return

        try:
            ticket_id_uuid = uuid.UUID(str(self.current_ticket_id))
        except ValueError:
            utils.show_error("Błąd ID Biletu", "Wewnętrzny błąd: Nieprawidłowe ID bieżącego biletu.", parent=self)
            return

        try:
            pdf_bytes = self.api_service.get_ticket_pdf(ticket_id_uuid)

            if not pdf_bytes:
                utils.show_warning("Brak PDF", "Serwis nie zwrócił danych PDF dla tego biletu.", parent=self)
                return

            default_filename = f"Potwierdzenie_{self.current_ticket_id}.pdf"
            filepath = filedialog.asksaveasfilename(
                parent=self,
                title="Zapisz potwierdzenie PDF",
                initialfile=default_filename,
                defaultextension=".pdf",
                filetypes=[("Pliki PDF", "*.pdf"), ("Wszystkie pliki", "*.*")]
            )

            if filepath:
                try:
                    with open(filepath, 'wb') as f:
                        f.write(pdf_bytes)
                    utils.show_info("Zapisano PDF", f"Potwierdzenie biletu zostało zapisane jako:\n{os.path.basename(filepath)}", parent=self)
                except IOError as ioe:
                    utils.show_error("Błąd Zapisu", f"Nie można zapisać pliku PDF:\n{ioe}", parent=self)
                except Exception as e_write:
                     utils.show_error("Błąd Zapisu", f"Nieoczekiwany błąd podczas zapisu pliku PDF:\n{e_write}", parent=self)

        except ValueError as ve:
             utils.show_warning("Błąd danych", str(ve), parent=self)
        except Fault as f:
             utils.show_error("Błąd SOAP", f"Błąd serwisu WCF podczas pobierania PDF:\n{f.message}", parent=self)
        except NotImplementedError as nie:
             utils.show_error("Funkcja niedostępna", str(nie), parent=self)
        except ConnectionError as ce:
             utils.show_error("Błąd Połączenia", f"Nie można połączyć się z serwisem: {ce}", parent=self)
        except RuntimeError as re:
             utils.show_error("Błąd Wewnętrzny", f"Błąd wykonania: {re}", parent=self)
        except Exception as e:
             utils.show_error("Nieoczekiwany Błąd", f"Wystąpił nieoczekiwany błąd podczas pobierania PDF:\n{e}", parent=self)