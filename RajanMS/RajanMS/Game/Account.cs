using MongoDB.Bson;

namespace RajanMS.Game
{
    public sealed class Account
    {
        public ObjectId Id { get; set; }
        public int AccountId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public byte Gender { get; set; }
        public bool GM { get; set; }
        public bool Banned { get; set; }
    }
}
