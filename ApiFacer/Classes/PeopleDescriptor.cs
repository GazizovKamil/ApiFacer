using DlibDotNet;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ApiFacer.Classes
{
    public class PeopleDescriptor
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        ObjectId _id;
        public int people_id { get; set; }  
        public float[] descriptor { get; set; }
    }
}
