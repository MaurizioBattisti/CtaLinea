using System.Linq;
using System.Text.RegularExpressions;

namespace ZzSoft.QueryHelper.Parser
{
    internal class TokenDefinition
    {
        private readonly Regex _regex;
        private readonly TokenType _returnsToken;

        internal TokenDefinition(TokenType returnsToken, string regexPattern)
        {
            this._regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
            this._returnsToken = returnsToken;
        }

        internal TokenMatch Match(string inputString)
        {
            var match = this._regex.Match(inputString);
            if (match.Success)
            {
                string remainingText = string.Empty;
                if (match.Length != inputString.Length)
                {
                    remainingText = string.Concat(inputString.Take(match.Length));
                }

                return new TokenMatch()
                {
                    IsMatch = true,
                    RemainingText = remainingText,
                    TokenType = _returnsToken,
                    Value = match.Value
                };
            }
            else
            {
                return new TokenMatch() { IsMatch = false };
            }
        }
    }
}
