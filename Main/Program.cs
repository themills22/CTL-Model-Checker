using System;
using ModelChecker;
using Parser;

namespace Main
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var parser = new Parser.Parser(args[0]);
            var formula = parser.ParseCTLFormula();
            var automata = new Automata(args[1]);

            var mc = new ModelChecker.ModelChecker(automata, formula, parser.GetPropositionsFound());

            Console.WriteLine(mc.Check() ? "Yay" : "Nay");
        }
    }
}