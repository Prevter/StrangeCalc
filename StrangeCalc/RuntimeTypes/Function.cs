using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeCalc.RuntimeTypes;

public sealed class Function
{
	public string Name { get; set; }
	public IEnumerable<string>? Arguments { get; }
	public IEnumerable<INode>? Body { get; }
	public Func<IEnumerable<object?>, RuntimeResult>? Native { get; }

	public CodePosition Start { get; set; }
	public CodePosition End { get; set; }

	public Function(IEnumerable<string> arguments, IEnumerable<INode> body, string name = "", CodePosition start = null, CodePosition end = null)
	{
		if (string.IsNullOrWhiteSpace(name)) name = "<anonymous>";
		Name = name;
		Arguments = arguments;
		Body = body;
		Start = start;
		End = end;
	}

	public Function(string name, Func<IEnumerable<object?>, RuntimeResult> native)
	{
		Native = native;
		Name = name;
		Start = new CodePosition(0, 0, 0, "<native>", "");
		End = Start;
	}

	public RuntimeResult Call(IEnumerable<object?> arguments, Context context)
	{
		if (Native is not null)
		{
			return Native(arguments);
		}

		Context local = new(Name, Start) { Parent = context };

		int i = 0;
		foreach (var argument in arguments)
		{
			local.SetVariable(Arguments!.ElementAt(i), argument);
			i++;
		}

		var interpreter = new Interpreter(Body!, local);
		var result = interpreter.Run();

		if (result.Error is not null)
		{
			return RuntimeResult.Failure(result.Error);
		}

		return RuntimeResult.Success(result.Value);
	}

	public override string ToString() => $"<function {Name}>";
}
