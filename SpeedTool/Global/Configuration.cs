﻿using System.Text.Json;
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

        private static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            AllowTrailingCommas = true,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString | System.Text.Json.Serialization.JsonNumberHandling.WriteAsString | System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
        };

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
            _mappedValues = JsonSerializer.Deserialize<JsonObject>(_loadedCfg, options: _jsonSerializerOptions);

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
        public static T? GetSection<T>(string? section = null) where T : class
        {
            if (!_init) throw new NotInitializedException();

            section ??= typeof(T).Name;
            return _mappedValues!.ContainsKey(section) ? _mappedValues.Single(x => x.Key == section).Value?.Deserialize<T>() ?? throw new InvalidOperationException($"Section {section} is missing") : default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NotInitializedException"
        public static bool SetSection<T>(T value, string? section = null) where T : class
        {
            if (!_init) throw new NotInitializedException();

            section ??= typeof(T).Name;
            _mappedValues!.Remove(section);
            _mappedValues.Add(section, JsonSerializer.SerializeToNode(value));
            using var writer = new StreamWriter(File.Create(_filepath!));
            writer.Write(JsonSerializer.Serialize(_mappedValues, options: _jsonSerializerOptions));
            return true;
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
