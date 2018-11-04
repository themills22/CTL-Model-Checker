using System;
using Parser;

namespace Main
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var parser = new Parser.Parser(args[0]);
            var formula = parser.ParseCTLFormula();
            formula.PrintCTLFormula();
            Console.WriteLine();
        }
    }
}