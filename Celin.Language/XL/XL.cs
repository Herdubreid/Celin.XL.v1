using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celin.Language.XL;

public class XL
{
    public class RangeObject
    {
        public AddressType Address { get; }
        static IEnumerable<IEnumerable<object?>> _values = Enumerable.Empty<IEnumerable<object?>>();
        public IEnumerable<IEnumerable<object?>> Values
        {
            get => _values;
            set => _values = value;
        }
        internal RangeObject(AddressType addres)
        {
            Address = addres;
        }
    }
    public static RangeObject Range(AddressType address)
        => new RangeObject(address);
}
