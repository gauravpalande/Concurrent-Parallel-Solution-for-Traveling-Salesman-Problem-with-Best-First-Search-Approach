using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_3
{
    /// <summary>
    /// State Class
    /// </summary>
    [Serializable]
    class State
    {
        // Property to store state name
        internal string StateName;
        // Property to store list of state cities
        internal List<City> Cities = new List<City>();
        // Property to store list of all connections
        internal List<Connection> ConsideredList = new List<Connection>();
        // Property to store connections that need to be included
        internal List<Connection> IncludeList = new List<Connection>();
        // Property to store connections that need to be excluded
        internal List<Connection> ExcludeList = new List<Connection>();
        // Property to store the state's minimum threshold
        internal double MinimumThreshold;
        // Property to store states status
        internal bool Status;
        // Property to store states consideration status
        internal bool Considered;

        /// <summary>
        /// Functionality to create clone of class instance
        /// </summary>
        /// <param name="inputcls"></param>
        /// <returns></returns>
        public State CreateDeepCopy(State inputcls)
        {
            MemoryStream m = new MemoryStream();
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(m, inputcls);
            m.Position = 0;
            return (State)b.Deserialize(m);
        }// End of CreateDeepCopy method
    }// End of State class
}
