using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeCalc.Exceptions;

public class Error : Exception
{
    public string Name { get; }
    public string Details { get; }

    public CodePosition Start { get; }
    public CodePosition End { get; }

    public Error(string name, string message, CodePosition start, CodePosition end)
    {
        Name = name;
        Details = message;
        Start = start;
        End = end;
    }

    public static string ArrowString(string text, CodePosition start, CodePosition end)
    {
        string result = "";

        int indexStart = Math.Max(text.LastIndexOf('\n', start.Index - 1), 0);
        int indexEnd = text.IndexOf('\n', start.Index - 1);
        if (indexEnd < 0) indexEnd = text.Length;

        int lineCount = end.Line - start.Line + 1;
        for (int i = 0; i < lineCount; i++)
        {
            string line = text[indexStart..indexEnd];
            var columnStart = i == 0 ? start.Column : 0;
            var columnEnd = i == lineCount - 1 ? end.Column : line.Length - 1;

            result += line + '\n';
            for (int j = 0; j < columnStart; j++) result += ' ';
            for (int j = 0; j < columnEnd - columnStart; j++) result += '^';

            indexStart = indexEnd;
            indexEnd = text.IndexOf('\n', indexStart - 1);
            if (indexEnd < 0) indexEnd = text.Length;
        }

        return result.Replace("\t", "");
    }

    public override string ToString()
    {
        return $"{Name}: {Details}\n" +
               $"  File {Start.Filename}, line {Start.Line + 1}\n";
        //$"{ArrowString(PositionStart.Filetext, PositionStart, PositionEnd)}"
    }

}
