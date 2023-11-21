using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeCalc;

public static class Calc
{
	public static RuntimeResult Evaluate(string expression, string filename, Context context)
	{
		var lexer = new Lexer(expression, filename);
		var parser = new Parser(lexer);
		var interpreter = new Interpreter(parser.Parse(), context);

		return interpreter.Run();
	}
}
