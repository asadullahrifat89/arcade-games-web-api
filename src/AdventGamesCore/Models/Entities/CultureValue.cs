namespace AdventGamesCore
{
    public class CultureValue
    {
        public CultureValue(string culture, string value)
        {
            Culture = culture;
            Value = value;
        }

        public string Culture { get; set; }

        public string Value { get; set; }
    }
}
