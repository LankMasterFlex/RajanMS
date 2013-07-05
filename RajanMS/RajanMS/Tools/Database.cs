using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using RajanMS.Game;

namespace RajanMS.Tools
{
    sealed class Database
    {
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
            return m_database.GetCollection<BsonDocument>("characters").FindOne(query) != null;
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

            Account acc = collection.FindOne(query);

            if (acc == null)//username not found
                return 5;

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

        public Account GetAccount(int id)
        {
            var query = new QueryDocument("AccountId", id);
            return m_database.GetCollection<Account>("accounts").FindOne(query);
        }
        public Character GetCharacter(int id)
        {
            var query = new QueryDocument("CharId", id);
            return m_database.GetCollection<Character>("characters").FindOne(query);
        }

        public List<Character> GetCharacters(Account a)
        {
            var query = new QueryDocument("AccountId", a.AccountId);
            return m_database.GetCollection<Character>("characters").Find(query).ToList();
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
            var query = new QueryDocument("LoggedIn", true);
            var update = Update.Set("LoggedIn", false);
            m_database.GetCollection<Account>("accounts").Update(query, update, UpdateFlags.Multi);
        }
    }
}
