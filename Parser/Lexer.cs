using System;
using System.Collections.Generic;
using System.Linq;

namespace Parser
{
    public class Lexer
    {
        private List<Token> _tokens;
        public int LineNo { get; set; }

        public Lexer()
        {
            _tokens = new List<Token>();
            LineNo = 1;
        }

        public TokenType UngetToken(Token token)
        {
            _tokens.Add(token);
            return token.TokenType;
        }

        public Token GetToken()
        {
            if (_tokens.Any())
            {
                var toReturn = _tokens[_tokens.Count - 1];
                _tokens.RemoveAt(_tokens.Count - 1);
                return toReturn;
            }
            
        }
    }
}