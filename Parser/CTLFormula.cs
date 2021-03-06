using System;

namespace Parser
{
    public class CTLFormula
    {
        public CTLExpressionType Type;
        public CTLFormula LeftFormula;
        public CTLFormula RightFormula;
        public string Lexeme;

        public CTLFormula()
        {
            
        }

        public CTLFormula(CTLExpressionType type)
        {
            Type = type;
        }

        public CTLFormula(CTLExpressionType type, string lexeme)
        {
            Type = type;
            Lexeme = lexeme;
        }

        public CTLFormula(CTLExpressionType type, CTLFormula left)
        {
            Type = type;
            LeftFormula = left;
        }

        public CTLFormula(CTLExpressionType type, CTLFormula left, CTLFormula right)
        {
            Type = type;
            LeftFormula = left;
            RightFormula = right;
        }

        private string GetTwoSidedOperatorString()
        {
            switch (Type)
            {
                case CTLExpressionType.And:
                    return "&&";
                case CTLExpressionType.Or:
                    return "||";
                case CTLExpressionType.Implies:
                    return "=>";
                case CTLExpressionType.IFF:
                    return "<=>";
            }

            return string.Empty;
        }

        public void PrintCTLFormula()
        {
            switch (Type)
            {
                case CTLExpressionType.True:
                    Console.Write("True");
                    break;
                case CTLExpressionType.False:
                    Console.Write("False");
                    break;
                case CTLExpressionType.Prop:
                    Console.Write(Lexeme);
                    break;
                case CTLExpressionType.Not:
                case CTLExpressionType.And:
                case CTLExpressionType.Or:
                case CTLExpressionType.Implies:
                case CTLExpressionType.IFF:
                    Console.Write("( ");
                    if (Type == CTLExpressionType.Not)
                    {
                        Console.Write("~ ");
                        LeftFormula.PrintCTLFormula();
                    }
                    else
                    {
                        LeftFormula.PrintCTLFormula();
                        Console.Write(" " + GetTwoSidedOperatorString() + " ");
                        RightFormula.PrintCTLFormula();
                    }
                    
                    Console.Write(" )");
                    break;
                case CTLExpressionType.AX:
                    Console.Write("AX ");
                    LeftFormula.PrintCTLFormula();
                    break;
                case CTLExpressionType.AF:
                    Console.Write("AF ");
                    LeftFormula.PrintCTLFormula();
                    break;
                case CTLExpressionType.AG:
                    Console.Write("AG ");
                    LeftFormula.PrintCTLFormula();
                    break;
                case CTLExpressionType.EX:
                    Console.Write("EX ");
                    LeftFormula.PrintCTLFormula();
                    break;
                case CTLExpressionType.EF:
                    Console.Write("EF ");
                    LeftFormula.PrintCTLFormula();
                    break;
                case CTLExpressionType.EG:
                    Console.Write("EG ");
                    LeftFormula.PrintCTLFormula();
                    break;
                case CTLExpressionType.AU:
                    Console.Write("A[ ");
                    LeftFormula.PrintCTLFormula();
                    Console.Write(" U ");
                    RightFormula.PrintCTLFormula();
                    Console.Write(" ]");
                    break;
                case CTLExpressionType.EU:
                    Console.Write("E[ ");
                    LeftFormula.PrintCTLFormula();
                    Console.Write(" U ");
                    RightFormula.PrintCTLFormula();
                    Console.Write(" ]");
                    break;
            }
        }
    }
}