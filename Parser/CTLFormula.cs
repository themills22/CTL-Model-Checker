namespace Parser
{
    public class CTLFormula
    {
        public CTLExpressionType Type;
        public CTLFormula LeftFormula;
        public CTLFormula RightFormula;
        public string Lexeme;

        public CTLFormula(CTLExpressionType type)
        {
            Type = type;
        }

        public CTLFormula(CTLExpressionType type, string lexeme)
        {
            Type = type;
            Lexeme = lexeme;
        }
    }
}