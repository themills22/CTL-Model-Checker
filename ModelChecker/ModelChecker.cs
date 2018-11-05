using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Windows.Markup;
using Parser;

namespace ModelChecker
{
    public class ModelChecker
    {
        private readonly List<CTLExpressionType> _validExpressionTypes = 
            new List<CTLExpressionType>(new CTLExpressionType[]
            {
                CTLExpressionType.True,
                CTLExpressionType.False,
                CTLExpressionType.Prop,
                CTLExpressionType.Not,
                CTLExpressionType.And,
                CTLExpressionType.EX,
                CTLExpressionType.EU,
                CTLExpressionType.AU
            });

        public ModelChecker()
        {
            
        }

        private CTLFormula Negate(CTLFormula formula)
        {
            switch (formula.Type)
            {
                case CTLExpressionType.Not:
                    return formula.LeftFormula;
                case CTLExpressionType.True:
                    formula.Type = CTLExpressionType.False;
                    return formula;
                case CTLExpressionType.False:
                    formula.Type = CTLExpressionType.True;
                    return formula;
                default:
                    return new CTLFormula(CTLExpressionType.Not, formula);
            }
        }

        public CTLFormula MakeValid(CTLFormula formula)
        {
            if (formula.LeftFormula != null)
            {
                formula.LeftFormula = MakeValid(formula.LeftFormula);
            }
            
            if (formula.RightFormula != null)
            {
                formula.RightFormula = MakeValid(formula.RightFormula);
            }
            
            if (_validExpressionTypes.Contains(formula.Type))
            {
                return formula;
            }


            switch (formula.Type)
            {
                case CTLExpressionType.Or:
                    formula.LeftFormula = Negate(formula.LeftFormula);
                    formula.RightFormula = Negate(formula.RightFormula);
                    formula.Type = CTLExpressionType.And;
                    return new CTLFormula(CTLExpressionType.Not, formula);
                case CTLExpressionType.Implies:
                    formula.LeftFormula = Negate(formula.LeftFormula);
                    formula.Type = CTLExpressionType.Or;
                    return MakeValid(formula);
                case CTLExpressionType.IFF:
                    var newLeftFormula =
                        new CTLFormula(CTLExpressionType.Implies, formula.LeftFormula, formula.RightFormula);
                    var newRightFormula =
                        new CTLFormula(CTLExpressionType.Implies, formula.RightFormula, formula.LeftFormula);
                    newLeftFormula = MakeValid(newLeftFormula);
                    newRightFormula = MakeValid(newRightFormula);
                    formula.LeftFormula = newLeftFormula;
                    formula.RightFormula = newRightFormula;
                    formula.Type = CTLExpressionType.And;
                    return formula;
                case CTLExpressionType.AX:
                    formula.LeftFormula = Negate(formula.LeftFormula);
                    formula.Type = CTLExpressionType.EX;
                    return new CTLFormula(CTLExpressionType.Not, formula);
                case CTLExpressionType.AF:
                    formula.RightFormula = formula.LeftFormula;
                    formula.LeftFormula = new CTLFormula(CTLExpressionType.True);
                    formula.Type = CTLExpressionType.AU;
                    return formula;
                case CTLExpressionType.AG:
                    formula.LeftFormula = Negate(formula.LeftFormula);
                    formula.RightFormula = formula.LeftFormula;
                    formula.LeftFormula = new CTLFormula(CTLExpressionType.True);
                    formula.Type = CTLExpressionType.EU;
                    return new CTLFormula(CTLExpressionType.Not, formula);
                case CTLExpressionType.EF:
                    formula.RightFormula = formula.LeftFormula;
                    formula.LeftFormula = new CTLFormula(CTLExpressionType.True);
                    formula.Type = CTLExpressionType.EU;
                    return formula;
                case CTLExpressionType.EG:
                    formula.LeftFormula = Negate(formula.LeftFormula);
                    formula.RightFormula = formula.LeftFormula;
                    formula.LeftFormula = new CTLFormula(CTLExpressionType.True);
                    formula.Type = CTLExpressionType.AU;
                    return new CTLFormula(CTLExpressionType.Not, formula);
            }
            
            return new CTLFormula();
        }
    }
}