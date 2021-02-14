using Database;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Bson;


namespace MongoDbConnector
{
    public class User : IUser
    {

        //public string id { get; set; }
        //[BsonElement("userName")]
        [BsonId] public string Username { get; set; }
        public string Password { get; set; }

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}