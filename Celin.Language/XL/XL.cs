namespace Celin.Language.XL;

public delegate void SetRangeValue(AddressType address, object?[,] value);
public delegate object?[,] GetRangeValue(AddressType address);
public class XL
{
    public class RangeObject
    {
        public static SetRangeValue SetRangeValue { get; set; } = null!;
        public static GetRangeValue GetRangeValue { get; set; } = null!;
        public AddressType Address { get; }
        public object?[,] Values
        {
            get => GetRangeValue(Address);
            set => SetRangeValue(Address, value);
        }
        internal RangeObject(AddressType addres)
        {
            Address = addres;
        }
        internal RangeObject(string sheet, string range)
            : this(new AddressType(sheet, range)) { }
        internal RangeObject(string range)
            : this(new AddressType(null, range)) { }
    }
    public static RangeObject Range(AddressType address)
        => new RangeObject(address);
    public static RangeObject Range(string sheet, string range)
        => new RangeObject(sheet, range);
    public static RangeObject Range(string sheet)
        => new RangeObject(sheet);
}
