import gi
import threading
import time
from gi.repository import Gtk, Adw

gi.require_version("Gtk", "4.0")
gi.require_version("Adw", "1")


class Ui():
    def GetConfigRow(title: str, actionsEnum: list[list[str]], onInputChanged: callable):
        prefRow = Adw.PreferencesRow(title=title)
        prefRow.storeModel = Gtk.ListStore.new([str, str])

        for action in actionsEnum:
            prefRow.storeModel.append([action[1], action[0]])

        prefRow.box = Gtk.Box(
            orientation=Gtk.Orientation.HORIZONTAL,
            margin_start=10,
            margin_end=10,
            margin_top=10,
            margin_bottom=10
        )

        prefRow.cellRenderer = Gtk.CellRendererText()
        prefRow.input = Gtk.ComboBox.new_with_model(prefRow.storeModel)
        prefRow.input.pack_start(prefRow.cellRenderer, True)
        prefRow.input.add_attribute(prefRow.cellRenderer, "text", 0)

        if onInputChanged is not None:
            prefRow.input.connect("changed", onInputChanged)

        prefRow.set_child(prefRow.box)
        prefRow.box.append(Gtk.Label(
            label="ToggleAction:",
            hexpand=True,
            xalign=0
        ))

        prefRow.box.append(prefRow.input)

        return prefRow


# This class was vibe-coded IDC, I hate Python.
# In C# this would only take a few lines...
class TrailingThrottle:
    def __init__(self, func: callable, interval_ms: int):
        self.func = func
        self.interval = interval_ms / 1000.0

        self._lock = threading.Lock()

        self._last_run = 0.0
        self._timer = None

        self._latest_args = None
        self._latest_kwargs = None

    def __call__(self, *args, **kwargs):
        now = time.monotonic()

        with self._lock:
            elapsed = now - self._last_run

            # Execute immediately if outside throttle window
            if elapsed >= self.interval and self._timer is None:
                self._last_run = now
                threading.Thread(
                    target=self.func,
                    args=args,
                    kwargs=kwargs,
                    daemon=True,
                ).start()
                return

            # Store latest call
            self._latest_args = args
            self._latest_kwargs = kwargs

            # Schedule trailing execution if not already scheduled
            if self._timer is None:
                delay = max(0, self.interval - elapsed)

                self._timer = threading.Timer(
                    delay,
                    self._run_trailing
                )
                self._timer.daemon = True
                self._timer.start()

    def _run_trailing(self):
        while True:
            with self._lock:
                args = self._latest_args
                kwargs = self._latest_kwargs

                self._latest_args = None
                self._latest_kwargs = None

                self._last_run = time.monotonic()

            if args is not None:
                self.func(*args, **kwargs)

            time.sleep(self.interval)

            with self._lock:
                # No new calls arrived while waiting
                if self._latest_args is None:
                    self._timer = None
                    return
