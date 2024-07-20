using Celin.AIS.Data;
using Celin.Language.XL;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.ParseError<char>;

namespace Celin.Language;
public enum Operand
{
    value,
    variable,
    xlrange,
};

public record ExpressionType(
    Operand Operand,
    string? Argument,
    AddressType? Address,
    object?[,]? Value);

public class Expression
{
    #region Parsers
    static readonly Parser<char, Operand> EQ =
        Base.Tok("=").ThenReturn(Operand.xlrange);
    static readonly Parser<char, ExpressionType> VARIABLE =
        Base.Tok("$")
            .Then(Literal.Plain
                .Select(s => new ExpressionType(Operand.variable, s, null, null)));
    static readonly Parser<char, ExpressionType> XLRANGE =
        Base.Tok("@")
            .Then(Address.Parser
                .Select(a => new ExpressionType(Operand.xlrange, null, a, null)));
    static readonly Parser<char, ExpressionType> VALUES =
        Values.Parser
            .Select(v => new ExpressionType(Operand.value, null, null, v));
    public static Parser<char, ExpressionType> ParseLH
        => OneOf(VARIABLE, XLRANGE);
    public static Parser<char, ExpressionType> ParseRH
        => OneOf(VARIABLE, XLRANGE, VALUES);
    #endregion
    public static Parser<char, (string, Maybe<string>)> Parser
        => Map((lh, rh) => (lh, rh),
            AnyCharExcept('=')
                .AtLeastOnceString()
                .Select(s => s.Trim()),
            EQ.Then(AnyCharExcept()
                .AtLeastOnceString()
                .Select(s => s.Trim()))
                .Optional());
}
