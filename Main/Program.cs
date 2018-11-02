using System;
using Parser;

namespace Main
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var lexer = new Lexer("hello.txt");
            var token = lexer.GetToken();
            Console.WriteLine(token.TokenType + " - " + token.Lexeme + " - " + token.LineNo);
            token = lexer.GetToken();
            Console.WriteLine(token.TokenType + " - " + token.Lexeme + " - " + token.LineNo);
        }
    }
}