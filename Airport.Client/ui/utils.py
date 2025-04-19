from tkinter import messagebox, simpledialog
from datetime import datetime

def show_error(title, message, **kwargs):
    """Wyświetla okno błędu, przekazując dodatkowe argumenty."""
    messagebox.showerror(title, message, **kwargs)

def show_info(title, message, **kwargs): # <--- Upewnij się, że jest **kwargs
    """Wyświetla okno informacyjne, przekazując dodatkowe argumenty."""
    messagebox.showinfo(title, message, **kwargs) # <--- Upewnij się, że **kwargs jest przekazywane

def show_warning(title, message, **kwargs): # <--- Upewnij się, że jest **kwargs
    """Wyświetla okno ostrzeżenia, przekazując dodatkowe argumenty."""
    messagebox.showwarning(title, message, **kwargs) # <--- Upewnij się, że **kwargs jest przekazywane

def ask_string(title, prompt, **kwargs): # <--- simpledialog.askstring też akceptuje np. parent
    """Wyświetla okno dialogowe do wpisania tekstu."""
    return simpledialog.askstring(title, prompt, **kwargs) # <--- Upewnij się, że **kwargs jest przekazywane

def format_datetime(dt):
    """Bezpiecznie formatuje datę/czas lub zwraca 'B/D'."""
    if dt and isinstance(dt, datetime):
         try:
             return dt.strftime('%Y-%m-%d %H:%M')
         except ValueError:
             return 'Błąd daty'
    return 'B/D'