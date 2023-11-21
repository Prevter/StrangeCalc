#pragma warning disable CS0162 // Unreachable code detected
namespace StrangeCalc;

/// <summary>
/// Math operations to get funny results
/// </summary>
public static class SpecialMath
{
    const bool USE_REAL_MATH = true;

    public static double Add(double a, double b)
    {
        if (USE_REAL_MATH)
            return a + b;

        // we do some funny math here
        // should make these calculations:
        // 2 + 2 = 5
        // 9 + 10 = 21
        // 66 + 44 = 100
        // etc...

        // get the "real" answer
        double wrongResult = a + b;

        // get the decimal part of the result
        double decimalPart = wrongResult - (int)wrongResult;

        // get the integer part of the inputs
        int av = (int)a;
        int bv = (int)b;

        // initialize temporary variables
        int av2 = av;
        int bv2 = bv;

        // save the sum of the digits of the inputs
        List<int> numbers = new();
        while (av2 > 0 || bv2 > 0)
        {
            numbers.Add((av2 % 10) + (bv2 % 10));
            av2 /= 10;
            bv2 /= 10;
        }

        int result = 0;
        // check if the inputs are equal and less than 10, if so, add 1 to the result (2 + 2 = 5)
        if (av == bv && av < 10 && bv < 10)
        {
            result += 1;
        }
        // check if the inputs are 9 and 10, if so, add 2 to the result (9 + 10 = 21)
        else if ((av == 9 && bv == 10) || (bv == 9 && av == 10))
        {
            result += 2;
        }

        // then add the numbers
        for (int i = 0; i < numbers.Count; i++)
        {
            // if we are at the last digit (most-left), add it normally
            if (i + 1 == numbers.Count)
            {
                result += numbers[i] * (int)Math.Pow(10, i);
            }
            // else, we need to modulo it with 10, so we can actually get 777 + 333 = 1000
            // 700 + 300 = 1000 (greatest digit works as normal)
            // 70 + 30 = 0 (because 100 % 10 = 0)
            // 7 + 3 = 0 (because 10 % 10 = 0)
            else
            {
                result += numbers[i] % 10 * (int)Math.Pow(10, i);
            }
        }

        // add the decimal part back to the result
        return result + decimalPart;
    }

    // no ideas for these ones, so just use the normal operators
    public static double Minus(double a, double b) => a - b;
    public static double Multiply(double a, double b) => a * b;
    public static double Divide(double a, double b) => a / b;

    public static bool Equals(double a, double b)
    {
        if (USE_REAL_MATH)
            return a == b;

        // who cares about precision anyway
        return Math.Round(a) == Math.Round(b);
    }
}
