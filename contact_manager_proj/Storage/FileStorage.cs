using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Contacting
{
    public class FileStorage<T> : IStorage<T>
    {
        private readonly string _path;
        private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

        public FileStorage(string path) => _path = path;

        public async Task SaveAsync(IEnumerable<T> items)
        {
            try
            {
                string json = JsonSerializer.Serialize(items, Options);
                await File.WriteAllTextAsync(_path, json);
            }
            catch (IOException ex)
            {
                throw new Exception($"Failed to save to '{_path}': {ex.Message}", ex);
            }
        }

        public async Task<List<T>> LoadAsync()
        {
            try
            {
                if (!File.Exists(_path)) return new List<T>();

                string json = await File.ReadAllTextAsync(_path);
                return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            }
            catch (IOException ex)
            {
                throw new Exception($"Failed to load from '{_path}': {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception($"Invalid JSON in '{_path}': {ex.Message}", ex);
            }
        }
    }
}
