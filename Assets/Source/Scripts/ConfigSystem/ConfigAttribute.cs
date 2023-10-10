namespace Source.Scripts.ConfigSystem
{
    public class ConfigAttribute
    {
        public ConfigAttribute(float floatValue, string description)
        {
            DefaultFloatValue = floatValue;
            DefaultDescription = description;
        }
        public ConfigAttribute(string stringValue, string description)
        {
            DefaultStringValue = stringValue;
            DefaultDescription = description;
        }
        
        public float DefaultFloatValue { get; }
        public string DefaultStringValue { get; }

        public string DefaultDescription { get; }
    }
}