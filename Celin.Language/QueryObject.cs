using Pidgin;
using System.Text.Json;
using static Pidgin.Parser<char>;
using static Pidgin.Parser;

namespace Celin.Language;
public class QueryObject
{
    public async Task<QueryObject> RunAsync()
    {
        var rs = await _e1.RequestAsync<JsonElement>(_request);

        FormResponse = JsonSerializer.Deserialize<AIS.FormResponse>(rs);
        Data = rs.EnumerateObject()
            .FirstOrDefault(e => e.Name.StartsWith("fs_") || e.Name.StartsWith("ds_"))
            .Value;

        return this;
    }
    public AIS.FormResponse? FormResponse { get; protected set; }
    public AIS.Form<AIS.FormData<AIS.DynamicJsonElement>>? DynamicForm =>
        JsonSerializer.Deserialize<AIS.Form<AIS.FormData<AIS.DynamicJsonElement>>>(Data, AIS.Server.JsonOutputOptions);
    public IEnumerable<AIS.DynamicJsonElement>? DynamicRows =>
        DynamicForm?.data.gridData.rowset;
    public AIS.Form<AIS.FormData<IEnumerable<object?>>>? GridForm =>
        JsonSerializer.Deserialize<AIS.Form<AIS.FormData<IEnumerable<object?>>>>(Data, AIS.Server.JsonOutputOptions);
    public IEnumerable<IEnumerable<object?>>? GridRows =>
        GridForm?.data.gridData.rowset;
    public JsonElement Data { get; protected set; }
    static AIS.Request Parse(string query) =>
        Try(AIS.Data.FormDataRequest.Parser)
            .Or(AIS.Data.DataRequest.Parser)
            .Before(AIS.Data.Skipper.Next(End))
        .ParseOrThrow(query);
    AIS.Server _e1;
    AIS.Request _request;
    JsonElement _result;
    QueryObject(AIS.Server e1, string query)
    {
        _request = Parse(query);
        _e1 = e1;
    }
    public static QueryObject Query(AIS.Server e1, string query)
        => new QueryObject(e1, query);

}
