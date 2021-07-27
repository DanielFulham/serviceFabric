using System.Runtime.Serialization;

namespace Communication
{
    /// <summary>
    /// Have to make everything serializable to write it to disc. Service fabric will store in memory and disc. 
    /// Will also store across nodes
    /// Make them DataMember to make entire object to eb serializable
    /// </summary>
    [DataContract]
    public class Product
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Type { get; set; }
    }
}
