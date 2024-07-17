using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

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
            _filepath = filePath;
            using var reader = new StreamReader(File.OpenRead(filePath));
            _loadedCfg = reader.ReadToEnd();
            _mappedValues = JsonNode.Parse(_loadedCfg)!.AsObject();

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
        public static T? GetSection<T>(string? section = null) where T : IConfigurationSection, new()
        {
            if (!_init) throw new NotInitializedException();

            section ??= typeof(T).Name;
            T ret = new T();
            if(!_mappedValues!.ContainsKey(section))
                throw new InvalidOperationException($"Section {section} is missing");

            ret.FromJSONObject(_mappedValues.Single(x => x.Key == section).Value!.AsObject());
            return ret;
        }

        /// <summary>
        /// Update loaded in-memory config and update it file on disc
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NotInitializedException"
        public static bool SetSection<T>(T value, string? section = null) where T : IConfigurationSection
        {
            if (!_init) throw new NotInitializedException();

            lock (_lock)
            {
                section ??= typeof(T).Name;
                _mappedValues!.Remove(section);
                _mappedValues![section] = value.ToJSONObject();
                _loadedCfg = _mappedValues.ToString();
                using var writer = new StreamWriter(File.Create(_filepath!));
                writer.Write(_loadedCfg);
                return true;
            }
        }


        /// <summary>
        /// Thrown when application tries to use service that was not properly initialized
        /// </summary>
        private class NotInitializedException : Exception
        {
            public override string Message => "Service requiered initialization before usage";
        }
    }
}
