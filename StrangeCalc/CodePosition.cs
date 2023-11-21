using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeCalc;

public sealed class CodePosition
{
	public int Index { get; set; }
	public int Line { get; set; }
	public int Column { get; set; }
	public string Filename { get; set; }
	public string FileContent { get; set; }

	public CodePosition(int index, int line, int column, string filename, string fileContent)
	{
		Index = index;
		Line = line;
		Column = column;
		Filename = filename;
		FileContent = fileContent;
	}

	public void Advance(char? currentChar = null)
	{
		Index++; Column++;
		if (currentChar == '\n')
		{
			Line++; Column = 0;
		}
	}

	public CodePosition Copy()
	{
		return new CodePosition(Index, Line, Column, Filename, FileContent);
	}

	public override string ToString()
	{
		return $"Index {Index}, Line {Line}, Column {Column}";
	}

}
