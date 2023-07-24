namespace cshlox;

public class Env
{
    private Dictionary<string, object> _values = new Dictionary<string, object>();

    public void Define(string name, object value)
    {
        _values[name] = value;
    }

    public object Get(Token name)
    {
        if (_values.TryGetValue(name.Lexeme, out var value))
        {
            return value;
        }

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }
}
