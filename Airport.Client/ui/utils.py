from datetime import datetime
from tkinter import messagebox, simpledialog

def show_error(title, message):
    messagebox.showerror(title, message)

def show_info(title, message):
    messagebox.showinfo(title, message)

def show_warning(title, message):
    messagebox.showwarning(title, message)

def ask_string(title, prompt):
    return simpledialog.askstring(title, prompt)

# ui/utils.py
# ... inne importy ...
# BRAKUJE: from datetime import datetime

# ... definicje show_error, etc ...

def format_datetime(dt):
    """Bezpiecznie formatuje datę/czas lub zwraca 'B/D'."""
    if dt and isinstance(dt, datetime):
         try:
             return dt.strftime('%Y-%m-%d %H:%M')
         except ValueError:
             return 'Błąd daty'
    return 'B/D'