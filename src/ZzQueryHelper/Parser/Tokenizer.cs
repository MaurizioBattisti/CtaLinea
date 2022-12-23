using System;
using System.Collections.Generic;
using System.Linq;

namespace ZzSoft.QueryHelper.Parser
{
    internal class Tokenizer<T>
    {
        private readonly IList<TokenDefinition> _tokenDefinitions;

        internal Tokenizer()
        {
            Type t = typeof(T);
            var props = t.GetProperties();

            this._tokenDefinitions = new List<TokenDefinition>()
            {
                new TokenDefinition(TokenType.BlankSpace, @"^ +"),
                new TokenDefinition(TokenType.And, @"^\$and\("),
                new TokenDefinition(TokenType.Or, @"^\$or\("),
                // new TokenDefinition(TokenType.Between, @"^\$between"),
                new TokenDefinition(TokenType.CloseParenthesis, @"^\)"),
                new TokenDefinition(TokenType.Comma, @"^,"),

                new TokenDefinition(TokenType.Equals, @"^\$eq\("),
                new TokenDefinition(TokenType.NotEquals, @"^\$ne\("),
                new TokenDefinition(TokenType.LessThan, @"^\$lt\("),
                new TokenDefinition(TokenType.LessOrEqual, @"^\$le\("),
                new TokenDefinition(TokenType.GreaterThan, @"^\$gt\("),
                new TokenDefinition(TokenType.GreaterOrEqual, @"^\$ge\("),
                new TokenDefinition(TokenType.In, @"^\$in\("),
                new TokenDefinition(TokenType.NotIn, @"^\$nin\("),

                new TokenDefinition(TokenType.StartWith, @"^\$start\("),
                new TokenDefinition(TokenType.NntStartWith, @"^\$nstart\("),
                new TokenDefinition(TokenType.Contains, @"^\$like\("),
                new TokenDefinition(TokenType.NotContains, @"^\$notlike\("),

                new TokenDefinition(TokenType.DateTimeValue, @"^\d\d\d\d-\d\d-\d\dT\d\d:\d\d:\d\d|^\d\d\d\d-\d\d-\d\d"),
                new TokenDefinition(TokenType.TimeValue, @"^\d\d:\d\d:\d\d|^\d\d:\d\d"),
                new TokenDefinition(TokenType.StringValue, @"^'[^']*'"),
                new TokenDefinition(TokenType.Number, @"^\d+"),
                new TokenDefinition(TokenType.TrueValue, @"^true"),
                new TokenDefinition(TokenType.FalseValue, @"^false")
            };

            if (props != null
                && props.Length > 0)
            {
                string attr = string.Empty;
                foreach (var p in props)
                {
                    if (string.IsNullOrEmpty (attr) == false)
                    {
                        attr += "|";
                    }
                    attr += "^" + p.Name;
                }
                this._tokenDefinitions.Add(new TokenDefinition(TokenType.AttributeName, attr));
            }
        }
        internal IEnumerable<DslToken> Tokenize(string lqlText)
        {
            string remainingText = lqlText;

            while (!string.IsNullOrWhiteSpace(remainingText))
            {
                var match = FindMatch(remainingText);
                if (match.IsMatch)
                {
                    yield return new DslToken(match.TokenType, match.Value);
                    remainingText = string.Concat(remainingText.Skip(match.Value.Length));
                    // remainingText = remainingText.Substring (match.Value.Length);
                }
                else
                {
                    remainingText = string.Concat(remainingText.Skip(1));
                    // remainingText = remainingText.Substring(1);
                }
            }

            yield return new DslToken(TokenType.SequenceTerminator, string.Empty);
        }

        private TokenMatch FindMatch(string lqlText)
        {
            foreach (var tokenDefinition in this._tokenDefinitions)
            {
                var match = tokenDefinition.Match(lqlText);
                if (match.IsMatch)
                    return match;
            }

            return new TokenMatch() { IsMatch = false };
        }
    }
}
