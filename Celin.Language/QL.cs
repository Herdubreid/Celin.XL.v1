using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Celin.Language;
public class QL
{
    public static Parser<char, AIS.Request> Parser
        => Try(AIS.Data.FormDataRequest.Parser)
            .Or(AIS.Data.DataRequest.Parser);
    public static AIS.Request Parse(string query)
        => Parser
            .Before(AIS.Data.Skipper.Next(End))
            .ParseOrThrow(query);
}
