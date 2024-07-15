using Celin.AIS.Data;
using Celin.Language.XL;
using Pidgin;
using static Pidgin.Parser<char>;
using static Pidgin.Parser;

namespace Celin.Language;
public enum Cmds
{
    value,
    ls,
    help,
    variable,
    xlrange,
};

public record CmdType(
    Cmds Cmd,
    string? Argument,
    AddresType? Address,
    IEnumerable<IEnumerable<object>>? Value);

public class PromptCommand
{
    #region Parameters
    public CmdType? LeftHand { get; set; }
    public CmdType? RightHand { get; set; }
    #endregion
    #region Parsers
    static readonly Parser<char, Cmds> LS
        = Base.Tok("ls").ThenReturn(Cmds.ls);
    static readonly Parser<char, Cmds> HELP
        = Base.Tok("help").ThenReturn(Cmds.help);
    static readonly Parser<char, Cmds> VARIABLE
        = Base.Tok("$").ThenReturn(Cmds.variable);
    static readonly Parser<char, Cmds> XLRANGE
        = Base.Tok("@").ThenReturn(Cmds.xlrange);
    static readonly Parser<char, Cmds> EQ
        = Base.Tok("=").ThenReturn(Cmds.xlrange);
    #endregion
    public static Parser<char, CmdType> LH
        => OneOf(LS, HELP, VARIABLE, XLRANGE)
        .Bind(cmd =>
        {
            switch (cmd)
            {
                case Cmds.ls:
                case Cmds.help:
                    return End.ThenReturn(new CmdType(cmd, null, null, null));
                case Cmds.xlrange:
                    return Address.Parser
                        .Select(a => new CmdType(cmd, null, a, null));
                default:
                    return Literal.Plain
                        .Select(l => new CmdType(cmd, l, null, null));
            }
        });
    public static Parser<char, CmdType> RH
        => SkipWhitespaces
        .Then(EQ)
        .Then(Try(OneOf(VARIABLE, XLRANGE)
            .Bind(cmd =>
            {
                switch (cmd)
                {
                    case Cmds.xlrange:
                        return Address.Parser
                            .Select(a => new CmdType(cmd, null, a, null));
                    default:
                        return Literal.Plain
                            .Select(l => new CmdType(cmd, l, null, null));
                }
            }))
            .Or(Values.Parser
                .Select(v => new CmdType(Cmds.value, null, null, v))));
    public static Parser<char, PromptCommand> Parser
        => Map((lh, rh) => new PromptCommand
        {
            LeftHand = lh,
            RightHand = rh.HasValue ? rh.Value : null,
        },
        LH, RH.Optional());
}
