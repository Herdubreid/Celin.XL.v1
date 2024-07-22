using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celin.Language;

public static class Interpreter
{
    public static string ToCSharp(ExpressionType expression)
    {
        switch (expression.Operand)
        {
            case Operand.variable:
                return $"";
        }
        return string.Empty;
    }
}
