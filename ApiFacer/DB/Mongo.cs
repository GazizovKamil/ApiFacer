using ApiFacer.Classes;
using DlibDotNet;
using MongoDB.Driver;
//using MathNet.Numerics.LinearAlgebra;

namespace ApiFacer.DB
{
    public class Mongo
    {
        public static void AddToDB(PeopleDescriptor people)
        {
            var client = new MongoClient();
            var database = client.GetDatabase("Facer");
            var collection = database.GetCollection<PeopleDescriptor>("PeopleDescriptor");
            collection.InsertOne(people);
        }

        public static PeopleDescriptor GetDescript(int id)
        {
            var client = new MongoClient();
            var database = client.GetDatabase("Facer");
            var collection = database.GetCollection<PeopleDescriptor>("PeopleDescriptor");
            var one = collection.Find(x => x.people_id == id).FirstOrDefault();

            return one;
        }
    }
}
