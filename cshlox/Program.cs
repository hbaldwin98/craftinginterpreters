// read from args

using cshlox;

if (args.Length > 1)
{
    Console.WriteLine("Usage:schlox [script]");
    Environment.Exit(64);
}
else if (args.Length == 1)
{
    Cshlox.RunFile(args[0]);
}
else
{
    Cshlox.RunPrompt();
}

