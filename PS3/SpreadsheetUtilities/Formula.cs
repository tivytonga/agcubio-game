// Skeleton written by Joe Zachary for CS 3500, September 2013
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

// Eric Longberg
// CS 3500, PS3
// September 24, 2015
namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax; variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        String expression; // The normalized, validated formula, suitable for ToString()
        HashSet<String> variables; // A list of unique, normalized variables

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            expression = "";
            variables = new HashSet<string>();

            int numRightParens = 0;
            int numLeftParens = 0;
            string previousToken = "";

            foreach (string token in GetTokens(formula))
            {
                if (expression == "")
                {
                    if (!(token.IsNumber() || token.IsLeftParen() || token.IsVariable()))
                    {
                        throw new FormulaFormatException("First token "+token+" is invalid. Must start with a number, left parenthesis, or variable.");
                    }
                }

                if (previousToken.IsLeftParen() || previousToken.IsOperator()) {
                    if (!(token.IsNumber() || token.IsVariable() || token.IsLeftParen())) {
                        throw new FormulaFormatException("Read token " + previousToken + " . Expected a number, variable, or left parenthesis to follow, but instead found: " + token);
                    }
                }

                if (previousToken.IsNumber() || previousToken.IsVariable() || previousToken.IsRightParen())
                {
                    if (!(token.IsOperator() || token.IsRightParen())) {
                        throw new FormulaFormatException("Read token " + previousToken + " . Expected an operator or right parenthesis to follow, but instead found: " + token);
                    }
                }

                if (token.IsRightParen())
                {
                    numRightParens++;
                    if (numRightParens > numLeftParens)
                    {
                        throw new FormulaFormatException("Too many close parenthesis.");
                    }
                    expression += token;
                }

                else if (token.IsLeftParen())
                {
                    numLeftParens++;
                    expression += token;
                }

                else if (token.IsOperator())
                {
                    expression += token;
                }

                else if (token.IsNumber())
                {
                    double d;
                    Double.TryParse(token, out d);
                    expression += d;
                }

                else if (token.IsVariable())
                {
                    string var;
                    try
                    {
                        var = normalize(token);
                    }
                    catch
                    {
                        throw new FormulaFormatException("Token " + token + "could not be normalized.");
                    }
                    if (!isValid(var))
                    {
                        throw new FormulaFormatException("Token " + token + " (normalized: " + var + " ) is not valid.");
                    }
                    expression += var;
                    
                    // If variable unique, add to list
                    if (!variables.Contains(var))
                    {
                        variables.Add(var);
                    }
                }

                else
                {
                    throw new FormulaFormatException("Unrecognized token: " + token);
                }

                previousToken = token;
            }


            if (expression == "")
                throw new FormulaFormatException("Formula contains no valid tokens.");

            if (!(previousToken.IsNumber() || previousToken.IsVariable() || previousToken.IsRightParen()))
            {
                // Must end with number, variable, or closing parenthesis
                throw new FormulaFormatException("Final token "+previousToken+" is invalid. Must end with a number, right parenthesis, or variable.");
            }

            if (numRightParens < numLeftParens)
            {
                throw new FormulaFormatException("Too many open parenthesis.");
            }
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            ////copied
            //todo: make work
            Stack<string> opStack = new Stack<string>();
            Stack<double> valStack = new Stack<double>();
            foreach (string token in GetTokens(expression))
            {
                // Try to read a double
                double num;
                if (Double.TryParse(token, out num))
                {
                    if (opStack.Count > 0)
                    {
                        string op = opStack.Peek();
                        if (op.Equals("*") || op.Equals("/"))
                        {
                            opStack.Pop();
                            double leftValue = valStack.Pop();
                            try
                            {
                                valStack.Push(applyOperation(op, leftValue, num));
                            }
                            catch
                            {
                                return new FormulaError("Division by 0.");
                            }
                        }
                        else
                            valStack.Push(num);
                    }
                    else
                        valStack.Push(num);
                    continue;
                }

                // Try to read + or -
                if (token.Equals("+") || token.Equals("-"))
                {
                    if (opStack.Count > 0)
                    {
                        string op = opStack.Peek();
                        if (op.Equals("+") || op.Equals("-"))
                        {
                            opStack.Pop();
                            double rightValue = valStack.Pop();
                            double leftValue = valStack.Pop();
                            valStack.Push(applyOperation(op, leftValue, rightValue));
                        }
                    }

                    opStack.Push(token);
                    continue;
                }

                // Try to read *, /, (
                if (token.Equals("*") || token.Equals("/") || token.Equals("("))
                {
                    opStack.Push(token);
                    continue;
                }

                // Try to read )
                if (token.Equals(")"))
                {
                    string op;
                    if (opStack.Count > 0)
                    {
                        op = opStack.Peek();
                        if (op.Equals("+") || op.Equals("-"))
                        {
                            opStack.Pop();
                            double rightValue = valStack.Pop();
                            double leftValue = valStack.Pop();
                            valStack.Push(applyOperation(op, leftValue, rightValue));
                        }
                    }

                    op = opStack.Pop();

                    if (opStack.Count > 0)
                    {
                        op = opStack.Peek();
                        if (op.Equals("*") || op.Equals("/"))
                        {
                            opStack.Pop();
                            double rightValue = valStack.Pop();
                            double leftValue = valStack.Pop();
                            try
                            {
                                valStack.Push(applyOperation(op, leftValue, rightValue));
                            }
                            catch
                            {
                                return new FormulaError("Division by 0.");
                            }
                        }
                    }

                    continue;
                }

                // Try to read a variable
                else
                {
                    try
                    {
                        num = lookup(token);
                    }
                    catch
                    {
                        return new FormulaError("Variable " + token + " not found.");
                    }

                    if (opStack.Count > 0)
                    {
                        string op = opStack.Peek();
                        if (op.Equals("*") || op.Equals("/"))
                        {
                            opStack.Pop();
                            double leftValue = valStack.Pop();
                            try
                            {
                                valStack.Push(applyOperation(op, leftValue, num));
                            }
                            catch
                            {
                                return new FormulaError("Division by 0.");
                            }
                        }
                        else
                            valStack.Push(num);
                    }
                    else
                        valStack.Push(num);
                    continue;
                }
            }
            
            if (opStack.Count == 1 && valStack.Count == 2)
            {
                double valRight = valStack.Pop();
                double valLeft = valStack.Pop();
                string op = opStack.Pop();
                try
                {
                    valStack.Push(applyOperation(op, valLeft, valRight));
                }
                catch
                {
                    return new FormulaError("Division by 0.");
                }
            }

            return valStack.Pop();
        }

        /// <summary>
        /// Applies the give operation (+, -, *, /) to the given parameters.
        /// Throws ArgumentException if divides by 0 or invalid operation.
        /// </summary>
        /// <param name="op"></param>
        /// <param name="leftValue"></param>
        /// <param name="rightValue"></param>
        /// <returns></returns>
        private static double applyOperation(string op, double leftValue, double rightValue)
        {
            if (op.Equals("+"))
                return leftValue + rightValue;
            if (op.Equals("-"))
                return leftValue - rightValue;
            if (op.Equals("*"))
                return leftValue * rightValue;
            if (op.Equals("/"))
            {
                if (rightValue.Equals(0))
                    throw new ArgumentException("Division by 0.");
                return leftValue / rightValue;
            }

            // Should never happen for our Formulas
            throw new Exception();
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            return (new List<string>(variables)).AsReadOnly();
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return expression;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens, which are compared as doubles, and variable tokens,
        /// whose normalized forms are compared as strings.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is Formula)
            {
                if (((Formula)obj).ToString() == ToString())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {

            if ((object)f1 == null && (object)f2 == null)
                return true;
            if ((object)f1 == null || (object)f2 == null)
                return false;
            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !(f1 == f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            // Piggybacking off of hashcode for String
            return expression.GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }

    }

    /// <summary>
    /// Extensions to String class to make symbol comparing a bit easier.
    /// </summary>
    static class StringExtensions
    {
        /// <summary>
        /// Returns true if the string is a double literal.
        /// </summary>
        /// <returns></returns>
        public static bool IsNumber(this string s)
        {
            //return Regex.IsMatch(s, @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?");
            double d;
            return Double.TryParse(s, out d);
        }

        /// <summary>
        /// Returns true if the string is a left parenthesis.
        /// </summary>
        /// <returns></returns>
        public static bool IsLeftParen(this string s)
        {
            return s == "(";
        }

        /// <summary>
        /// Returns true if the string is a right parenthesis.
        /// </summary>
        /// <returns></returns>
        public static bool IsRightParen(this string s)
        {
            return s == ")";
        }

        /// <summary>
        /// Returns true if a string matches the variable pattern:
        /// consists of a letter or underscore followed by 
        /// zero or more letters, underscores, or digits.
        /// </summary>
        /// <returns></returns>
        public static bool IsVariable(this string s)
        {
            return Regex.IsMatch(s, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*");
        }

        /// <summary>
        /// Returns true if a string is an operator * / + -
        /// </summary>
        /// <returns></returns>
        public static bool IsOperator(this string s)
        {
            return Regex.IsMatch(s, @"[\+\-*/]");
        }
    }


    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}
