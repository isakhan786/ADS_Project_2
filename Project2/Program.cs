using System;
using System.Collections.Generic; // for Dictionary
using System.Linq; // for Stack

public class ExpressionEvaluator
{
    // precedence of operators in descending order (highest precedence first)
    private static Dictionary<string, int> precedence = new Dictionary<string, int>
    {
        { "^", 4 },
        { "*", 3 },
        { "/", 3 },
        { "+", 2 },
        { "-", 2 },
        { "and", 1 },
        { "or", 1 },
        { "not", 1 },
        { "<", 1 },
        { "<=", 1 },
        { ">", 1 },
        { ">=", 1 },
        { "=", 1 },
        { "=!", 1 },
        { "(", 0 },
    };

    // convert infix expression to postfix expression
    public static List<string> InfixToPostfix(string[] tokens)
    {
        Stack<string> operators = new Stack<string>(); // for operators and brackets
        List<string> postfixExp = new List<string>(); // final postfix expression

        foreach (string tk in tokens)
        {
            string token = tk.ToLower(); // convert token to lowercase
            // if token is a number or variable, add it to postfix expression
            if (double.TryParse(token, out _) || !precedence.ContainsKey(token) && token != "(" && token != ")")
            {
                if (tk != token)
                    postfixExp.Add(tk);
                else
                postfixExp.Add(token);
            }
            // if token is an '(' push it to operators stack
            else if (token == "(")
            {
                operators.Push(token);
            }
            // if token is an ')', pop operators stack until '(' is found and add them to postfix expression
            else if (token == ")")
            {
                while (operators.Count > 0 && operators.Peek() != "(")
                {
                    postfixExp.Add(operators.Pop());
                }
                operators.Pop(); // finally remove "("
            }
            // if token is an operator, pop operators stack until an operator with lower precedence
            // is found and add them to postfix expression
            else
            {
                while (operators.Count > 0 && precedence[operators.Peek()] >= precedence[token])
                {
                    postfixExp.Add(operators.Pop());
                }
                operators.Push(token);
            }
        }
        // pop remaining operators and add them to postfix expression
        while (operators.Count > 0)
        {
            postfixExp.Add(operators.Pop());
        }

        return postfixExp;
    }

    // evaluate postfix expression and return result
    public static double EvaluatePostfix(List<string> postfix, Dictionary<string, double> variableValues)
    {
        Stack<double> values = new Stack<double>(); // for operands

        foreach (string tk in postfix)
        {
            string token = tk.ToLower(); // convert token to lowercase
            // if token is a number, push it to values stack
            if (double.TryParse(token, out double val))
            {
                values.Push(val);
            }
            // if token is a variable, push its value to values stack
            else if (variableValues.ContainsKey(token) || variableValues.ContainsKey(tk))
            {
                if (tk != token)
                    values.Push(variableValues[tk]);
                else
                values.Push(variableValues[token]);
            }
            // if token is an operator, pop two values from values stack, apply the operator and push the result back
            else
            {
                double right = values.Pop();
                double left = values.Count > 0 ? values.Pop() : 0; // if there is only one operand, use 0 as the other operand (i.e. for unary operators)

                // apply the operator and push the result back
                switch (token)
                {
                    case "+":
                        values.Push(left + right);
                        break;
                    case "-":
                        values.Push(left - right);
                        break;
                    case "*":
                        values.Push(left * right);
                        break;
                    case "/":
                        values.Push(left / right);
                        break;
                    case "^":
                        values.Push(Math.Pow(left, right));
                        break;
                    case "and":
                        values.Push(left != 0 && right != 0 ? 1 : 0);
                        break;
                    case "or":
                        values.Push(left != 0 || right != 0 ? 1 : 0);
                        break;
                    case "not":
                        values.Push(right == 0 ? 1 : 0);
                        break;
                    case "<":
                        values.Push(left < right ? 1 : 0);
                        break;
                    case "<=":
                        values.Push(left <= right ? 1 : 0);
                        break;
                    case ">":
                        values.Push(left > right ? 1 : 0);
                        break;
                    case ">=":
                        values.Push(left >= right ? 1 : 0);
                        break;
                    case "=":
                        values.Push(left == right ? 1 : 0);
                        break;
                    case "=!":
                        values.Push(left != right ? 1 : 0);
                        break;
                }
            }
        }
        // return the final result
        return values.Pop();
    }

    public static void Main()
    {
        // read infix expression from user
        Console.WriteLine("Enter the infix expression:");
        string expression = Console.ReadLine();

        // split the expression into tokens
        string[] tokens = expression.Split(' ');

        // convert infix expression to postfix expression
        List<string> postfix = InfixToPostfix(tokens);

        // print postfix expression
        Console.WriteLine("Postfix expression:");
        foreach (string token in postfix)
        {
            Console.Write($"{token} ");
        }
        Console.WriteLine();

        // create a dictionary to store values for variables
        Dictionary<string, double> variableValues = new Dictionary<string, double>();

        // read values for variables
        foreach (string tk in tokens)
        {
            string token = tk.ToLower(); // convert token to lowercase
            // if token is a variable, read its value from user
            if (!precedence.ContainsKey(token) && token != "(" && token != ")")
            {
                if (!double.TryParse(token, out _))
                {
                    if (tk != token)
                    {
                        Console.WriteLine($"Enter value for {tk}:");
                        variableValues[tk] = Convert.ToDouble(Console.ReadLine());
                    }
                    else {
                        Console.WriteLine($"Enter value for {token}:");
                        variableValues[token] = Convert.ToDouble(Console.ReadLine());
                    }
                }
            }

        }
        // evaluate postfix expression and print result
        double result = EvaluatePostfix(postfix, variableValues);

        Console.WriteLine($"Result: {result}");
    }
}