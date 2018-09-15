namespace Parser
{
    public class Token
    {
        public TokenType TokenType { get; set; }
        public string Lexeme { get; set; }
        public int? LineNo { get; set; }

        public Token()
        {
            TokenType = TokenType.Error;
            Lexeme = string.Empty;
            LineNo = null;
        }

        public Token(int lineNo)
        {
            TokenType = TokenType.Error;
            Lexeme = string.Empty;
            LineNo = lineNo;
        }

        public Token(TokenType tokenType)
        {
            TokenType = tokenType;
            Lexeme = string.Empty;
            LineNo = null;
        }
        
        public Token(TokenType tokenType, int lineNo)
        {
            TokenType = tokenType;
            Lexeme = string.Empty;
            LineNo = lineNo;
        }

        public Token(TokenType tokenType, string lexeme)
        {
            TokenType = tokenType;
            Lexeme = lexeme;
            LineNo = null;
        }
        
        public Token(TokenType tokenType, string lexeme, int lineNo)
        {
            TokenType = tokenType;
            Lexeme = lexeme;
            LineNo = lineNo;
        }
    }
}