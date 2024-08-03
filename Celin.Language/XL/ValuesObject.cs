﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celin.Language.XL;

public record ValuesProperties<T>(
    IEnumerable<IEnumerable<T>>? xl = null,
    List<List<T>>? local = null)
{
    public ValuesProperties() : this(null, null) { }
}
public class ValuesObject<T> : BaseObject<ValuesProperties<T>>
{
    public T this[int row, int col]
    {
        get => _xl.ElementAt(row)!.ElementAt(col)!;
        set => _local[row][col] = value;
    }
    public override string Key => _address ?? string.Empty;
    public override ValuesProperties<T> Properties
    {
        get => new ValuesProperties<T>(xl: _xl);
        protected set => _xl = value.xl!;
    }
    public override ValuesProperties<T> LocalProperties
    {
        get => new ValuesProperties<T>(local: _local);
        protected set => _local = value.local!;
    }
    string _address;
    IEnumerable<IEnumerable<T>> _xl;
    List<List<T>> _local;
    public ValuesObject(
        string? address,
        IEnumerable<IEnumerable<T>>? xl,
        List<List<T>>? local)
    {
        _address = address ?? throw new NullReferenceException("null Range address!");
        _xl = xl ?? Enumerable.Empty<IEnumerable<T>>();
        (int Left, int Top, int Right, int Bottom) sz = RangeObject.ToRef(address);
        _local = local ?? Enumerable.Range(0, sz.Bottom - sz.Top + 1)!
            .Select(_ => Enumerable.Range(0, sz.Right - sz.Left + 1)!
               .Select(_ => default(T)!)
               .ToList())
            .ToList();
    }
}
