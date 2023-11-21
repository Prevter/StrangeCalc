using StrangeCalc.Exceptions;
using StrangeCalc.RuntimeTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeCalc;

public sealed class RuntimeResult
{
	public object? Value { get; set; }
	public Error? Error { get; set; }

	public object? Register(RuntimeResult result)
	{
		if (result.Error is not null) Error = result.Error;
		return result.Value;
	}

	public static RuntimeResult Failure(Error error)
	{
		var result = new RuntimeResult
		{
			Error = error
		};
		return result;
	}

	public static RuntimeResult Success(object? value)
	{
		var result = new RuntimeResult
		{
			Value = value
		};
		return result;
	}

	public override string ToString()
	{
		if (Error is not null) return Error.ToString();

		return Value switch
		{
			double number => number.ToString(CultureInfo.InvariantCulture),
			bool boolean => boolean.ToString(),
			string text => text,
			Function func => func.ToString(),
			_ => "null"
		};
	}
}
