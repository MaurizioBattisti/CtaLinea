using System;
using System.Linq;
using System.Globalization;

namespace ZzSoft.QueryHelper.Parser
{
    internal class DslToken
    {
        internal DslToken(TokenType tokenType)
        {
            TokenType = tokenType;
            Value = string.Empty;
        }

        internal DslToken(TokenType tokenType, string value)
        {
            TokenType = tokenType;
            Value = value;
        }

        internal TokenType TokenType { get; set; }
        internal string Value { get; set; }

        internal object GetNumericValue (
            Type desiredType)
        {
            object value;
            if (this.TokenType != TokenType.Number)
            {
                throw new InvalidOperationException("Cannot get a numeric value of a token that isn't a Number");
            }

            if (desiredType == typeof(int))
            {
                value = int.Parse(this.Value, CultureInfo.InvariantCulture);
            }
            else if (desiredType == typeof(short))
            {
                value = short.Parse(this.Value, CultureInfo.InvariantCulture);
            }
            else if (desiredType == typeof(decimal))
            {
                value = decimal.Parse(this.Value, CultureInfo.InvariantCulture);
            }
            else if (desiredType == typeof(float))
            {
                value = float.Parse(this.Value, CultureInfo.InvariantCulture);
            }
            else if (desiredType == typeof(double))
            {
                value = double.Parse(this.Value, CultureInfo.InvariantCulture);
            }
            else if (desiredType == typeof(bool))
            {
                var tmp = int.Parse(this.Value, CultureInfo.InvariantCulture);
                value = tmp != 0;
            }
            else
            {
                value = null;
            }
            return value;
        }

        internal decimal GetDecimalNumericValue ()
        {
            if (this.TokenType != TokenType.Number)
            {
                throw new InvalidOperationException("Cannot get a numeric value of a token that isn't a Number");
            }

            return decimal.Parse(this.Value, CultureInfo.InvariantCulture);
        }
        internal DateTime GetDatetTimeValue ()
        {
            if (this.TokenType != TokenType.DateTimeValue)
            {
                throw new InvalidOperationException("Cannot get a DateTime value of a token that isn't a DateTime value");
            }
            DateTime dt;
            if (this.Value.Length > 10)
            {
                dt = DateTime.ParseExact(this.Value, "s", CultureInfo.InvariantCulture);
            }
            else
            {
                dt = DateTime.ParseExact(this.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            }
            return dt;
        }
        internal TimeSpan GetTimeValue()
        {
            if (this.TokenType != TokenType.TimeValue)
            {
                throw new InvalidOperationException("Cannot get a Time value of a token that isn't a DateTime value");
            }
            DateTime dt;
            if (this.Value.Length > 5)
            {
                dt = DateTime.ParseExact(this.Value, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None);
            }
            else
            {
                dt = DateTime.ParseExact(this.Value, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None);
            }
            return new TimeSpan(dt.Hour, dt.Minute, dt.Second);
        }
        internal string GetStringValue ()
        {
            if (this.TokenType != TokenType.StringValue)
            {
                throw new InvalidOperationException("Cannot get a string value of a token that isn't a  strign value");
            }
            string result = string.Empty;
            if (string.IsNullOrEmpty (this.Value) == false)
            {
                // result = this.Value.Substring (1, this.Value.Length - 2);
                result = string.Concat(this.Value.Skip(1).Take(this.Value.Length - 2));
            }
            return result;
        }
        internal DslToken Clone()
        {
            return new DslToken(TokenType, Value);
        }
    }
}
