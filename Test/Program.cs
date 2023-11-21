using StrangeCalc;

// check if the user has provided a filename
if (args.Length != 0)
{
	var filename = args[0];
	if (!File.Exists(filename))
	{
		Console.WriteLine($"File '{filename}' does not exist.");
		return;
	}

	var source = File.ReadAllText(filename);
	var context = new Context(filename, new CodePosition(0,1,1,filename,source));
	try
	{
		var result = Calc.Evaluate(source, filename, context);
		if (result.Value is not null || result.Error is not null) Console.WriteLine(result);
	}
	catch (Exception ex)
	{
		Console.WriteLine(ex.Message);
		Console.WriteLine(ex.StackTrace);
	}
	return;
}
else
{
	Console.WriteLine("REPL Console");
	var context = new Context("<stdin>", new CodePosition(0,0,0,"<stdin>",""));
	do
	{
		Console.Write("> ");
		var line = Console.ReadLine();

		if (string.IsNullOrWhiteSpace(line))
			continue;

		try
		{
			var result = Calc.Evaluate(line, "<stdin>", context);
			if (result.Value is not null || result.Error is not null) Console.WriteLine(result);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			Console.WriteLine(ex.StackTrace);
			continue;
		}

	} while (true);
}