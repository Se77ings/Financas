using System.Text.Json;

namespace Financas.Desktop.Settings
{
    public sealed class DesktopSettingsStore
    {
        private readonly string _path;
        private readonly Lock _gate = new();
        private DesktopSettings _current;

        public DesktopSettingsStore(string path)
        {
            _path = path;
            _current = Load(path);
        }

        public DesktopSettings Current
        {
            get
            {
                lock (_gate) return _current;
            }
        }

        public DesktopSettings Update(Func<DesktopSettings, DesktopSettings> change)
        {
            lock (_gate)
            {
                var updated = change(_current);
                Save(updated);
                _current = updated;
                return updated;
            }
        }

        private static DesktopSettings Load(string path)
        {
            if (!File.Exists(path)) return new DesktopSettings();

            try
            {
                return JsonSerializer.Deserialize<DesktopSettings>(File.ReadAllText(path)) ?? new DesktopSettings();
            }
            catch (JsonException)
            {
                return new DesktopSettings();
            }
        }

        private void Save(DesktopSettings settings)
        {
            var temporary = _path + ".tmp";
            File.WriteAllText(temporary, JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));
            File.Move(temporary, _path, overwrite: true);
        }
    }
}
