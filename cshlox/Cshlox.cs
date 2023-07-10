namespace cshlox;

public class Cshlox
{
    public static bool HadError = false;

    public static void RunFile(string path)
    {
        try
        {
            string text = File.ReadAllText(Path.GetFullPath(path));
            Run(text);

            if (HadError)
            {
                Environment.Exit(-1);
            }
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
                break;
            }
            HadError = false;
        }
    }

    private static void Run(string source)
    {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();
        
        Parser parser = new Parser(tokens);
        Expression expr = parser.Parse();

        if (HadError) return;

        Console.WriteLine(new AstPrinter().Print(expr));
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

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine(string.Format("[line: {0}] Error{1}: {2}", line, where, message));
    }
}
