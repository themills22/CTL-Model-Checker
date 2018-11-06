using System;
using ModelChecker;
using Parser;

namespace Main
{
    internal class Program
    {
        public static void Main(string[] args)
        {
//            var parser = new Parser.Parser(args[0]);
//            var formula = parser.ParseCTLFormula();
//            var mc = new ModelChecker.ModelChecker();
//            formula = mc.MakeValid(formula);
//            formula.PrintCTLFormula();
//            Console.WriteLine();

            var automata = new Automata(args[1]);
            Console.WriteLine("Hello World");
        }
    }
}