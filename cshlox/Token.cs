namespace cshlox;

public class Token
{
    TokenType Type { get; }
    string Lexeme { get; }
    object Literal { get; }
    int Line { get; }
    
    public Token(TokenType type, string lexeme, object literal, int line) 
    {
        Type = type;
        Lexeme = lexeme; 
        Literal = literal;
        Line = line;
    }

    public override string ToString()
    {
        return string.Format("{0} {1} {2}", Type, Lexeme, Literal);
    }
}
