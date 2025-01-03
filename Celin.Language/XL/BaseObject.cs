﻿using Pidgin;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Celin.Language.XL;

[JsonDerivedType(typeof(WorkbookProperties), nameof(WorkbookProperties))]
[JsonDerivedType(typeof(WorksheetProperties), nameof(WorksheetProperties))]
[JsonDerivedType(typeof(RangeProperties), nameof(RangeProperties))]
[JsonDerivedType(typeof(FormatProperties), nameof(FormatProperties))]
[JsonDerivedType(typeof(FillProperties), nameof(FillProperties))]
[JsonDerivedType(typeof(FontProperties), nameof(FontProperties))]
[JsonDerivedType(typeof(TableProperties), nameof(TableProperties))]
[JsonDerivedType(typeof(TableHeaderProperties), nameof(TableHeaderProperties))]
[JsonDerivedType(typeof(TableBodyProperties), nameof(TableBodyProperties))]
public record BaseProperties(
    string? Id = null)
{
    public BaseProperties() : this(Id: null) { }
}
public delegate ValueTask<T> SyncAsyncDelegate<T>(string? key, T values, params object?[] pars);
public abstract class BaseObject { }
public abstract class BaseObject<T> : BaseObject
    where T : new()
{
    public abstract string? Key { get; }
    public virtual object?[] Params => [];
    public abstract T Properties { get; protected set; }
    public abstract T LocalProperties { get; set; }
    public static SyncAsyncDelegate<T> SyncAsyncDelegate { get; set; } = null!;
    public async Task SyncAsync()
    {
        Properties = await SyncAsyncDelegate(Key, LocalProperties, Params);
        LocalProperties = new();
    }
    public void Set(IEnumerable<Action<T>> actions)
    {
        var props = new T();
        foreach (var action in actions) action(props);
        LocalProperties = props;
    }
}
public static class ParserExtensions
{
    public static Parser<char, T> InBraces<T>(this Parser<char, T> parser) =>
        Char('(').Then(parser).Before(Char(')'));
    public static Parser<char, T> InBracket<T>(this Parser<char, T> parser) =>
        Char('[').Then(parser).Before(Char(']'));
    public static Parser<char, T> Trim<T>(this Parser<char, T> parser) =>
        parser.Before(SkipWhitespaces);
    public static Parser<char, T> DotPrefix<T>(this Parser<char, T> parser) =>
        Try(SkipWhitespaces.Then(Char('.')).Then(parser));
    public static Parser<char, T> Actions<T>(
        this Parser<char, T> parser,
        Parser<char, IEnumerable<Action<T>>> actions)
        where T : BaseObject =>
        Map((obj, actions) =>
        {
            if (actions.HasValue)
                foreach (var action in actions.Value)
                    action(obj);
            return obj;
        },
        parser,
        actions.DotPrefix().Optional());
}
public partial class BaseParser
{
    [GeneratedRegex(@"^('[^']+'!|\w+!)?[A-Za-z]{1,3}\d{0,7}(:[A-Za-z]{1,3}\d{0,7})?$")]
    protected static partial Regex AddressPattern();
    protected static Parser<char, T> Tok<T>(Parser<char, T> p) =>
        Try(SkipWhitespaces.Then(p)).Before(SkipWhitespaces);
    protected static Parser<char, char> Tok(char value) => Tok(CIChar(value));
    protected static Parser<char, string> Tok(string value) => Tok(CIString(value));
    protected static Parser<char, string> ALIAS =>
        Map((l, r) => l.ToUpper() + (r.HasValue ? $".{r.Value.ToUpper()}" : string.Empty),
        LetterOrDigit.AtLeastOnceString(),
        Char('.').Then(LetterOrDigit.AtLeastOnceString()).Optional());
    protected static Parser<char, char> DOT_SEPARATOR =>
        SkipWhitespaces.Then(Char('.'));
    protected static Parser<char, char> COMMA_SEPARATOR =>
        SkipWhitespaces.Then(Char(','));
    protected static Parser<char, List<object?>> OBJECT_ARRAY_PARAMETER =>
        Values<object?>.ARRAY.InBraces();
    protected static Parser<char, List<string?>> STRING_ARRAY_PARAMETER =>
        Values<string?>.ARRAY.InBraces();
    protected static Parser<char, List<List<object?>>> OBJECT_MATRIX_PARAMETER =>
        Values<object?>.MATRIX.InBraces();
    protected static Parser<char, List<List<string?>>> STRING_MATRIX_PARAMETER =>
        Values<string?>.MATRIX.InBraces();
    protected static Parser<char, string> ADDRESS =>
        AnyCharExcept(')').ManyString().Where(s =>
        {
            if (string.IsNullOrEmpty(s)) return true;
            var m = AddressPattern().Match(s);
            return m.Success;
        });
    protected static Parser<char, string> ADDRESS_PARAMETER =>
        ADDRESS.InBraces();
    protected static Parser<char, string> STRING_PARAMETER =>
        Values<string>.STRING.InBraces();
    protected static Parser<char, int> INT_PARAMETER =>
        Values<int>.NUMBER.InBraces();
    protected static Parser<char, decimal> DECIMAL_PARAMETER =>
        Values<decimal>.NUMBER.InBraces();
    protected static Parser<char, bool> BOOL_PARAMETER =>
        Values<string>.BOOL.InBraces();
}
