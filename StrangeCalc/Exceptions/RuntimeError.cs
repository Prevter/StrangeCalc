using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeCalc.Exceptions;

public class RuntimeError : Error
{
	public Context Context { get; set; }

	public RuntimeError(CodePosition start, CodePosition end, string details, Context context)
			: base("RuntimeError", details, start, end)
	{
		Context = context;
	}

	public string GenerateTraceback()
	{
		string result = "";
		var pos = Start;
		var ctx = Context;

		while (ctx != null)
		{
			result = $"  File {pos.Filename}, line {pos.Line + 1}, in {ctx.Name}\n" + result;
			pos = ctx.EntryPosition;
			ctx = ctx.Parent;
		}
		return "Traceback (most recent call last):\n" + result;
	}

	public override string ToString()
	{
		return GenerateTraceback() +
			   $"{Name}: {Details}";
		//$"{ArrowString(PositionStart.Filetext, PositionStart, PositionEnd)}"
	}
}
