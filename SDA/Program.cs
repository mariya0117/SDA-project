using System;

class Program
{
    static void Main(string[] args)
    {
        string input = "3+4*2/(1-5)^2^3"; //Console.ReadLine();

        MathText.ReversePolishNotation rpn = new MathText.ReversePolishNotation(input);

        string postfixExpression = rpn.Parse();
        double result = rpn.Evaluate();

        Console.WriteLine("original input: {0}", input);
        Console.WriteLine("postfix expression: {0}", postfixExpression);
        Console.WriteLine("result: {0:F2}", result);
    }
}