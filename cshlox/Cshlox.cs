namespace cshlox;

public class Cshlox
{
    private static Interpreter _interpreter = new Interpreter();
    public static bool HadError = false;
    public static bool HadRuntimeError = false;
    public static List<string> CommandHistory = new List<string>();
    public static int CommandHistoryIndex = -1;

    public static void RunFile(string path)
    {
        try
        {
            string text = File.ReadAllText(Path.GetFullPath(path));
            Run(text);

            if (HadError) { Environment.Exit(65); }
            if (HadRuntimeError) { Environment.Exit(70); }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private static List<string> Symbols = new List<string>
    {
        "var",
        "fun",
        "print",
        "if",
        "else",
        "for",
        "while",
        "return",
        "class",
        "this",
        "super",
        "true",
        "false",
        "nil",
        "or",
        "and",
        "not",
        "break",
        "continue",
    };

    public static void RunPrompt()
    {
        while (true)
        {
            try
            {
                Console.Write("cshlox > ");
                string line = ReadLineWithHistory();

                if (string.IsNullOrEmpty(line))
                {
                    break;
                }

                CommandHistory.Add(line);
                CommandHistoryIndex = CommandHistory.Count;
                Run(line);
            }
            catch (Exception)
            {
                continue;
                // break;
            }
            HadError = false;
        }
    }



    private static string ReadLineWithHistory()
    {
        ConsoleKeyInfo keyInfo;
        var inputBuffer = new List<char>();
        int currentPos = 0;
        var suggestion = "";

        void RefreshLine()
        {
            ClearCurrentLine();
            Console.Write($"cshlox > {string.Join("", inputBuffer.ToArray())}");
        }

        while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Enter)
        {
            if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                if (CommandHistoryIndex > 0)
                {
                    CommandHistoryIndex--;
                    inputBuffer.Clear();
                    inputBuffer.AddRange(CommandHistory[CommandHistoryIndex]);
                    RefreshLine();
                    currentPos = inputBuffer.Count;
                }
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                if (CommandHistoryIndex < CommandHistory.Count - 1)
                {
                    CommandHistoryIndex++;
                    inputBuffer.Clear();
                    inputBuffer.AddRange(CommandHistory[CommandHistoryIndex]);
                    RefreshLine();
                    currentPos = inputBuffer.Count;
                }
                else
                {
                    CommandHistoryIndex = CommandHistory.Count;
                    inputBuffer.Clear();
                    RefreshLine();
                    currentPos = inputBuffer.Count;
                }
            }
            else if (keyInfo.Key == ConsoleKey.LeftArrow)
            {
                if (currentPos > 0)
                {
                    currentPos--;
                }
            }
            else if (keyInfo.Key == ConsoleKey.RightArrow)
            {
                if (currentPos < inputBuffer.Count)
                {
                    currentPos++;
                }
            }
            else if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (currentPos > 0)
                {
                    currentPos--;
                    inputBuffer.RemoveAt(currentPos);
                    RefreshLine();
                }
            }
            else if (keyInfo.Key == ConsoleKey.Delete)
            {
                if (currentPos < inputBuffer.Count)
                {
                    inputBuffer.RemoveAt(currentPos);
                    RefreshLine();
                }
            }
            else if (keyInfo.Key == ConsoleKey.Tab)
            {
                if (!string.IsNullOrEmpty(suggestion))
                {
                    inputBuffer.AddRange(suggestion.ToCharArray());
                    currentPos = inputBuffer.Count;
                    RefreshLine();
                }
            }
            else
            {
                inputBuffer.Insert(currentPos, keyInfo.KeyChar);
                currentPos++;
                RefreshLine();
            }

            suggestion = GetSuggestion(string.Join("", inputBuffer.ToArray()));
            if (!string.IsNullOrEmpty(suggestion))
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(suggestion);
                Console.ResetColor();
            }
            Console.CursorLeft = currentPos + 9;
        }

        Console.WriteLine();
        return new string(inputBuffer.ToArray());
    }

    private static string GetSuggestion(string input)
    {
        string suggestion = "";
        string[] inputArray = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (inputArray.Length >= 1)
        {
            // check if the last entry is a symbol
            foreach (var symbol in Symbols)
            {
                if (symbol.StartsWith(inputArray[inputArray.Length - 1]) && symbol.Length > inputArray[inputArray.Length - 1].Length)
                {
                    suggestion += symbol.Substring(inputArray[inputArray.Length - 1].Length);
                    break;
                }
            }
        }


        return suggestion;
    }

    private static void ClearCurrentLine()
    {
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth - 1));
        Console.SetCursorPosition(0, Console.CursorTop);
    }

    private static void Run(string source)
    {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();

        Parser parser = new Parser(tokens);
        List<Stmt> statements = parser.Parse();

        if (HadError) return;

        _interpreter.Interpret(statements);
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    public static void Error(Token token, string message)
    {
        if (token.Type == TokenType.EOF)
        {
            Report(token.Line, " at end", message);
        }
        else
        {
            Report(token.Line, " at '" + token.Lexeme + "'", message);
        }
    }

    public static void RuntimeError(RuntimeError error)
    {
        Console.Error.WriteLine(error.Message + "\n[line " + error.Token.Line + "]");

        HadRuntimeError = true;
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine(string.Format("[line: {0}] Error{1}: {2}", line, where, message));
    }
}
