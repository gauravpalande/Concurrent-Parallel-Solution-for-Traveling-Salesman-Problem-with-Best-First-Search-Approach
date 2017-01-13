using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Assignment_2
{
    /// <summary>
    /// Connection class
    /// </summary>
    [Serializable]
    internal class Connection
    {
        // Property to store connection name
        public string ConnectionName { get; set; }
        // Property to store connections source city
        public City Source { get; set; }
        // Property to store connections destination city
        public City Destination { get; set; }
        // Property to store connections cost
        public int PathCost { get; set; }
        // Property to store connections status
        public bool Considered { get; set; }

        /// <summary>
        /// Functionality to create clone of class instance
        /// </summary>
        /// <param name="inputcls"></param>
        /// <returns></returns>
        public Connection CreateDeepCopy(Connection inputcls)
        {
            MemoryStream m = new MemoryStream();
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(m, inputcls);
            m.Position = 0;
            return (Connection)b.Deserialize(m);
        }// End of CreateDeepCopy method
    }// End of Connection class
}