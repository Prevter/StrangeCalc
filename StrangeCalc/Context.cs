using StrangeCalc.Exceptions;
using StrangeCalc.RuntimeTypes;

namespace StrangeCalc;

public sealed class Context
{
    public string Name { get; set; }
    public Dictionary<string, Variable> Variables { get; } = new();
    public Context? Parent { get; set; }
    public CodePosition EntryPosition { get; set; }
    public static Context Root { get; set; }

    public Context(string name, CodePosition entryPosition)
    {
        Name = name;
        EntryPosition = entryPosition;

        if (Root is null)
        {
            Root = this;
            // Built-in constants
            Variables.Add("pi", new Variable(Math.PI, true));
            Variables.Add("e", new Variable(Math.E, true));
            Variables.Add("null", new Variable(null, true));
            Variables.Add("true", new Variable(true, true));
            Variables.Add("false", new Variable(false, true));

            // Built-in functions
            Variables.Add("print", new Variable(new Function("print", BuiltInFunctions.Print), true));
            Variables.Add("printf", new Variable(new Function("printf", BuiltInFunctions.Printf), true));
            Variables.Add("sprintf", new Variable(new Function("sprintf", BuiltInFunctions.SPrintf), true));
            Variables.Add("scanf", new Variable(new Function("scanf", BuiltInFunctions.Scanf), true));
            Variables.Add("clear", new Variable(new Function("clear", BuiltInFunctions.Clear), true));

            Variables.Add("sin", new Variable(new Function("sin", BuiltInFunctions.Sin), true));
            Variables.Add("cos", new Variable(new Function("cos", BuiltInFunctions.Cos), true));
            Variables.Add("tan", new Variable(new Function("tan", BuiltInFunctions.Tan), true));

            Variables.Add("asin", new Variable(new Function("asin", BuiltInFunctions.Asin), true));
            Variables.Add("acos", new Variable(new Function("acos", BuiltInFunctions.Acos), true));
            Variables.Add("atan", new Variable(new Function("atan", BuiltInFunctions.Atan), true));

            Variables.Add("sinh", new Variable(new Function("sinh", BuiltInFunctions.Sinh), true));
            Variables.Add("cosh", new Variable(new Function("cosh", BuiltInFunctions.Cosh), true));
            Variables.Add("tanh", new Variable(new Function("tanh", BuiltInFunctions.Tanh), true));

            Variables.Add("sqrt", new Variable(new Function("sqrt", BuiltInFunctions.Sqrt), true));
            Variables.Add("pow", new Variable(new Function("pow", BuiltInFunctions.Pow), true));
            Variables.Add("abs", new Variable(new Function("abs", BuiltInFunctions.Abs), true));
            Variables.Add("floor", new Variable(new Function("floor", BuiltInFunctions.Floor), true));
            Variables.Add("ceil", new Variable(new Function("ceil", BuiltInFunctions.Ceil), true));
            Variables.Add("round", new Variable(new Function("round", BuiltInFunctions.Round), true));
            Variables.Add("min", new Variable(new Function("min", BuiltInFunctions.Min), true));
            Variables.Add("max", new Variable(new Function("max", BuiltInFunctions.Max), true));
            Variables.Add("clamp", new Variable(new Function("clamp", BuiltInFunctions.Clamp), true));
            Variables.Add("random", new Variable(new Function("random", BuiltInFunctions.Random), true));
            Variables.Add("log", new Variable(new Function("log", BuiltInFunctions.Log), true));
            Variables.Add("log10", new Variable(new Function("log10", BuiltInFunctions.Log10), true));
            Variables.Add("log2", new Variable(new Function("log2", BuiltInFunctions.Log2), true));
            Variables.Add("ln", new Variable(new Function("ln", BuiltInFunctions.Ln), true));

            Variables.Add("len", new Variable(new Function("len", BuiltInFunctions.Length), true));
            Variables.Add("substr", new Variable(new Function("substr", BuiltInFunctions.Substring), true));
            Variables.Add("replace", new Variable(new Function("replace", BuiltInFunctions.Replace), true));
        }
    }

    public Variable? GetVariable(string name)
    {
        if (Variables.TryGetValue(name, out var variable))
        {
            return variable;
        }
        else
        {
            if (Parent is not null)
            {
                return Parent.GetVariable(name);
            }
            else
            {
                return null;
            }
        }
    }

    public void SetVariable(string name, object? value, bool constant = false)
    {
        if (Variables.ContainsKey(name))
        {
            if (Variables[name].IsConstant)
            {
                throw new Exception($"Cannot assign to constant {name}");
            }

            Variables[name] = new Variable(value, constant);
        }
        else
        {
            if (Parent is not null)
            {
                if (Parent.GetVariable(name) != null)
                    Parent.SetVariable(name, value, constant);
            }
            else
            {
                Variables.Add(name, new Variable(value, constant));
            }
        }
    }
}

public struct Variable
{
    public object? Value { get; set; }
    public bool IsConstant { get; }

    public Variable(object? value, bool isConstant)
    {
        Value = value;
        IsConstant = isConstant;
    }
}