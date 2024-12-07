﻿using Pidgin;

namespace Celin.Language.XL;

public record WorkbookProperties(
    string? Name = null)
{
    public enum Methods { getActiveCell }
    public WorkbookProperties() : this(Name: null) { }
}
public class WorkbookObject<T> : BaseObject<T> where T : new()
{
    public void Method(WorkbookProperties.Methods method, params object?[] pars) =>
        _method = new(method, pars);
    public override object?[] Params => _method == null
        ? base.Params
        : new object?[] { _method.Value.Key.ToString() }.Concat(_method.Value.Value).ToArray();
    public override string? Key => null;
    public override T Properties { get => _xl; protected set => _xl = value; }
    public override T LocalProperties { get => _local; set => _local = value; }
    protected T _local = new T();
    protected T _xl = new T();
    protected KeyValuePair<WorkbookProperties.Methods, object?[]>? _method = null;
    #region Parser
    protected static Parser<char, string> XL = AIS.Data.Base.Tok("xl");
    #endregion
}
