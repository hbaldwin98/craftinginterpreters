namespace cshlox;

public class Cshlox
{
    private static Interpreter _interpreter = new Interpreter();
    public static bool HadError = false;
    public static bool HadRuntimeError = false;

    public static void RunFile(string path)
    {
        try
        {
            string text = File.ReadAllText(Path.GetFullPath(path));
            Run(text);

            if (HadError) { Environment.Exit(65); }
            if (HadRuntimeError) { Environment .Exit(70); }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static void RunPrompt()
    {
        while (true)
        {
            try
            {
                Console.Write("> ");
                string line = Console.ReadLine();

                if (string.IsNullOrEmpty(line))
                {
                    break;
                }
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
