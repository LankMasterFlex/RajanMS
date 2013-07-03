using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using RajanMS.Game;

namespace RajanMS
{
    sealed class Database
    {
        MongoClient m_client;
        MongoServer m_server;
        MongoDatabase m_database;

        public Database(string host,string databaseName)
        {
            m_client = new MongoClient(host);
            m_server = m_client.GetServer();
            m_database = m_server.GetDatabase(databaseName);
        }

        public bool NameAvailable(string name)
        {
            var collection = m_database.GetCollection<BsonDocument>("characters");
            var query = new QueryDocument("Name", name);    
            return collection.Find(query).Count() > 0;
        }

        /// <returns>
        /// 0 - General error?
        /// 1 - Success
        /// 2 - User not found
        /// 3 - Wrong password
        /// 4 - Already logged in
        /// </returns>
        public int Login(MapleClient c,string user, string pass)
        {
            var collection = m_database.GetCollection<Account>("accounts");
            var query = new QueryDocument("Username", user);

            var result = collection.Find(query);

            if (result.Count() == 0) //username not found
                return 2;

            Account acc = result.ElementAt(0);

            if (acc.Password == pass)
            {
                if (acc.LoggedIn)
                    return 4;

                c.Account = acc;
                c.Characters = GetCharacters(acc);

                return 1;
            }
            else
            {
                return 3;
            }
        }

        public List<Character> GetCharacters(Account a)
        {
            var x = new List<Character>();

            var collection = m_database.GetCollection<Character>("characters");
            var query = new QueryDocument("AccountId", a.AccountId);

            var result = collection.Find(query);

            foreach (Character c in result)
                x.Add(c);

            return x;
        }

        public int GetNewCharacterId()
        {
            var collection = m_database.GetCollection<Character>("characters");
            
            int id = 0;

            foreach (Character c in collection.FindAll())
            {
                if (c.CharId > id)
                    id = c.CharId;
            }

            return id + 1;
        }

        public void DeleteCharacter(Character c)
        {
            var query = new QueryDocument("CharId",c.CharId);
            m_database.GetCollection<Character>("characters").Remove(query);
        }

        public void SaveAccount(Account a)
        {
            m_database.GetCollection<Account>("accounts").Save(a);
        }
        public void SaveCharacter(Character c)
        {
            m_database.GetCollection<Character>("characters").Save(c);
        }
    }
}
