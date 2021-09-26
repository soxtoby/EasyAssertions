using System;
using System.Collections.Generic;

namespace EasyAssertions
{
    static class CodeFinding
    {
        public static List<string> SplitCode(this ReadOnlySpan<char> remainingArgSource, string delimiter)
        {
            var arguments = new List<string>();
            var delimiterSpan = delimiter.AsSpan();

            while (!remainingArgSource.IsEmpty)
            {
                var nextDelimiter = remainingArgSource.FindCode(delimiterSpan);
                var arg = nextDelimiter >= 0 ? remainingArgSource[..nextDelimiter] : remainingArgSource;
                remainingArgSource = nextDelimiter >= 0 ? remainingArgSource[(nextDelimiter + delimiter.Length)..] : Array.Empty<char>();
                arguments.Add(arg.ToString().Trim());
            }

            return arguments;
        }

        /// <summary>
        /// Finds the given code, skipping over strings and brace pairs.
        /// </summary>
        public static int FindCode(this ReadOnlySpan<char> source, string codeToFind, int startIndex = 0)
        {
            return source.FindCode(codeToFind.AsSpan(), startIndex);
        }

        static int FindCode(this ReadOnlySpan<char> source, ReadOnlySpan<char> codeToFind, int startIndex = 0)
        {
            var context = new Stack<Context>(new[] { Context.None });

            for (var i = startIndex; i < source.Length; i++)
            {
                if (context.Peek() == Context.None && source[i..].StartsWith(codeToFind))
                {
                    return i;
                }
                else if (context.Peek().HasFlag(Context.String))
                {
                    if (context.Peek().HasFlag(Context.VerbatimString))
                    {
                        if (source[i..].StartsWith("\"\"".AsSpan()))
                            i++;
                        else if (source[i..].StartsWith("\\\\".AsSpan()))
                            i++;
                        else if (source[i] == '"')
                            context.Pop();
                    }
                    else
                    {
                        if (context.Peek().HasFlag(Context.InterpolatedString))
                        {
                            if (source[i..].StartsWith("{{".AsSpan()))
                                i++;
                            else if (source[i] == '{')
                                context.Push(Context.Braces);
                        }

                        if (source[i..].StartsWith("\\\"".AsSpan()))
                            i++;
                        else if (source[i] == '"')
                            context.Pop();
                    }
                }
                else
                {
                    if (context.Peek() == Context.Parens && source[i] == ')'
                        || context.Peek() == Context.Braces && source[i] == '}')
                    {
                        context.Pop();
                    }
                    else if (source[i] == '(')
                    {
                        context.Push(Context.Parens);
                    }
                    else if (source[i] == '{')
                    {
                        context.Push(Context.Braces);
                    }
                    else if (source[i..].StartsWith("$@\"".AsSpan()) || source[i..].StartsWith("@$\"".AsSpan()))
                    {
                        i += 2;
                        context.Push(Context.String | Context.VerbatimString | Context.InterpolatedString);
                    }
                    else if (source[i..].StartsWith("$\"".AsSpan()))
                    {
                        i++;
                        context.Push(Context.String | Context.InterpolatedString);
                    }
                    else if (source[i..].StartsWith("@\"".AsSpan()))
                    {
                        i++;
                        context.Push(Context.String | Context.VerbatimString);
                    }
                    else if (source[i] == '"')
                    {
                        context.Push(Context.String);
                    }
                }
            }

            return -1;
        }

        [Flags]
        enum Context
        {
            None = 0,
            Parens = 1,
            Braces = 2,
            String = 4,
            VerbatimString = 8,
            InterpolatedString = 16
        }
    }
}