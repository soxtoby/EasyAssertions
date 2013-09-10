using System.Text.RegularExpressions;

namespace EasyAssertions
{
    /// <summary>
    /// A helper class for building string assertion failure messages in a consistent format when the ExpectedValue is a Regex.
    /// </summary>
    public class RegexFailureMessage : StringFailureMessage
    {
        public override object ExpectedValue
        {
            get
            {
                Regex regex = (Regex)RawExpectedValue;

                string value = "/" + RawExpectedValue + "/";

                if (regex.Options != RegexOptions.None)
                    value += " {" + regex.Options + "}";

                return value;
            }
            set
            {
                RawExpectedValue = value;
            }
        }
    }
}