using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Source.Scripts.ConfigSystem
{
    public abstract class ConfigsAbstraction
    {
        private string _configPath = "config.txt";
        private string[] _configLines;
        private string _hash;
        private string _resultPath => Application.dataPath + "/" + _configPath; 
        private bool _isInitialized;
        private Dictionary<string, ConfigAttribute> _defaultValues = new Dictionary<string, ConfigAttribute>();
        
        public void Initialize()
        {
            _isInitialized = false;
            LoadConfig();
            InitFields();
            _isInitialized = true;

        }
        
        protected abstract void InitFields();

        public void CheckHashSum()
        {
            var hash = GetConfigHash();

            if (hash != _hash)
            {
                LoadConfig();
                _hash = hash;
                InitFields();
            }
        }
        
        private string GetConfigHash()
        {
            var filePath = _resultPath;
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(filePath);
            return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
        }
        
        private void LoadConfig()
        {
            _configLines = File.ReadAllLines(_resultPath);
        }
        
        private void SaveNewLineToConfig(string value)
        {
            if (File.Exists(_resultPath))
            {
                _configLines = File.ReadAllLines(_resultPath);
                using var sw = File.AppendText(_resultPath);
                sw.WriteLine(value);
            }
            else
            {
                Debug.LogError($"Config file not found: {_resultPath}.");
            }

            _hash = GetConfigHash();
        }
        
        protected float GetValueOrSetDefault(string valueName, string description, float defaultValue)
        {
            if (!_isInitialized)
            {
                var configAttribute = new ConfigAttribute(defaultValue, description);
                _defaultValues[valueName] = configAttribute;
            }
            
            float value = 0;
            bool isFound = false;
            foreach (string line in _configLines)
            {
                var rawLine = line.Trim().Split('#');
                var rawData = rawLine[0].Trim().Split('=');
                if (!Validate(rawData)) continue;
                
                var data = new[]{ rawData[0].Trim(), rawData[1].Trim() };
            
                if (string.Equals(data[0], valueName, StringComparison.CurrentCultureIgnoreCase))
                {
                    value = ParseToFloat(data[1]);
                    isFound = true;
                    break;
                }
            }
            if (!isFound)
            {
                var resultLine = $"{valueName}={_defaultValues[valueName].DefaultFloatValue}{GetEmptySpaces(valueName)}  # {_defaultValues[valueName].DefaultDescription}";
                SaveNewLineToConfig(resultLine);
                value = _defaultValues[valueName].DefaultFloatValue;
            }
        
            return value;
        }
        
        protected string GetValueOrSetDefault(string valueName, string description, string defaultValue)
        {
            if (!_isInitialized)
            {
                var configAttribute = new ConfigAttribute(defaultValue, description);
                _defaultValues[valueName] = configAttribute;
            }
            
            string value = "";
            bool isFound = false;
            foreach (string line in _configLines)
            {
                var rawLine = line.Trim().Split('#');
                var rawData = rawLine[0].Trim().Split('=');
                if (!Validate(rawData)) continue;
                
                var data = new[]{ rawData[0].Trim(), rawData[1].Trim() };
            
                if (string.Equals(data[0], valueName, StringComparison.CurrentCultureIgnoreCase))
                {
                    value = ParseToString(data[1]);
                    isFound = true;
                    break;
                }
            }
            if (!isFound)
            {
                var resultLine = $"{valueName}={'"'}{_defaultValues[valueName].DefaultStringValue}{'"'}{GetEmptySpaces(valueName)}  # {_defaultValues[valueName].DefaultDescription}";
                SaveNewLineToConfig(resultLine);
                value = _defaultValues[valueName].DefaultStringValue;
            }
        
            return value;
        }

        private string ParseToString(string text)
        {
            var resultString = text.Replace("\"", "");
            return resultString;
        }
        
        private float ParseToFloat(string text)
        {
            var resultString = text.Replace(".", ",");
            return float.Parse(resultString);
        }
        
        private string GetEmptySpaces(string valueName)
        {
            var configAttribute = _defaultValues[valueName];
            var valueString = configAttribute.DefaultStringValue == null ? configAttribute.DefaultFloatValue.ToString(CultureInfo.InvariantCulture) : configAttribute.DefaultStringValue + "..";
            var holdingLetters = 40 - valueName.Length - valueString.Length;
            if (holdingLetters < 1) holdingLetters = 1;
            var result = new StringBuilder().Insert(0, " ", holdingLetters).ToString(); 
            return result;
        }
        
        private bool Validate(string[] rawData)
        {
            if (rawData.Length < 2) return false;
            if (string.IsNullOrEmpty(rawData[0])) return false;
            if (rawData[0][0] == '[') return false;
            return true;
        }
    }
}