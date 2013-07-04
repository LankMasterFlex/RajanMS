using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using RajanMS.Game;

namespace RajanMS.Tools
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
        /// 0 - Success
        /// 4 - Wrong password
        /// 5 - User not found
        /// 7 - Already logged in
        /// </returns>
        public byte Login(MapleClient c,string user, string pass)
        {
            var collection = m_database.GetCollection<Account>("accounts");
            var query = new QueryDocument("Username", user);

            var result = collection.Find(query);

            if (result.Count() == 0) //username not found
                return 5;

            Account acc = result.ElementAt(0);

            if (acc.Password == pass)
            {
                if (acc.LoggedIn)
                    return 7;

                c.Account = acc;
                c.Characters = GetCharacters(acc);

                return 0;
            }
            else
            {
                return 4;
            }
        }

        public Character GetCharacter(int id)
        {
            var collection = m_database.GetCollection<Character>("characters");
            var query = new QueryDocument("CharId", id);
            return collection.Find(query).ElementAtOrDefault(0);
        }

        public Account GetAccount(int id)
        {
            var collection = m_database.GetCollection<Account>("accounts");
            var query = new QueryDocument("AccountId", id);
            return collection.Find(query).ElementAtOrDefault(0);
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
            var collection = m_database.GetCollection<Account>("accounts");

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

        public void SetAllOffline()
        {
            var lol = m_database.GetCollection<Account>("accounts");

            foreach (Account acc in lol.FindAll())
            {
                acc.LoggedIn = false;
            }
        }
    }
}
