using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Eric Longberg
// CS3500
// September 17, 2015
namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// s1 depends on t1 --> t1 must be evaluated before s1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// (Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.)
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        private Dictionary<String, List<String>> dependents;
        private Dictionary<String, List<String>> dependees;
        private int _size;
        private List<String> empty = new List<String>();
        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            // Invariant: If a key has no values (the list is empty), we will delete it.
            dependents = new Dictionary<String, List<String>>();
            dependees = new Dictionary<String, List<String>>();
            _size = 0;
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return _size; }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get
            {
                List<String> vals;
                if (dependees.TryGetValue(s, out vals))
                    return vals.Count;
                return 0;
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            return dependents.ContainsKey(s);
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            return dependees.ContainsKey(s);
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            return HasDependents(s) ? dependents[s].AsReadOnly() : empty.AsReadOnly();
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            return HasDependees(s) ? dependees[s].AsReadOnly() : empty.AsReadOnly();
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   s depends on t
        ///
        /// </summary>
        /// <param name="s"> s cannot be evaluated until t is</param>
        /// <param name="t"> t must be evaluated first.  S depends on T</param>
        public void AddDependency(string s, string t)
        {
            // Reminder: if a key has no values, we have removed it from the dictionary

            // If s isn't already in dictionary
            if (!dependents.ContainsKey(s))
                dependents.Add(s, new List<String>());

            // If s already depends on t (includes t dependee of s)
            if (dependents[s].Contains(t))
                return;

            dependents[s].Add(t);

            if (!dependees.ContainsKey(t))
                dependees.Add(t, new List<String>());

            dependees[t].Add(s);
            _size++;
            return;
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            if (!dependents.ContainsKey(s))
                return;
            dependents[s].Remove(t);
            _size--;
            // If empty, remove
            if (dependents[s].Count == 0)
                dependents.Remove(s);

            if (!dependees.ContainsKey(t))
                return;
            dependees[t].Remove(s);
            // If empty, remove
            if (dependees[t].Count == 0)
                dependees.Remove(t);
        }

        //todo: look over
        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (dependents.ContainsKey(s))
            {
                // need an unchanging backupList, since we are actively changing the list
                // being iterated through
                var backupList = new List<String>(dependents[s]);
                foreach (string r in backupList)
                {
                    RemoveDependency(s,r);
                }
            }

            foreach (string t in newDependents)
            {
                AddDependency(s,t);
            }
        }

        
        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            if (dependees.ContainsKey(s))
            {
                var backupList = new List<String>(dependees[s]);
                foreach (string r in backupList)
                {
                    RemoveDependency(r, s);
                }
            }

            foreach (string t in newDependees)
            {
                AddDependency(t, s);
            }
        }
    }
}