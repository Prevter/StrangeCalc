using StrangeCalc.Exceptions;
using System.Globalization;

namespace StrangeCalc;

public static class BuiltInFunctions
{
    private static readonly CodePosition InternalCodePosition = new(-1, -1, -1, "<module>", "");

    #region IO Functions

    public static RuntimeResult Print(IEnumerable<object?> args)
    {
        foreach (var arg in args)
        {
            if (arg is IEnumerable<object> array)
            {
                Console.Write("[" + string.Join(", ", array.Select(x => x is null ? "null" : x.ToString())) + "]");
            }
            else if (arg is null)
            {
                Console.Write("null");
            }
            else
            {
                Console.Write(arg.ToString());
            }
        }
        return RuntimeResult.Success(null);
    }

    public static RuntimeResult Printf(IEnumerable<object?> args)
    {
        if (!args.Any())
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "printf() takes at least 1 argument", Context.Root));
        }

        if (args.ElementAt(0) is string format)
        {
            Console.Write(string.Format(format, args.Skip(1).ToArray()));
            return RuntimeResult.Success(null);
        }

        return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "format string was not a string", Context.Root));
    }

    public static RuntimeResult SPrintf(IEnumerable<object?> args)
    {
        if (!args.Any())
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "sprintf() takes at least 1 argument", Context.Root));
        }

        if (args.ElementAt(0) is string format)
        {
            return RuntimeResult.Success(string.Format(format, args.Skip(1).ToArray()));
        }

        return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "format string was not a string", Context.Root));
    }

    public static RuntimeResult Scanf(IEnumerable<object?> args)
    {
        if (args.Count() != 1)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "scanf() takes exactly 1 argument", Context.Root));
        }

        if (args.ElementAt(0) is string format)
        {
            var input = Console.ReadLine();
            if (input is null)
            {
                return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "scanf() failed to read input", Context.Root));
            }

            switch (format)
            {
                case "%d":
                    if (int.TryParse(input, out int i))
                    {
                        return RuntimeResult.Success((double)i);
                    }
                    else
                    {
                        return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "scanf() failed to parse input", Context.Root));
                    }
                case "%f":
                    if (double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out double d))
                    {
                        return RuntimeResult.Success(d);
                    }
                    else
                    {
                        return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "scanf() failed to parse input", Context.Root));
                    }
                case "%s":
                    return RuntimeResult.Success(input);
                default:
                    return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "scanf() format was not a valid format", Context.Root));
            }
        }

        return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "format string was not a string", Context.Root));
    }

    public static RuntimeResult Clear(IEnumerable<object?> _)
    {
        Console.Clear();
        return RuntimeResult.Success(null);
    }

    public static RuntimeResult Sleep(IEnumerable<object?> args)
    {
        if (args.Count() != 1)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "sleep() takes exactly 1 argument", Context.Root));
        }

        if (args.ElementAt(0) is double value)
        {
            Thread.Sleep((int)value);
            return RuntimeResult.Success(null);
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "sleep() takes only numbers", Context.Root));
        }
    }

    #endregion

    #region Trigonometric functions

    public static RuntimeResult Sin(IEnumerable<object?> args)
    {
        if (args.Count() != 1)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "sin() takes exactly 1 argument", Context.Root));
        }

        if (args.ElementAt(0) is double value)
        {
            return RuntimeResult.Success(Math.Sin(value));
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "sin() takes only number", Context.Root));
        }
    }

    public static RuntimeResult Cos(IEnumerable<object?> args)
    {
        if (args.Count() != 1)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "cos() takes exactly 1 argument", Context.Root));
        }

        if (args.ElementAt(0) is double value)
        {
            return RuntimeResult.Success(Math.Cos(value));
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "cos() takes only number", Context.Root));
        }
    }

    public static RuntimeResult Tan(IEnumerable<object?> args)
    {
        if (args.Count() != 1)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "tan() takes exactly 1 argument", Context.Root));
        }

        if (args.ElementAt(0) is double value)
        {
            return RuntimeResult.Success(Math.Tan(value));
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "tan() takes only number", Context.Root));
        }
    }

    public static RuntimeResult Asin(IEnumerable<object?> args)
    {
        if (args.Count() != 1)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "asin() takes exactly 1 argument", Context.Root));
        }

        if (args.ElementAt(0) is double value)
        {
            return RuntimeResult.Success(Math.Asin(value));
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "asin() takes only number", Context.Root));
        }
    }

    public static RuntimeResult Acos(IEnumerable<object?> args)
    {
        if (args.Count() != 1)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "acos() takes exactly 1 argument", Context.Root));
        }

        if (args.ElementAt(0) is double value)
        {
            return RuntimeResult.Success(Math.Acos(value));
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "acos() takes only number", Context.Root));
        }
    }

    public static RuntimeResult Atan(IEnumerable<object?> args)
    {
        if (args.Count() != 1)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "atan() takes exactly 1 argument", Context.Root));
        }

        if (args.ElementAt(0) is double value)
        {
            return RuntimeResult.Success(Math.Atan(value));
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "atan() takes only number", Context.Root));
        }
    }

    #endregion

    #region Hyperbolic functions

    public static RuntimeResult Sinh(IEnumerable<object?> args)
    {
        if (args.Count() != 1)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "sinh() takes exactly 1 argument", Context.Root));
        }

        if (args.ElementAt(0) is double value)
        {
            return RuntimeResult.Success(Math.Sinh(value));
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "sinh() takes only number", Context.Root));
        }
    }

    public static RuntimeResult Cosh(IEnumerable<object?> args)
    {
        if (args.Count() != 1)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "cosh() takes exactly 1 argument", Context.Root));
        }

        if (args.ElementAt(0) is double value)
        {
            return RuntimeResult.Success(Math.Cosh(value));
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "cosh() takes only number", Context.Root));
        }
    }

    public static RuntimeResult Tanh(IEnumerable<object?> args)
    {
        if (args.Count() != 1)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "tanh() takes exactly 1 argument", Context.Root));
        }

        if (args.ElementAt(0) is double value)
        {
            return RuntimeResult.Success(Math.Tanh(value));
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "tanh() takes only number", Context.Root));
        }
    }

    #endregion

    #region Math functions

    public static RuntimeResult Sqrt(IEnumerable<object?> args)
    {
        if (args.Count() == 1)
        {
            // Sqrt2
            if (args.ElementAt(0) is double value)
            {
                return RuntimeResult.Success(Math.Sqrt(value));
            }
            else
            {
                return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "sqrt() takes only numbers", Context.Root));
            }
        }
        else if (args.Count() == 2)
        {
            // SqrtN
            if (args.ElementAt(0) is double value && args.ElementAt(1) is double root)
            {
                return RuntimeResult.Success(Math.Pow(value, 1.0 / root));
            }
            else
            {
                return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "sqrt() takes only numbers", Context.Root));
            }
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "sqrt() takes 1 or 2 arguments", Context.Root));
        }
    }

    public static RuntimeResult Pow(IEnumerable<object?> args)
    {
        if (args.Count() != 2)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "pow() takes exactly 2 arguments", Context.Root));
        }

        if (args.ElementAt(0) is double left && args.ElementAt(1) is double right)
        {
            return RuntimeResult.Success(Math.Pow(left, right));
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "pow() takes only numbers", Context.Root));
        }
    }

    public static RuntimeResult Abs(IEnumerable<object?> args)
    {
        if (args.Count() != 1)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "abs() takes exactly 1 argument", Context.Root));
        }

        if (args.ElementAt(0) is double value)
        {
            return RuntimeResult.Success(Math.Abs(value));
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "abs() takes only numbers", Context.Root));
        }
    }

    public static RuntimeResult Floor(IEnumerable<object?> args)
    {
        if (args.Count() != 1)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "floor() takes exactly 1 argument", Context.Root));
        }

        if (args.ElementAt(0) is double value)
        {
            return RuntimeResult.Success(Math.Floor(value));
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "floor() takes only numbers", Context.Root));
        }
    }

    public static RuntimeResult Ceil(IEnumerable<object?> args)
    {
        if (args.Count() != 1)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "ceil() takes exactly 1 argument", Context.Root));
        }

        if (args.ElementAt(0) is double value)
        {
            return RuntimeResult.Success(Math.Ceiling(value));
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "ceil() takes only numbers", Context.Root));
        }
    }

    public static RuntimeResult Round(IEnumerable<object?> args)
    {
        if (args.Count() != 1)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "round() takes exactly 1 argument", Context.Root));
        }

        if (args.ElementAt(0) is double value)
        {
            return RuntimeResult.Success(Math.Round(value));
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "round() takes only numbers", Context.Root));
        }
    }

    public static RuntimeResult Min(IEnumerable<object?> args)
    {
        if (args.Count() < 2)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "min() takes at least 2 arguments", Context.Root));
        }

        if (args.All(x => x is double))
        {
            return RuntimeResult.Success(args.Min(x => (double)x));
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "min() takes only numbers", Context.Root));
        }
    }

    public static RuntimeResult Max(IEnumerable<object?> args)
    {
        if (args.Count() < 2)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "max() takes at least 2 arguments", Context.Root));
        }

        if (args.All(x => x is double))
        {
            return RuntimeResult.Success(args.Max(x => (double)x));
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "max() takes only numbers", Context.Root));
        }
    }

    public static RuntimeResult Clamp(IEnumerable<object?> args)
    {
        if (args.Count() != 3)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "clamp() takes exactly 3 arguments", Context.Root));
        }

        if (args.All(x => x is double))
        {
            var (value, min, max) = (args.ElementAt(0), args.ElementAt(1), args.ElementAt(2));

            if ((double)value < (double)min)
            {
                return RuntimeResult.Success(min);
            }
            else if ((double)value > (double)max)
            {
                return RuntimeResult.Success(max);
            }
            else
            {
                return RuntimeResult.Success(value);
            }
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "clamp() takes only numbers", Context.Root));
        }
    }

    public static RuntimeResult Random(IEnumerable<object?> args)
    {
        if (args.Count() != 2)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "random() takes exactly 2 arguments", Context.Root));
        }

        if (args.All(x => x is double))
        {
            var (min, max) = ((double)args.ElementAt(0), (double)args.ElementAt(1));

            return RuntimeResult.Success(System.Random.Shared.NextDouble() * (max - min) + min);
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "random() takes only numbers", Context.Root));
        }
    }

    public static RuntimeResult Log(IEnumerable<object?> args)
    {
        if (args.Count() != 2)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "log() takes exactly 2 arguments", Context.Root));
        }

        if (args.All(x => x is double))
        {
            var (value, @base) = ((double)args.ElementAt(0), (double)args.ElementAt(1));

            return RuntimeResult.Success(Math.Log(value, @base));
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "log() takes only numbers", Context.Root));
        }
    }

    public static RuntimeResult Log10(IEnumerable<object?> args)
    {
        if (args.Count() != 1)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "log10() takes exactly 1 argument", Context.Root));
        }

        if (args.ElementAt(0) is double value)
        {
            return RuntimeResult.Success(Math.Log10(value));
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "log10() takes only numbers", Context.Root));
        }
    }

    public static RuntimeResult Log2(IEnumerable<object?> args)
    {
        if (args.Count() != 1)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "log2() takes exactly 1 argument", Context.Root));
        }

        if (args.ElementAt(0) is double value)
        {
            return RuntimeResult.Success(Math.Log2(value));
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "log2() takes only numbers", Context.Root));
        }
    }

    public static RuntimeResult Ln(IEnumerable<object?> args)
    {
        if (args.Count() != 1)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "ln() takes exactly 1 argument", Context.Root));
        }

        if (args.ElementAt(0) is double value)
        {
            return RuntimeResult.Success(Math.Log(value));
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "ln() takes only numbers", Context.Root));
        }
    }

    #endregion

    #region String functions

    public static RuntimeResult Length(IEnumerable<object?> args)
    {
        if (args.Count() != 1)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "len() takes exactly 1 argument", Context.Root));
        }

        if (args.ElementAt(0) is string value)
        {
            return RuntimeResult.Success(value.Length);
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "len() takes only strings", Context.Root));
        }
    }

    public static RuntimeResult Substring(IEnumerable<object?> args)
    {
        if (args.Count() != 3)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "substr() takes exactly 3 arguments", Context.Root));
        }

        if (args.ElementAt(0) is string value && args.ElementAt(1) is double start && args.ElementAt(2) is double length)
        {
            return RuntimeResult.Success(value.Substring((int)start, (int)length));
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "substr() takes only strings and numbers", Context.Root));
        }
    }

    public static RuntimeResult Replace(IEnumerable<object?> args)
    {
        if (args.Count() != 3)
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "replace() takes exactly 3 arguments", Context.Root));
        }

        if (args.ElementAt(0) is string value && args.ElementAt(1) is string oldValue && args.ElementAt(2) is string newValue)
        {
            return RuntimeResult.Success(value.Replace(oldValue, newValue));
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "replace() takes only strings", Context.Root));
        }
    }

    #endregion

    #region Array functions

    public static RuntimeResult CreateArray(IEnumerable<object?> args)
    {
        // parameters: length, default value
        if (args.Count() == 1)
        {
            if (args.ElementAt(0) is double length)
            {
                return RuntimeResult.Success(Enumerable.Repeat<object>(null, (int)length).ToArray());
            }
            else
            {
                return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "arr() takes only numbers", Context.Root));
            }
        }
        else if (args.Count() == 2)
        {
            if (args.ElementAt(0) is double length && args.ElementAt(1) is not null)
            {
                return RuntimeResult.Success(Enumerable.Repeat<object>(args.ElementAt(1), (int)length).ToArray());
            }
            else
            {
                return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "arr() takes only numbers and values", Context.Root));
            }
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(InternalCodePosition, InternalCodePosition, "arr() takes 1 or 2 arguments", Context.Root));
        }
    }

    #endregion
}
