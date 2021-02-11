using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkRift;
using DarkRift.Server;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace UDRMS.Shared
{
    //TODO: Changes code to MongoDB
    public class Lobby_Player : IDarkRiftSerializable
    {
        public IClient client;
        

        [BsonId()]
        public ObjectId Id { get; set; }

        [BsonElement("userName")]
        [BsonRequired()]
        public string playerName;


        Lobby_Match currentMatch;
            
        public Lobby_Player(){
            
        }
        public Lobby_Player(IClient client)
        {
            this.client = client;
        }
        public void AssignMatch(Lobby_Match lobby_Match)
        {
            this.currentMatch = lobby_Match;
        }
        public void RemoveAssignedMatch()
        {
            this.currentMatch = null;
        }
        public Lobby_Match getCurrentMatch()
        {
            return currentMatch;
        }
        public void Serialize(SerializeEvent e){
            e.Writer.Write(playerName);
        }
        public void Deserialize(DeserializeEvent e){
            this.playerName = e.Reader.ReadString();
        }

    }
}
