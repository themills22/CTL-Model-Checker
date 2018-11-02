using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Parser
{
    public class Lexer
    {
        private readonly List<Token> _tokens;
        private int LineNo;
        private readonly StreamReader _stream;

        private readonly string _fileLocation =
            @"C:\Users\vinny\Desktop\ASU\senior_year\FSE100\Honors Contract\CTL-Model-Checker\CTL Files\";

        private readonly Regex _propositionSymbols = new Regex(@"[a-z0-9_]");
        private readonly Regex _existentialSymbols = new Regex(@"[XFG]");

        public Lexer(string fileName)
        {
            _tokens = new List<Token>();
            LineNo = 1;
            _stream = new StreamReader(_fileLocation + fileName);
        }

        public TokenType UngetToken(Token token)
        {
            _tokens.Add(token);
            return token.TokenType;
        }

        private bool IsNextEof()
        {
            return _stream.Peek() == -1;
        }

        private bool IsNextEofOrWhitespace()
        {
            var read = _stream.Peek();
            return read == -1 || char.IsWhiteSpace((char) read);
        }

        private bool SkipSpace()
        {
            var spaceEncountered = false;
            var nextChar = _stream.Peek();
            
            while (nextChar != -1 && char.IsWhiteSpace((char) nextChar))
            {
                spaceEncountered = true;
                nextChar = _stream.Read();
                LineNo += nextChar == '\n' ? 1 : 0;
                nextChar = _stream.Peek();
            }

            return spaceEncountered;
        }

        private Token ScanProposition(char firstChar)
        {
            var toReturn = new Token(TokenType.Prop, firstChar.ToString(), LineNo);
            string read;
            
            while (!IsNextEofOrWhitespace() && _propositionSymbols.IsMatch(read = ((char) _stream.Read()).ToString()))
            {
                toReturn.Lexeme += read;
            }

            return toReturn;
        }
        
        public Token GetToken()
        {
            Token toReturn;
            
            if (_tokens.Any())
            {
                toReturn = _tokens[_tokens.Count - 1];
                _tokens.RemoveAt(_tokens.Count - 1);
                return toReturn;
            }

            SkipSpace();
            
            toReturn = new Token(LineNo);

            if (IsNextEof())
            {
                toReturn.TokenType = TokenType.EOF;
                return toReturn;
            }

            var read = (char) _stream.Read();
            switch (read)
            {
                case '(':
                    toReturn.Lexeme = "(";
                    toReturn.TokenType = TokenType.LParen;
                    break;
                case ')':
                    toReturn.Lexeme = ")";
                    toReturn.TokenType = TokenType.RParen;
                    break;
                case '[':
                    toReturn.Lexeme = "[";
                    toReturn.TokenType = TokenType.LBrack;
                    break;
                case ']':
                    toReturn.Lexeme = "]";
                    toReturn.TokenType = TokenType.RBrack;
                    break;
                case '~':
                    toReturn.Lexeme = "~";
                    toReturn.TokenType = TokenType.Not;
                    break;
                case '&':
                    if (!IsNextEofOrWhitespace() && _stream.Read() == '&')
                    {
                        toReturn.Lexeme = "&&";
                        toReturn.TokenType = TokenType.And;
                    }
                    else
                    {
                        toReturn.TokenType = TokenType.Error;
                    }

                    break;
                case '|':
                    if (!IsNextEofOrWhitespace() && _stream.Read() == '|')
                    {
                        toReturn.Lexeme = "||";
                        toReturn.TokenType = TokenType.Or;
                    }
                    else
                    {
                        toReturn.TokenType = TokenType.Error;
                    }

                    break;
                case '=':
                    if (!IsNextEofOrWhitespace() && _stream.Read() == '>')
                    {
                        toReturn.Lexeme = "=>";
                        toReturn.TokenType = TokenType.Implies;
                    }
                    else
                    {
                        toReturn.TokenType = TokenType.Error;
                    }

                    break;
                case '<':
                    var verify = new char[2];
                    if (_stream.Read(verify, 0, 2) == 2 && verify.SequenceEqual(new[] {'=', '>'}))
                    {
                        toReturn.Lexeme = "<=>";
                        toReturn.TokenType = TokenType.IFF;
                    }
                    else
                    {
                        toReturn.TokenType = TokenType.Error;
                    }

                    break;
                case 'T':
                    verify = new char[3];
                    if (_stream.Read(verify, 0, 3) == 3 && verify.SequenceEqual(new[] {'r', 'u', 'e'}))
                    {
                        toReturn.Lexeme = "True";
                        toReturn.TokenType = TokenType.True;
                    }
                    else
                    {
                        toReturn.TokenType = TokenType.Error;
                    }

                    break;
                case 'F':
                    verify = new char[4];
                    if (_stream.Read(verify, 0, 4) == 4 && verify.SequenceEqual(new[] {'a', 'l', 's', 'e'}))
                    {
                        toReturn.Lexeme = "False";
                        toReturn.TokenType = TokenType.False;
                    }
                    else
                    {
                        toReturn.TokenType = TokenType.Error;
                    }

                    break;
                case 'A':
                    if (!IsNextEofOrWhitespace() &&
                        _existentialSymbols.IsMatch((read = (char) _stream.Peek()).ToString()))
                    {
                        switch (read)
                        {
                            case 'X':
                                toReturn.Lexeme = "AX";
                                toReturn.TokenType = TokenType.AX;
                                break;
                            case 'F':
                                toReturn.Lexeme = "AF";
                                toReturn.TokenType = TokenType.AF;
                                break;
                            case 'G':
                                toReturn.Lexeme = "AG";
                                toReturn.TokenType = TokenType.AG;
                                break;
                        }

                        _stream.Read();
                    }
                    else
                    {
                        toReturn.Lexeme = "A";
                        toReturn.TokenType = TokenType.A;
                    }

                    break;
                case 'E':
                    if (!IsNextEofOrWhitespace() &&
                        _existentialSymbols.IsMatch((read = (char) _stream.Peek()).ToString()))
                    {
                        switch (read)
                        {
                            case 'X':
                                toReturn.Lexeme = "EX";
                                toReturn.TokenType = TokenType.EX;
                                break;
                            case 'F':
                                toReturn.Lexeme = "EF";
                                toReturn.TokenType = TokenType.EF;
                                break;
                            case 'G':
                                toReturn.Lexeme = "EG";
                                toReturn.TokenType = TokenType.EG;
                                break;
                        }

                        _stream.Read();
                    }
                    else
                    {
                        toReturn.Lexeme = "E";
                        toReturn.TokenType = TokenType.E;
                    }

                    break;
                case 'U':
                    toReturn.Lexeme = "U";
                    toReturn.TokenType = TokenType.U;
                    break;
                default:
                    if (char.IsLower(read))
                    {
                        return ScanProposition(read);
                    }
                    else
                    {
                        toReturn.TokenType = TokenType.Error;
                    }

                    break;
            }

            return toReturn;
        }

        public Token Peek()
        {
            var token = GetToken();
            UngetToken(token);
            return token;
        }
    }
}