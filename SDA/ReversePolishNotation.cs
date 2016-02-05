namespace MathText
{
    using System;
    using System.Collections;
    using System.Text;
    using System.Text.RegularExpressions;

    public class ReversePolishNotation
    {
        private Queue output;
        private Stack operators;
        private string originalExpression;
        private string transitionExpression;
        private string postfixExpression;

        public string PostfixExpression
        {
            get
            {
                return this.postfixExpression;
            }
        }

        public ReversePolishNotation(string input)
        {
            this.originalExpression = input;
            this.transitionExpression = string.Empty;
            this.postfixExpression = string.Empty;
            this.output = new Queue();
            this.operators = new Stack();
        }

        public string Parse()
        {
            string sBuffer = this.originalExpression.ToLower();

            // The following is done so that the string would be able to be splited propertly

            // captures numbers
            sBuffer = Regex.Replace(sBuffer, @"(?<number>\d+(\.\d+)?)", " ${number} ");
            // captures these symbols: + - * / ^ ( )
            sBuffer = Regex.Replace(sBuffer, @"(?<ops>[+\-*/^()])", " ${ops} ");
            // captures alphabets
            sBuffer = Regex.Replace(sBuffer, "(?<alpha>(pi|e|sin|cos|tan))", " ${alpha} ");
            // trims up consecutive spaces and replace it with just one space
            sBuffer = Regex.Replace(sBuffer, @"\s+", " ").Trim();

            // The following code captures unary minus operations.

            // Step 1.
            sBuffer = Regex.Replace(sBuffer, "-", "MINUS");
            // Step 2. Looking for pi or e or generic number \d+(\.\d+)?
            sBuffer = Regex.Replace(sBuffer, @"(?<number>(pi|e|(\d+(\.\d+)?)))\s+MINUS", "${number} -");
            // Step 3. Use the tilde ~ as the unary minus operator
            sBuffer = Regex.Replace(sBuffer, "MINUS", "~");

            this.transitionExpression = sBuffer;

            // tokenise it!
            string[] saParsed = sBuffer.Split(" ".ToCharArray());
            int i = 0;
            double tokenvalue;
            ReversePolishNotationToken token, opstoken;

            for (i = 0; i < saParsed.Length; ++i)
            {
                token = new ReversePolishNotationToken();
                token.TokenValue = saParsed[i];
                token.TokenValueType = TokenType.None;

                try
                {
                    tokenvalue = double.Parse(saParsed[i]);
                    token.TokenValueType = TokenType.Number;
                    // If the token is a number, then add it to the output queue.
                    output.Enqueue(token);
                }
                catch
                {
                    switch (saParsed[i])
                    {
                        case "+":
                            token.TokenValueType = TokenType.Plus;
                            if (operators.Count > 0)
                            {
                                opstoken = (ReversePolishNotationToken) operators.Peek();
                                // while there is an operator, o2, at the top of the stack
                                while (IsOperatorToken(opstoken.TokenValueType))
                                {
                                    // pop o2 off the stack, onto the output queue;
                                    output.Enqueue(operators.Pop());
                                    if (operators.Count > 0)
                                    {
                                        opstoken = (ReversePolishNotationToken) operators.Peek();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            // push o1 onto the operator stack.
                            operators.Push(token);
                            break;
                        case "-":
                            token.TokenValueType = TokenType.Minus;
                            if (operators.Count > 0)
                            {
                                opstoken = (ReversePolishNotationToken) operators.Peek();
                                // while there is an operator, o2, at the top of the stack
                                while (IsOperatorToken(opstoken.TokenValueType))
                                {
                                    // pop o2 off the stack, onto the output queue;
                                    output.Enqueue(operators.Pop());
                                    if (operators.Count > 0)
                                    {
                                        opstoken = (ReversePolishNotationToken) operators.Peek();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            // push o1 onto the operator stack.
                            operators.Push(token);
                            break;
                        case "*":
                            token.TokenValueType = TokenType.Multiply;
                            if (operators.Count > 0)
                            {
                                opstoken = (ReversePolishNotationToken) operators.Peek();
                                // while there is an operator, o2, at the top of the stack
                                while (IsOperatorToken(opstoken.TokenValueType))
                                {
                                    if (opstoken.TokenValueType == TokenType.Plus ||
                                        opstoken.TokenValueType == TokenType.Minus)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        // Once we're in here, the following algorithm condition is satisfied.
                                        // o1 is associative or left-associative and its precedence is less than (lower precedence) or equal to that of o2, or
                                        // o1 is right-associative and its precedence is less than (lower precedence) that of o2,

                                        // pop o2 off the stack, onto the output queue;
                                        output.Enqueue(operators.Pop());
                                        if (operators.Count > 0)
                                        {
                                            opstoken = (ReversePolishNotationToken) operators.Peek();
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            // push o1 onto the operator stack.
                            operators.Push(token);
                            break;
                        case "/":
                            token.TokenValueType = TokenType.Divide;
                            if (operators.Count > 0)
                            {
                                opstoken = (ReversePolishNotationToken) operators.Peek();
                                // while there is an operator, o2, at the top of the stack
                                while (IsOperatorToken(opstoken.TokenValueType))
                                {
                                    if (opstoken.TokenValueType == TokenType.Plus ||
                                        opstoken.TokenValueType == TokenType.Minus)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        // Once we're in here, the following algorithm condition is satisfied.
                                        // o1 is associative or left-associative and its precedence is less than (lower precedence) or equal to that of o2, or
                                        // o1 is right-associative and its precedence is less than (lower precedence) that of o2,

                                        // pop o2 off the stack, onto the output queue;
                                        output.Enqueue(operators.Pop());
                                        if (operators.Count > 0)
                                        {
                                            opstoken = (ReversePolishNotationToken) operators.Peek();
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            // push o1 onto the operator stack.
                            operators.Push(token);
                            break;
                        case "^":
                            token.TokenValueType = TokenType.Exponent;
                            // push o1 onto the operator stack.
                            operators.Push(token);
                            break;
                        case "~":
                            token.TokenValueType = TokenType.UnaryMinus;
                            // push o1 onto the operator stack.
                            operators.Push(token);
                            break;
                        case "(":
                            token.TokenValueType = TokenType.LeftParenthesis;
                            // If the token is a left parenthesis, then push it onto the stack.
                            operators.Push(token);
                            break;
                        case ")":
                            token.TokenValueType = TokenType.RightParenthesis;
                            if (operators.Count > 0)
                            {
                                opstoken = (ReversePolishNotationToken) operators.Peek();
                                // Until the token at the top of the stack is a left parenthesis
                                while (opstoken.TokenValueType != TokenType.LeftParenthesis)
                                {
                                    // pop operators off the stack onto the output queue
                                    output.Enqueue(operators.Pop());
                                    if (operators.Count > 0)
                                    {
                                        opstoken = (ReversePolishNotationToken) operators.Peek();
                                    }
                                    else
                                    {
                                        // If the stack runs out without finding a left parenthesis,
                                        // then there are mismatched parentheses.
                                        throw new Exception("Unbalanced parenthesis!");
                                    }

                                }
                                // Pop the left parenthesis from the stack, but not onto the output queue.
                                operators.Pop();
                            }

                            if (operators.Count > 0)
                            {
                                opstoken = (ReversePolishNotationToken) operators.Peek();
                                // If the token at the top of the stack is a function token
                                if (IsFunctionToken(opstoken.TokenValueType))
                                {
                                    // pop it and onto the output queue.
                                    output.Enqueue(operators.Pop());
                                }
                            }
                            break;
                        case "pi":
                            token.TokenValueType = TokenType.Constant;
                            // If the token is a number, then add it to the output queue.
                            output.Enqueue(token);
                            break;
                        case "e":
                            token.TokenValueType = TokenType.Constant;
                            // If the token is a number, then add it to the output queue.
                            output.Enqueue(token);
                            break;
                        case "sin":
                            token.TokenValueType = TokenType.Sine;
                            // If the token is a function token, then push it onto the stack.
                            operators.Push(token);
                            break;
                        case "cos":
                            token.TokenValueType = TokenType.Cosine;
                            // If the token is a function token, then push it onto the stack.
                            operators.Push(token);
                            break;
                        case "tan":
                            token.TokenValueType = TokenType.Tangent;
                            // If the token is a function token, then push it onto the stack.
                            operators.Push(token);
                            break;
                    }
                }
            }

            // When there are no more tokens to read:

            // While there are still operator tokens in the stack:
            while (operators.Count != 0)
            {
                opstoken = (ReversePolishNotationToken) operators.Pop();
                // If the operator token on the top of the stack is a parenthesis
                if (opstoken.TokenValueType == TokenType.LeftParenthesis)
                {
                    // then there are mismatched parenthesis.
                    throw new Exception("Unbalanced parenthesis!");
                }
                else
                {
                    // Pop the operator onto the output queue.
                    output.Enqueue(opstoken);
                }
            }

            StringBuilder postfixExpressionResult = new StringBuilder();

            foreach (ReversePolishNotationToken RPNToken in output)
            {
                opstoken = RPNToken;
                postfixExpressionResult.Append(string.Format("{0} ", opstoken.TokenValue));
            }

            this.postfixExpression = postfixExpressionResult.ToString();
            return this.PostfixExpression;
        }

        public double Evaluate()
        {
            Stack result = new Stack();
            double firstOperator = 0.0, secondOperator = 0.0;
            ReversePolishNotationToken token = new ReversePolishNotationToken();

            // While there are input tokens left
            foreach (ReversePolishNotationToken RPNToken in output)
            {
                // Read the next token from input.
                token = RPNToken;
                switch (token.TokenValueType)
                {
                    case TokenType.Number:
                        result.Push(double.Parse(token.TokenValue));
                        break;
                    case TokenType.Constant:
                        result.Push(EvaluateConstant(token.TokenValue));
                        break;
                    case TokenType.Plus:
                        if (result.Count >= 2)
                        {
                            // So, pop the top 2 values from the stack.
                            secondOperator = (double) result.Pop();
                            firstOperator = (double) result.Pop();
                            result.Push(firstOperator + secondOperator);
                        }
                        else
                        {
                            // (Error) The user has not input sufficient values in the expression.
                            throw new Exception("Evaluation error!");
                        }
                        break;
                    case TokenType.Minus:
                        if (result.Count >= 2)
                        {
                            // So, pop the top 2 values from the stack.
                            secondOperator = (double) result.Pop();
                            firstOperator = (double) result.Pop();
                            result.Push(firstOperator - secondOperator);
                        }
                        else
                        {
                            // (Error) The user has not input sufficient values in the expression.
                            throw new Exception("Evaluation error!");
                        }
                        break;
                    case TokenType.Multiply:
                        if (result.Count >= 2)
                        {
                            // So, pop the top 2 values from the stack.
                            secondOperator = (double) result.Pop();
                            firstOperator = (double) result.Pop();
                            result.Push(firstOperator*secondOperator);
                        }
                        else
                        {
                            // (Error) The user has not input sufficient values in the expression.
                            throw new Exception("Evaluation error!");
                        }
                        break;
                    case TokenType.Divide:
                        if (result.Count >= 2)
                        {
                            // So, pop the top 2 values from the stack.
                            secondOperator = (double) result.Pop();
                            firstOperator = (double) result.Pop();
                            result.Push(firstOperator/secondOperator);
                        }
                        else
                        {
                            // (Error) The user has not input sufficient values in the expression.
                            throw new Exception("Evaluation error!");
                        }
                        break;
                    case TokenType.Exponent:
                        if (result.Count >= 2)
                        {
                            // So, pop the top 2 values from the stack.
                            secondOperator = (double) result.Pop();
                            firstOperator = (double) result.Pop();
                            result.Push(Math.Pow(firstOperator, secondOperator));
                        }
                        else
                        {
                            // (Error) The user has not input sufficient values in the expression.
                            throw new Exception("Evaluation error!");
                        }
                        break;
                    case TokenType.UnaryMinus:
                        if (result.Count >= 1)
                        {
                            // So, pop the top value from the stack.
                            firstOperator = (double) result.Pop();
                            result.Push(-firstOperator);
                        }
                        else
                        {
                            // (Error) The user has not input sufficient values in the expression.
                            throw new Exception("Evaluation error!");
                        }
                        break;
                    case TokenType.Sine:
                        if (result.Count >= 1)
                        {
                            // So, pop the top value from the stack.
                            firstOperator = (double) result.Pop();
                            result.Push(Math.Sin(firstOperator));
                        }
                        else
                        {
                            // (Error) The user has not input sufficient values in the expression.
                            throw new Exception("Evaluation error!");
                        }
                        break;
                    case TokenType.Cosine:
                        if (result.Count >= 1)
                        {
                            // So, pop the top value from the stack.
                            firstOperator = (double) result.Pop();
                            result.Push(Math.Cos(firstOperator));
                        }
                        else
                        {
                            // (Error) The user has not input sufficient values in the expression.
                            throw new Exception("Evaluation error!");
                        }
                        break;
                    case TokenType.Tangent:
                        if (result.Count >= 1)
                        {
                            // So, pop the top value from the stack.
                            firstOperator = (double) result.Pop();
                            result.Push(Math.Tan(firstOperator));
                        }
                        else
                        {
                            // (Error) The user has not input sufficient values in the expression.
                            throw new Exception("Evaluation error!");
                        }
                        break;
                }
            }

            // If there is only one value in the stack
            if (result.Count == 1)
            {
                // That value is the result of the calculation.
                return (double) result.Pop();
            }
            else
            {
                // If there are more values in the stack
                // (Error) The user input too many values.
                throw new Exception("Evaluation error!");
            }
        }

        private bool IsOperatorToken(TokenType t)
        {
            bool result = false;
            switch (t)
            {
                case TokenType.Plus:
                case TokenType.Minus:
                case TokenType.Multiply:
                case TokenType.Divide:
                case TokenType.Exponent:
                case TokenType.UnaryMinus:
                    result = true;
                    break;
                default:
                    result = false;
                    break;
            }
            return result;
        }

        private bool IsFunctionToken(TokenType t)
        {
            bool result = false;
            switch (t)
            {
                case TokenType.Sine:
                case TokenType.Cosine:
                case TokenType.Tangent:
                    result = true;
                    break;
                default:
                    result = false;
                    break;
            }
            return result;
        }

        private double EvaluateConstant(string TokenValue)
        {
            double result = 0.0;
            switch (TokenValue)
            {
                case "pi":
                    result = Math.PI;
                    break;
                case "e":
                    result = Math.E;
                    break;
            }
            return result;
        }
    }
}