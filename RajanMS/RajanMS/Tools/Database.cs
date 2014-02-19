using MongoDB.Bson;
using MongoDB.Driver;
using RajanMS.Game;
using System.Collections.Generic;
using System.Linq;

namespace RajanMS.Tools
{
    public sealed class Database
    {
        public static Database Instance { get; set; }

        public const string Accounts = "accounts";
        public const string Characters = "characters";

        private readonly MongoDatabase m_database;

        public Database(string host,string databaseName)
        {
            var client = new MongoClient(host);
            var server = client.GetServer();
            m_database = server.GetDatabase(databaseName);
        }

        public bool NameAvailable(string name)
        {
            var query = new QueryDocument("Name", name);
            return m_database.GetCollection<BsonDocument>(Characters).FindOne(query) == null;
        }

        /// <returns>
        /// 0 - Success
        /// 4 - Wrong password
        /// 5 - User not found
        /// 7 - Already logged in
        /// </returns>
        public byte Login(MapleClient c,string user, string pass)
        {
            var collection = m_database.GetCollection<Account>(Accounts);
            var query = new QueryDocument("Username", user);

            Account acc = collection.FindOne(query);

            if (acc == null)//username not found
                return 5;

            if (acc.Banned)
                return 2;

            if (acc.Password == pass)
            {
                c.Account = acc;
                c.Characters = GetCharacters(acc);
                return 0;
            }
            else
            {
                return 4;
            }
        }

        public Account GetAccount(int id)
        {
            var query = new QueryDocument("AccountId", id);
            return m_database.GetCollection<Account>(Accounts).FindOne(query);
        }
        public Character GetCharacter(int id)
        {
            var query = new QueryDocument("CharId", id);
            return m_database.GetCollection<Character>(Characters).FindOne(query);
        }

        public List<Character> GetCharacters(Account a)
        {
            var query = new QueryDocument("AccountId", a.AccountId);
            return m_database.GetCollection<Character>(Characters).Find(query).ToList();
        }

        //TODO : Fix these 'Get' methods
        public int GetNewCharacterId()
        {
            var collection = m_database.GetCollection<Character>(Characters);
            
            int id = -1;

            foreach (Character c in collection.FindAll())
            {
                if (c.CharId > id)
                    id = c.CharId;
            }

            if (id == -1) //no characters
                return 100; //base

            return id + 1;
        }
        public int GetNewAccountId()
        {
            var collection = m_database.GetCollection<Account>(Accounts);

            int id = -1;

            foreach (Account c in collection.FindAll())
            {
                if (c.AccountId > id)
                    id = c.AccountId;
            }

            return id + 1;
        }


        public void DeleteCharacter(Character c)
        {
            var query = new QueryDocument("CharId",c.CharId);
            m_database.GetCollection<Character>(Characters).Remove(query);
        }

        public void Delete<T>(string collection,QueryDocument query)
        {
            m_database.GetCollection<T>(collection).Remove(query);
        }
        public void Save<T>(string collection,T value)
        {
            m_database.GetCollection<T>(collection).Save(value);
        }
    }
}
