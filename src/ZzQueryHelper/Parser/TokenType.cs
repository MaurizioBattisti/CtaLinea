namespace ZzSoft.QueryHelper.Parser
{
    internal enum TokenType
    {
        Undefined,

        AttributeName,
        CloseParenthesis,
        BlankSpace,
        Comma,

        // Operatori Logici
        And,
        Or,

        // Operatori di confronto
        Equals,
        NotEquals,
        GreaterThan,
        GreaterOrEqual,
        LessThan,
        LessOrEqual,
        In,
        NotIn,
        StartWith,
        Contains,
        NntStartWith,
        NotContains,

        // costanti
        StringValue,
        DateTimeValue,
        TimeValue,
        Number,
        TrueValue,
        FalseValue,

        // terminatore
        SequenceTerminator
    }
}
