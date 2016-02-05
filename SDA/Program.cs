using System;

class Program
{
    static void Main(string[] args)
    {
        string input = Console.ReadLine();

        // 5 + sin(pi) / 2^10 - log(e, e^sqrt(4)) -> 3
        // 3 / 0 -> exception
        // log(0, 10) -> exception

        MathText.ReversePolishNotation rpn = new MathText.ReversePolishNotation(input);

        string postfixExpression = rpn.Parse();
        double result = rpn.Evaluate();

        Console.WriteLine("original input: {0}", input);
        Console.WriteLine("postfix expression: {0}", postfixExpression);
        Console.WriteLine("result: {0:F2}", result);
    }
}