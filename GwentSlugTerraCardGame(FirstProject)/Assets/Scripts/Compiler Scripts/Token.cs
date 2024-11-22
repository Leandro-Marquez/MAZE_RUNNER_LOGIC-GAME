public class Token
{
    public string TokenType {get; set;}
    public string TokenValue {get; set;}
    public Token(string TipoDeToken , string PropioToken)
    {
        this.TokenType = TipoDeToken;
        this.TokenValue = PropioToken;
    }
    public override string ToString()
    {
        return $"{TokenType} :  {TokenValue}";
    }
}