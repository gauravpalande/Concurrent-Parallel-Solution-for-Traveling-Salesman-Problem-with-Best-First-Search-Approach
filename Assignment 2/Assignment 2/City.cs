using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Assignment_2
{
    /// <summary>
    /// City class
    /// </summary>
    [Serializable]
    class City
    {
        // Property to store connections of the city
        internal List<Connection> Connections = new List<Connection>();
        // Property for city name
        internal string CityName;

        /// <summary>
        /// Functionality to create clone of class instance
        /// </summary>
        /// <param name="inputcls"></param>
        /// <returns></returns>
        public City CreateDeepCopy(City inputcls)
        {
            MemoryStream m = new MemoryStream();
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(m, inputcls);
            m.Position = 0;
            return (City)b.Deserialize(m);
        }// End of CreateDeepCopy method
    }// End of City class
}
