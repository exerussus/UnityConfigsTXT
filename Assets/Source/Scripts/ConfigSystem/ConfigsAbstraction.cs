using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace Source.Scripts.ConfigSystem
{
    public abstract class ConfigsAbstraction : MonoBehaviour
    {
        public static ConfigsAbstraction Instance;

        private string _configPath = "config.txt";
        private string[] _configLines;
        private string _hash;

        private bool _isInitialized = false;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
            
            LoadConfig();
        }
        
        protected abstract void InitFields();
        protected abstract void UpdateFields();
        
        public void CheckHashSum()
        {
            var filePath = Application.dataPath + "/" + _configPath;
            var hash = "";

            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                }
            }

            if (hash != _hash)
            {
                LoadConfig();
                _hash = hash;
            }
        }
    
        private void LoadConfig()
        {
            var path = Application.dataPath + "/" + _configPath;

            if (File.Exists(path))
            {
                _configLines = File.ReadAllLines(path);
                if (_isInitialized) UpdateFields();
                else
                {
                    _isInitialized = true;
                    InitFields();
                }
            }
            else
            {
                Debug.LogError($"Config file not found: {Application.dataPath + "/" + _configPath}.");
            }
        }
        
        private void SaveNewLineToConfig(string value)
        {
            var path = Application.dataPath + "/" + _configPath;

            if (File.Exists(path))
            {
                _configLines = File.ReadAllLines(path);
                using (var sw = File.AppendText(path))
                {
                    sw.WriteLine(value);
                }
            }
            else
            {
                Debug.LogError($"Config file not found: {Application.dataPath + "/" + _configPath}.");
            }
        }
        
        public float GetValueOrSetDefault(string valueName, float defaultValue)
        {
            Debug.Log($"DefaultValue = {defaultValue}");
            float value = 0;
            bool isFound = false;
            foreach (string line in _configLines)
            {
                var rawData = line.Trim().Split('=');
                if (string.IsNullOrEmpty(rawData[0])) continue;
                if (rawData[0][0] == '[') continue;
                
                var data = new[]{ rawData[0].Trim(), rawData[1].Trim() };
            
                if (string.Equals(data[0], valueName, StringComparison.CurrentCultureIgnoreCase))
                {
                    var resultString = data[1].Replace(".", ",");
                    value = float.Parse(resultString);
                    isFound = true;
                    break;
                }
            }
            if (!isFound)
            {
                var resultLine = $"{valueName}={defaultValue}";
                SaveNewLineToConfig(resultLine);
            }
        
            return value;
        }
        
        public float GetValue(string valueName)
        {
            float value = 0;

            foreach (string line in _configLines)
            {
                var rawData = line.Trim().Split('=');
                if (string.IsNullOrEmpty(rawData[0])) continue;
                if (rawData[0][0] == '[') continue;
            
                var data = new[]{ rawData[0].Trim(), rawData[1].Trim() };
            
                if (string.Equals(data[0], valueName, StringComparison.CurrentCultureIgnoreCase))
                {
                    var resultString = data[1].Replace(".", ",");
                    value = float.Parse(resultString);
                    break;
                }
            }
        
            return value;
        }

        private void FixedUpdate()
        {
            CheckHashSum();
        }
    }
}