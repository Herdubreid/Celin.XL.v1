using Celin.AIS.Data;
using Celin.AIS.Form;
using Celin.Language.XL;
using Pidgin;
using static Pidgin.Parser;

namespace Celin.Language;
public enum Operand
{
    value,
    variable,
    xlrange,
    datarequest,
    formrequest,
};

public record ExpressionType(
    Operand Operand,
    string? Argument = null,
    AddressType? Address = null,
    object?[,]? Value = null,
    AIS.Request? DataRequest = null,
    StackFormRequestChain? FormRequest = null);

public class Expression
{
    #region Parsers
    static readonly Parser<char, Operand> EQ =
        Base.Tok("=").ThenReturn(Operand.xlrange);
    static readonly Parser<char, ExpressionType> FORMREQUEST =
        StackFormRequestChain.Parser
            .Select(r => new ExpressionType(Operand.formrequest, FormRequest: r));
    static readonly Parser<char, ExpressionType> DATAREQUEST =
        OneOf(DataRequest.Parser, FormDataRequest.Parser)
            .Select(r => new ExpressionType(Operand.datarequest, DataRequest: r));
    static readonly Parser<char, ExpressionType> VARIABLE =
        Base.Tok("$")
            .Then(Literal.Plain
                .Select(s => new ExpressionType(Operand.variable, s)));
    static readonly Parser<char, ExpressionType> XLRANGE =
        Base.Tok("@")
            .Then(Address.Parser
                .Select(a => new ExpressionType(Operand.xlrange, null, a)));
    static readonly Parser<char, ExpressionType> VALUES =
        Values.Parser
            .Select(v => new ExpressionType(Operand.value, null, null, v));
    public static Parser<char, ExpressionType> ParseLH
        => OneOf(VARIABLE, XLRANGE);
    public static Parser<char, ExpressionType> ParseRH
        => OneOf(VARIABLE, XLRANGE, DATAREQUEST, FORMREQUEST, VALUES);
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
