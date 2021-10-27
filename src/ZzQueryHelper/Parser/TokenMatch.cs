namespace ZzSoft.QueryHelper.Parser
{
    public class TokenMatch
    {
        internal bool IsMatch { get; set; }
        internal TokenType TokenType { get; set; }
        internal string Value { get; set; }
        internal string RemainingText { get; set; }
    }
}
