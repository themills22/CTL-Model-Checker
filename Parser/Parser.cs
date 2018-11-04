using System;
using System.Collections.Generic;

namespace Parser
{
    public class Parser
    {
        private List<string> _propositionsFound;
        private readonly List<TokenType> _firstCTLFormula = new List<TokenType>(new TokenType[]
        {
            TokenType.True,
            TokenType.False,
            TokenType.Prop,
            TokenType.LParen,
            TokenType.AX,
            TokenType.AF,
            TokenType.AG,
            TokenType.A,
            TokenType.EX,
            TokenType.EF,
            TokenType.EG,
            TokenType.E
        });

        private Lexer _lexer;

        public Parser(string fileName)
        {
            _lexer = new Lexer(fileName);
        }

        private void Error(Token errorToken)
        {
            Console.WriteLine("There was an error while parsing this file on line number " + errorToken.LineNo + ".");
            Environment.Exit(1);
        }

        private Token Expect(TokenType tokenType)
        {
            Token token = _lexer.GetToken();
            if (token.TokenType != tokenType)
            {
                Error(token);
            }

            return token;
        }

        public CTLFormula ParseCTLFormula()
        {
            Token token = _lexer.GetToken();
            CTLFormula formula;
            if (token.TokenType == TokenType.True)
            {
                return new CTLFormula(CTLExpressionType.True);
            }

            if (token.TokenType == TokenType.False)
            {
                return new CTLFormula(CTLExpressionType.False);
            }

            if (token.TokenType == TokenType.Prop)
            {
                if (!_propositionsFound.Contains(token.Lexeme))
                {
                    _propositionsFound.Add(token.Lexeme);
                }
                return new CTLFormula(CTLExpressionType.Prop, token.Lexeme);
            }

            if (token.TokenType == TokenType.LParen)
            {
                token = _lexer.Peek();
                if (token.TokenType == TokenType.Not)
                {
                    formula = new CTLFormula(CTLExpressionType.Not);
                    _lexer.GetToken();    // remove NOT token as Peek call did not
                    formula.LeftFormula = ParseCTLFormula();
                    Expect(TokenType.RParen);
                    return formula;
                }

                if (_firstCTLFormula.Contains(token.TokenType))
                {
                    formula = new CTLFormula();
                    formula.LeftFormula = ParseCTLFormula();
                    token = _lexer.GetToken();
                    if (token.TokenType == TokenType.And)
                    {
                        formula.Type = CTLExpressionType.And;
                    }
                    else if (token.TokenType == TokenType.Or)
                    {
                        formula.Type = CTLExpressionType.Or;
                    }
                    else if (token.TokenType == TokenType.Implies)
                    {
                        formula.Type = CTLExpressionType.Implies;
                    }
                    else if (token.TokenType == TokenType.IFF)
                    {
                        formula.Type = CTLExpressionType.IFF;
                    }
                    else
                    {
                        Error(token);
                    }

                    formula.RightFormula = ParseCTLFormula();
                    Expect(TokenType.RParen);
                    return formula;
                }

                Error(token);
            }

            if (token.TokenType == TokenType.AX)
            {
                formula = new CTLFormula(CTLExpressionType.AX);
                formula.LeftFormula = ParseCTLFormula();
                return formula;
            }

            if (token.TokenType == TokenType.AF)
            {
                formula = new CTLFormula(CTLExpressionType.AF);
                formula.LeftFormula = ParseCTLFormula();
                return formula;
            }

            if (token.TokenType == TokenType.AG)
            {
                formula = new CTLFormula(CTLExpressionType.AG);
                formula.LeftFormula = ParseCTLFormula();
                return formula;
            }

            if (token.TokenType == TokenType.EX)
            {
                formula = new CTLFormula(CTLExpressionType.EX);
                formula.LeftFormula = ParseCTLFormula();
                return formula;
            }

            if (token.TokenType == TokenType.EF)
            {
                formula = new CTLFormula(CTLExpressionType.EF);
                formula.LeftFormula = ParseCTLFormula();
                return formula;
            }

            if (token.TokenType == TokenType.EG)
            {
                formula = new CTLFormula(CTLExpressionType.EG);
                formula.LeftFormula = ParseCTLFormula();
                return formula;
            }

            if (token.TokenType == TokenType.A)
            {
                Expect(TokenType.LBrack);
                formula = new CTLFormula(CTLExpressionType.AU);
                formula.LeftFormula = ParseCTLFormula();
                Expect(TokenType.U);
                formula.RightFormula = ParseCTLFormula();
                Expect(TokenType.RBrack);
                return formula;
            }

            if (token.TokenType == TokenType.E)
            {
                Expect(TokenType.LBrack);
                formula = new CTLFormula(CTLExpressionType.EU);
                formula.LeftFormula = ParseCTLFormula();
                Expect(TokenType.U);
                formula.RightFormula = ParseCTLFormula();
                Expect(TokenType.RBrack);
                return formula;
            }
            
            Error(token);
            return null;
        }
    }
}