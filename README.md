# StrangeCalc

> Project is not under active development, as it was created as a joke with no real purpose. However, I'm still open to [pull requests](https://github.com/Prevter/StrangeCalc/pulls) and [issues](https://github.com/Prevter/StrangeCalc/issues). You can also give ideas for new features in the [Discussions](https://github.com/Prevter/StrangeCalc/discussions) section.

StrangeCalc is a programming language written in C# that features inconsistent math operations. In this language, simple arithmetic calculations may yield unexpected results. For example, 2 + 2 could equal 5, 9 + 10 could equal 21, and 77 + 33 could equal 100.

## Why?

I don't have an answer to that question. I just thought it would be fun to make a programming language that doesn't make sense, learning more about how programming languages work in the process. The language doesn't have any practical use, so it's basically educational only (or maybe you can use it to troll your friends).

There is also an easy way to disable the wrong math, as they've all been moved into a separate file, so you could in theory use it as a normal programming language.

## Features

- Variables: You can declare and use variables to store values.
- Loops: You can use loops to repeat a block of code multiple times.
- Built-in Functions: The language provides a set of built-in functions for performing various operations.
- If/Else If/Else Logic: You can use conditional statements to control the flow of your program.
- Strings: The language supports string manipulation and operations.
- Basic Terminal I/O: You can interact with the user through the terminal by reading input and printing output.

## Basic Syntax

Here's an example of the basic syntax in StrangeCalc:
```js
// This is a comment
/* This is a
   multi-line comment */

// Declare a variable
x = 5; // note the semicolon
a = 2; b = 3; // you can use it to separate statements on the same line

// "Hello, world" program:
print("Hello, world!");

// Conditional statements:
if (x == 5) {
    print("x is 5");
} else if (x == 6) {
    print("x is 6");
} else {
    print("x is not 5 or 6");
}

// Loops:
for (i = 0; i < 5; i = i + 1) {
    print(i); // prints 0, 1, 3, 4
    // yes, the loop skips 2, because 1 + 1 = 3
}

// Built-in functions:
printf("The square root of 4 is {0}\n", sqrt(4)); // The square root of 4 is 2

// Strings:
str1 = "Hello"
str2 = "world"
print(str1 + ", " + str2 + "!"); // Hello, world!

// Basic terminal I/O:
printf("Enter your name: ");
name = scanf("%s");
printf("Hello, {0}!\n", name);

// And of course, math:
print(2 + 2); // 5
print(9 + 10); // 21
print(77 + 33); // 100
print(pi, pi == 3); // 3.14159265358979323846 True
```

## How to Use
1. Clone the repository onto your computer
```bash
git clone https://github.com/Prevter/StrangeCalc.git
```
2. Compile "Test" project. This project contains actual REPL console, while "StrangeCalc" project contains only the core of the language.
3. You can either start the compiled program on itself, to launch the REPL console, or you can pass a file path as an argument to the program to run a script.
```bash
StrangeCalc.exe
StrangeCalc.exe "path/to/script.str"
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details