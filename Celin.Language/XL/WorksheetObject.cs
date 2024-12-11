using Celin.AIS.Data;
using Pidgin;
using static Pidgin.Parser;

namespace Celin.Language.XL;

public record WorksheetProperties(string? Id = null) : BaseProperties(Id)
{
    public string? Name { get; set; }
    public int? Position { get; set; }
    public bool? EnableCalculation { get; set; }
    public bool? ShowGridlines { get; set; }
    public bool? ShowHeadings { get; set; }
    public decimal? StandardHeight { get; set; }
    public decimal? StandardWidth { get; set; }
    public string? TabColor { get; set; }
    public int? TabId { get; set; }
    public string? Visibility { get; set; }
    public WorksheetProperties() : this(Id: null) { }
};
public class WorksheetObject : BaseObject<WorksheetProperties>
{
    public RangeObject Range(string? address = null)
    {
        string? name = _xl.Name ?? _local.Name;
        return RangeObject.Range($"{(string.IsNullOrEmpty(name) ? "" : $"'{name}'!")}{address}");
    }
    public override string Key => _xl.Id ?? _local.Name ?? string.Empty;
    public override WorksheetProperties Properties
    {
        get => _xl;
        protected set => _xl = value;
    }
    public override WorksheetProperties LocalProperties
    {
        get => _local;
        set => _local = value;
    }
    WorksheetProperties _local;
    WorksheetProperties _xl;
    public WorksheetObject(string? sheetName)
    {
        _xl = new WorksheetProperties();
        _local = new WorksheetProperties { Name = sheetName };
    }
    public static WorksheetObject Sheet(string? name = null)
        => new WorksheetObject(name);
}
public class WorksheetParser : BaseParser
{
    static Parser<char, Action<WorksheetObject>> Name =>
        Base.Tok(nameof(Name)).Then(STRING_PARAMETER)
        .Select<Action<WorksheetObject>>(s => sheet => sheet.LocalProperties.Name = s);
    static Parser<char, Action<WorksheetObject>> Position =>
        Base.Tok(nameof(Position)).Then(INT_PARAMETER)
        .Select<Action<WorksheetObject>>(n => sheet => sheet.LocalProperties.Position = n);
    static Parser<char, Action<WorksheetObject>> EnableCalculation =>
        Base.Tok(nameof(EnableCalculation)).Then(BOOL_PARAMETER)
        .Select<Action<WorksheetObject>>(b => sheet => sheet.LocalProperties.EnableCalculation = b);
    static Parser<char, Action<WorksheetObject>> ShowGridlines =>
        Base.Tok(nameof(ShowGridlines)).Then(BOOL_PARAMETER)
        .Select<Action<WorksheetObject>>(b => sheet => sheet.LocalProperties.ShowGridlines = b);
    static Parser<char, Action<WorksheetObject>> ShowHeadings =>
        Base.Tok(nameof(ShowHeadings)).Then(BOOL_PARAMETER)
        .Select<Action<WorksheetObject>>(b => sheet => sheet.LocalProperties.ShowHeadings = b);
    static Parser<char, Action<WorksheetObject>> StandardWidth =>
        Base.Tok(nameof(StandardWidth)).Then(DECIMAL_PARAMETER)
        .Select<Action<WorksheetObject>>(d => sheet => sheet.LocalProperties.StandardWidth = d);
    static Parser<char, Action<WorksheetObject>> TabColor =>
        Base.Tok(nameof(TabColor)).Then(STRING_PARAMETER)
        .Select<Action<WorksheetObject>>(s => sheet => sheet.LocalProperties.TabColor = s);
    static Parser<char, Action<WorksheetObject>> Visibility =>
        Base.Tok(nameof(Visibility)).Then(
            OneOf(Base.Tok("Visible"), Base.Tok("Hidden"), Base.Tok("VeryHidden"))
            .Between(Char('('), Char(')')))
            .Select<Action<WorksheetObject>>(s => sheet => sheet.LocalProperties.Visibility = s);
    static Parser<char, WorksheetObject> WORKSHEET =>
        Base.Tok("sheet").Then(STRING_PARAMETER.Optional())
        .Select(w => new WorksheetObject(w.HasValue ? w.Value : null));
    public static Parser<char, BaseObject> Parser =>
        Map((sheet, actions) =>
        {
            foreach (var action in actions) action(sheet);
            return sheet as BaseObject;
        },
        Skipper.Next(WORKSHEET).Before(Char('.')),
            OneOf(
                Name,
                Position,
                EnableCalculation,
                ShowGridlines,
                ShowHeadings,
                StandardWidth,
                TabColor,
                Visibility)
            .Separated(SkipWhitespaces.Then(Char('.'))));
}