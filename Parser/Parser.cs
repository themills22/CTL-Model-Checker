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
            _propositionsFound = new List<string>();
        }

        public List<string> GetPropositionsFound()
        {
            return _propositionsFound;
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
            var token = _lexer.GetToken();
            CTLFormula formula;
            switch (token.TokenType)
            {
                case TokenType.True:
                    return new CTLFormula(CTLExpressionType.True, "True");
                case TokenType.False:
                    return new CTLFormula(CTLExpressionType.False, "False");
                case TokenType.Prop:
                {
                    if (!_propositionsFound.Contains(token.Lexeme))
                    {
                        _propositionsFound.Add(token.Lexeme);
                    }
                    return new CTLFormula(CTLExpressionType.Prop, token.Lexeme);
                }
                case TokenType.LParen:
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
                        switch (token.TokenType)
                        {
                            case TokenType.And:
                                formula.Type = CTLExpressionType.And;
                                break;
                            case TokenType.Or:
                                formula.Type = CTLExpressionType.Or;
                                break;
                            case TokenType.Implies:
                                formula.Type = CTLExpressionType.Implies;
                                break;
                            case TokenType.IFF:
                                formula.Type = CTLExpressionType.IFF;
                                break;
                            default:
                                Error(token);
                                break;
                        }

                        formula.RightFormula = ParseCTLFormula();
                        Expect(TokenType.RParen);
                        return formula;
                    }

                    Error(token);
                    break;
                }
                case TokenType.AX:
                {
                    formula = new CTLFormula(CTLExpressionType.AX);
                    formula.LeftFormula = ParseCTLFormula();
                    return formula;
                }
                case TokenType.AF:
                {
                    formula = new CTLFormula(CTLExpressionType.AF);
                    formula.LeftFormula = ParseCTLFormula();
                    return formula;
                }
                case TokenType.AG:
                {
                    formula = new CTLFormula(CTLExpressionType.AG);
                    formula.LeftFormula = ParseCTLFormula();
                    return formula;
                }
                case TokenType.EX:
                {
                    formula = new CTLFormula(CTLExpressionType.EX);
                    formula.LeftFormula = ParseCTLFormula();
                    return formula;
                }
                case TokenType.EF:
                {
                    formula = new CTLFormula(CTLExpressionType.EF);
                    formula.LeftFormula = ParseCTLFormula();
                    return formula;
                }
                case TokenType.EG:
                {
                    formula = new CTLFormula(CTLExpressionType.EG);
                    formula.LeftFormula = ParseCTLFormula();
                    return formula;
                }
                case TokenType.A:
                {
                    Expect(TokenType.LBrack);
                    formula = new CTLFormula(CTLExpressionType.AU);
                    formula.LeftFormula = ParseCTLFormula();
                    Expect(TokenType.U);
                    formula.RightFormula = ParseCTLFormula();
                    Expect(TokenType.RBrack);
                    return formula;
                }
                case TokenType.E:
                {
                    Expect(TokenType.LBrack);
                    formula = new CTLFormula(CTLExpressionType.EU);
                    formula.LeftFormula = ParseCTLFormula();
                    Expect(TokenType.U);
                    formula.RightFormula = ParseCTLFormula();
                    Expect(TokenType.RBrack);
                    return formula;
                }
            }
            
            Error(token);
            return null;
        }
    }
}