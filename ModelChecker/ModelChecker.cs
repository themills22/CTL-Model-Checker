using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        private Automata _automata;
        private CTLFormula _formula;

        public ModelChecker()
        {
            
        }

        public ModelChecker(Automata automata, CTLFormula formula)
        {
            _automata = automata;
            _formula = MakeValid(formula);
        }

        private CTLFormula NegateFormula(CTLFormula formula)
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

        private List<bool> NegateBoolList(List<bool> toNegate)
        {
            for (var i = 0; i < toNegate.Count; i++)
            {
                toNegate[i] = !toNegate[i];
            }

            return toNegate;
        }

        private bool isStringListSubset(List<string> subsetList, List<string> supersetList)
        {
            supersetList.Sort();
            foreach (var element in subsetList)
            {
                if (supersetList.BinarySearch(element) < 0)
                {
                    return false;
                }
            }

            return true;
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
                    formula.LeftFormula = NegateFormula(formula.LeftFormula);
                    formula.RightFormula = NegateFormula(formula.RightFormula);
                    formula.Type = CTLExpressionType.And;
                    return new CTLFormula(CTLExpressionType.Not, formula);
                case CTLExpressionType.Implies:
                    formula.LeftFormula = NegateFormula(formula.LeftFormula);
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
                    formula.LeftFormula = NegateFormula(formula.LeftFormula);
                    formula.Type = CTLExpressionType.EX;
                    return new CTLFormula(CTLExpressionType.Not, formula);
                case CTLExpressionType.AF:
                    formula.RightFormula = formula.LeftFormula;
                    formula.LeftFormula = new CTLFormula(CTLExpressionType.True);
                    formula.Type = CTLExpressionType.AU;
                    return formula;
                case CTLExpressionType.AG:
                    formula.LeftFormula = NegateFormula(formula.LeftFormula);
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
                    formula.LeftFormula = NegateFormula(formula.LeftFormula);
                    formula.RightFormula = formula.LeftFormula;
                    formula.LeftFormula = new CTLFormula(CTLExpressionType.True);
                    formula.Type = CTLExpressionType.AU;
                    return new CTLFormula(CTLExpressionType.Not, formula);
            }
            
            return new CTLFormula();
        }

        private List<bool> Marking(CTLFormula formula)
        {
            List<bool> toReturn;
            List<bool> check1;
            List<bool> check2;
            List<int> toProcessStates;
            int q;

            switch (formula.Type)
            {
                case CTLExpressionType.Prop:
                case CTLExpressionType.True:
                case CTLExpressionType.False:
                    return _automata.StatePropositions[formula.Lexeme];
                case CTLExpressionType.Not:
                    return NegateBoolList(Marking(formula.LeftFormula));
                case CTLExpressionType.And:
                    check1 = Marking(formula.LeftFormula);
                    check2 = Marking(formula.RightFormula);
                    
                    toReturn = new List<bool>();
                    for (var i = 0; i < check1.Count; i++)
                    {
                        toReturn.Add(check1[i] && check2[i]);
                    }

                    return toReturn;
                case CTLExpressionType.EX:
                    check1 = Marking(formula.LeftFormula);
                    
                    toReturn = new List<bool>();
                    for (var i = 0; i < check1.Count; i++)
                    {
                        toReturn.Add(false);
                    }

                    for (var i = 0; i < _automata.Transitions.Count; i++)
                    {
                        var transition = _automata.Transitions[i];
                        foreach (var successor in transition)
                        {
                            if (check1[successor])
                            {
                                toReturn[i] = true;
                            }
                        }
                    }

                    return toReturn;
                case CTLExpressionType.EU:
                    check1 = Marking(formula.LeftFormula);
                    check2 = Marking(formula.RightFormula);

                    var seenBefore = new List<bool>();
                    toReturn = new List<bool>();
                    for (var i = 0; i < check1.Count; i++)
                    {
                        toReturn.Add(false);
                        seenBefore.Add(false);
                    }
                    
                    toProcessStates = new List<int>();
                    for (var i = 0; i < check2.Count; i++)
                    {
                        if (check2[i])
                        {
                            toProcessStates.Add(i);
                        }
                    }

                    while (!toProcessStates.Any())
                    {
                        q = toProcessStates[0];
                        toProcessStates.RemoveAt(0);
                        toReturn[q] = true;
                        foreach (var predecessor in _automata.TransitionsReversed[q])
                        {
                            if (!seenBefore[predecessor])
                            {
                                seenBefore[predecessor] = true;
                                if (check1[predecessor] && !toReturn[predecessor])
                                {
                                    toProcessStates.Add(predecessor);
                                }
                            }
                        }
                    }

                    return toReturn;
                case CTLExpressionType.AU:
                    check1 = Marking(formula.LeftFormula);
                    check2 = Marking(formula.RightFormula);
                    
                    var nb = new List<int>();
                    toProcessStates = new List<int>();
                    toReturn = new List<bool>();
                    for (var i = 0; i < check1.Count; i++)
                    {
                        toReturn.Add(false);
                        nb.Add(_automata.Transitions[i].Count);
                        if (check2[i])
                        {
                            toProcessStates.Add(i);
                        }
                    }

                    while (!toProcessStates.Any())
                    {
                        q = toProcessStates[0];
                        toProcessStates.RemoveAt(0);
                        toReturn[q] = true;
                        foreach (var predecessor in _automata.TransitionsReversed[q])
                        {
                            nb[predecessor]--;
                            if (nb[predecessor] == 0 && check1[predecessor] && !toReturn[predecessor])
                            {
                                toProcessStates.Add(predecessor);
                            }
                        }
                    }

                    return toReturn;
            }
            
            return new List<bool>();
        }
    }
}