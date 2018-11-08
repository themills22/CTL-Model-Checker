using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ModelChecker
{
    public class Automata
    {
        public int InitialState { get; set; }
        public List<int> States { get; set; }
        public Dictionary<int, int> StatesDict { get; set; }
        public List<List<int>> Transitions { get; set; }
        public List<List<int>> TransitionsReversed { get; set; }
        public List<string> Propositions { get; set; }
        public Dictionary<string, List<bool>> StatePropositions { get; set; }

        private int AddState(int q)
        {
            if (StatesDict.ContainsKey(q))
            {
                return StatesDict[q];
            }

            StatesDict[q] = States.Count;
            States.Add(q);
            Transitions.Add(new List<int>());
            TransitionsReversed.Add(new List<int>());
            return StatesDict[q];
        }

        public Automata(string fileName)
        {
            States = new List<int>();
            StatesDict = new Dictionary<int, int>();
            Transitions = new List<List<int>>();
            TransitionsReversed = new List<List<int>>();
            Propositions = new List<string>();
            StatePropositions = new Dictionary<string, List<bool>>();

            var stream = new StreamReader(fileName);
            stream.ReadLine();
            var str = stream.ReadLine();
            while (str != null && !string.Equals(str, "TRANSITIONS"))
            {
                Propositions.Add(str);
                StatePropositions[str] = new List<bool>();
                str = stream.ReadLine();
            }

            var q = int.Parse(stream.ReadLine());
            AddState(q);
            InitialState = 0;

            str = stream.ReadLine();
            while (str != null && !string.Equals(str, "STATE PROPOSITIONS"))
            {
                var strings = str.Split(null);
                q = int.Parse(strings[0]);
                var qPrime = int.Parse(strings[1]);
                q = AddState(q);
                qPrime = AddState(qPrime);
                Transitions[q].Add(qPrime);
                TransitionsReversed[qPrime].Add(q);
                str = stream.ReadLine();
            }

            foreach (var proposition in Propositions)
            {
                for (var i = 0; i < States.Count; i++)
                {
                    StatePropositions[proposition].Add(false);
                }
            }

            Propositions.Add("True");
            StatePropositions["True"] = new List<bool>();
            Propositions.Add("False");
            StatePropositions["False"] = new List<bool>();
            for (var i = 0; i < States.Count; i++)
            {
                StatePropositions["True"].Add(true);
                StatePropositions["False"].Add(false);
            }

            while (!stream.EndOfStream)
            {
                str = stream.ReadLine();
                var strings = str.Split(null);
                q = StatesDict[int.Parse(strings[0])];
                for (var i = 1; i < strings.Length; i++)
                {
                    StatePropositions[strings[i]][q] = true;
                }
            }
        }
    }
}