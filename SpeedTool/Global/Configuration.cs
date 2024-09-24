using System.Text.Json;
using System.Text.Json.Nodes;

namespace SpeedTool.Global
{
    /// <summary>
    /// Global access to application configuration
    /// </summary>
    public static class Configuration
    {
        private static string? _filepath;
        private static JsonObject? _mappedValues;
        private static string? _loadedCfg;
        private static bool _init = false;

        private static object _lock = new object();

        /// <summary>
        /// Initialise global configuration service
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool Init(string filePath)
        {
            if (_init) return _init;
            if(!File.Exists(filePath))
            {
                DeployDefaultConfiguration(filePath);
                return Init(filePath);
            }
            _filepath = filePath;
            using var reader = new StreamReader(File.OpenRead(filePath));
            _loadedCfg = reader.ReadToEnd();
            try
            {
                _mappedValues = JsonNode.Parse(_loadedCfg)!.AsObject();
            }
            catch
            {
                // Unable to load config, restore defaults
                DeployDefaultConfiguration(filePath);
                return Init(filePath);
            }
            _init = true;
            return _init;
        }

        /// <summary>
        /// Trying to deserialise part of configuration and return object of type T
        /// </summary>
        /// <typeparam name="T">Configuration class</typeparam>
        /// <param name="section">Name of section in configuration file. If null Type.Name will be taken</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotInitializedException"></exception>
        public static T? GetSection<T>(string? section = null) where T : class, IConfigurationSection, new()
        {
            if (!_init) throw new NotInitializedException();

            section ??= typeof(T).Name;
            if(!_mappedValues!.ContainsKey(section))
                return new();

            var value = _mappedValues.Single(x => x.Key == section).Value!;
            return JsonSerializer.Deserialize(value, typeof(T), SourceGeneratorContext.Default) as T;
        }

        /// <summary>
        /// Update loaded in-memory config and update it file on disc
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NotInitializedException"
        public static bool SetSection<T>(T value, string? section = null) where T : class, IConfigurationSection
        {

            if (!_init) throw new NotInitializedException();

            lock (_lock)
            {
                section ??= typeof(T).Name;
                _mappedValues!.Remove(section);
                _mappedValues![section] = JsonSerializer.SerializeToNode(value, typeof(T), SourceGeneratorContext.Default);
                _loadedCfg = _mappedValues.ToString();
                using var writer = new StreamWriter(File.Create(_filepath!));
                writer.Write(_loadedCfg);
                NotifyConfigurationChanged(value);
                return true;
            }
        }

        private static void DeployDefaultConfiguration(string filePath)
        {
            var stream = typeof(Program).Assembly.GetManifestResourceStream(RESOURCE_NAME)!;
            var sr = new StreamReader(stream);
            File.WriteAllText(filePath, sr.ReadToEnd());
        }

        public static void NotifyConfigurationChanged(IConfigurationSection section)
        {
            OnConfigurationChanged?.Invoke(Platform.Platform.SharedPlatform, section);
        }

        public static event EventHandler<IConfigurationSection>? OnConfigurationChanged;

        /// <summary>
        /// Thrown when application tries to use service that was not properly initialized
        /// </summary>
        private class NotInitializedException : Exception
        {
            public override string Message => "Service requiered initialization before usage";
        }

        private const string RESOURCE_NAME = "SpeedTool.App.appsettings.json";
    }
}
