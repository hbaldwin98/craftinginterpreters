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
            Console.Write("> ");
            string line = Console.ReadLine();

            if (string.IsNullOrEmpty(line))
            {
                break;
            }
            Run(line);

            HadError = false;
        }
    }

    private static void Run(string source)
    {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();

        foreach (Token token in tokens)
        {
            Console.WriteLine(token);
        }
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine(string.Format("[line: {0}] Error{1}: {2}", line, where, message));
    }
}
