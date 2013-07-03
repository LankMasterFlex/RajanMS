using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Constants
    {
        public static readonly short MajorVersion = 111;
        public static readonly string MinorVersion = "1";
        public static readonly byte Locale = 8;

        public static readonly string ConfigName = "Server.ini";

        public static readonly byte[] RIV = new byte[] { 0x52, 0x61, 0x6A, 0x61 }; //Raja
        public static readonly byte[] SIV = new byte[] { 0x6E, 0x52, 0x30, 0x58 }; //nR0X

        public static readonly string EventMessage = "Welcome to BasedWorld!";

        public static readonly string[] WorldNames = new string[]
        {
            "Scania", "Bera", "Broa",
            "Windia", "Khaini", "Bellocan",
            "Mardia", "Kradia", "Yellonde",
            "Demethos", "Galicia", "El Nido",
            "Zenith", "Arcania", "Chaos",
            "Nova", "Renegades"
        };

        public static readonly int MaxPlayers = 100;

        public static bool isEvan(short job)
        {
            return job == 2001 || (job / 100 == 22);
        }

        public static bool isMercedes(short job)
        {
            return job == 2002 || (job / 100 == 23);
        }

        public static bool isJett(short job)
        {
            return job == 508 || (job / 10 == 57);
        }

        public static bool isPhantom(short job)
        {
            return job == 2003 || (job / 100 == 24);
        }

        public static bool isDemon(short job)
        {
            return job == 3001 || (job >= 3100 && job <= 3112);
        }

        public static bool isAran(short job)
        {
            return job >= 2000 && job <= 2112 && job != 2001 && job != 2002 && job != 2003;
        }

        public static bool isMihile(short job)
        {
            return job == 5000 || (job >= 5100 && job <= 5112);
        }

        public static bool isResist(short job)
        {
            return job / 1000 == 3;
        }

        public static bool isKaiser(short job)
        {
            return job == 6000 || (job >= 6100 && job <= 6112);
        }

        public static bool isWildHunter(short job)
        {
            return job == 3300 || (job >= 3310 && job <= 3312);
        }

        public static bool isDualBlade(short job)
        {
            return job >= 430 && job <= 434;
        }

        public static bool isCannon(short job)
        {
            return job >= 530 && job <= 532;
        }

        public static bool isSeparatedSp(short job)
        {
            return (isEvan(job) || (isResist(job)) || (isMercedes(job)) || (isJett(job)) || (isPhantom(job) || (isMihile(job) || isKaiser(job))));
        }

        public static bool is_extendsp_job(int jobId)
        {
            return jobId / 1000 == 3 || jobId / 100 == 22 || jobId == 2001 || jobId / 100 == 23 || jobId == 2002;
        }

        public enum Job : short
        {
            Beginner = 0,

            Swordman = 100,
            Fighter = 110,
            Crusader = 111,
            Hero = 112,
            Page = 120,
            WhiteKnight = 121,
            Paladin = 122,
            Spearman = 130,
            DragonKnight = 131,
            DarkKnight = 132,

            Magician = 200,
            FirePoisonMagician = 210,
            FirePoisonWizzard = 211,
            FirePoisonArch = 112,
            IceLightningMagician = 220,
            IceLightningWizzard = 221,
            IceLightningArch = 222,
            Cleric = 230,
            Priest = 231,
            Bishop = 232,

            Archer = 300,
            Hunter = 310,
            Ranger = 311,
            Bowmaster = 312,
            Crossbowman = 320,
            Sniper = 321,
            Marksman = 322,

            Rogue = 400,
            Assassin = 410,
            Hermit = 411,
            NightLord = 412,
            Bandit = 420,
            ChiefBandit = 421,
            Shadower = 422,
            BladeRecruit = 430,
            BladeAcolyte = 431,
            BladeSpecialist = 432,
            BladeLord = 433,
            BladeMaster = 434,

            Pirate = 500,
            Brawler = 510,
            Marauder = 511,
            Buccaneer = 512,
            Gunslinger = 520,
            Outlaw = 521,
            Corsair = 522,

            Cannoneer = 530,
            CannonBlaster = 531,
            CannonMaster = 532,

            Manager = 800,

            GM = 900,
            SuperGM = 910,

            Noblesse = 1000,

            DawnWarrior1 = 1100,
            DawnWarrior2 = 1110,
            DawnWarrior3 = 1111,
            DawnWarrior4 = 1112,

            BlazeWizzard1 = 1200,
            BlazeWizzard2 = 1210,
            BlazeWizzard3 = 1211,
            BlazeWizzard4 = 1212,

            WindArcher1 = 1300,
            WindArcher2 = 1310,
            WindArcher3 = 1311,
            WindArcher4 = 1312,

            NightWalker1 = 1400,
            NightWalker2 = 1410,
            NightWalker3 = 1411,
            NightWalker4 = 1412,

            ThunderBreaker1 = 1500,
            ThunderBreaker2 = 1510,
            ThunderBreaker3 = 1511,
            ThunderBreaker4 = 1512,

            Legend = 2000,
            Aran1 = 2100,
            Aran2 = 2110,
            Aran3 = 2111,
            Aran4 = 2112,

            Evan1 = 2001,
            Evan2 = 2201,
            Evan3 = 2210,
            Evan4 = 2211,
            Evan5 = 2212,
            Evan6 = 2213,
            Evan7 = 2214,
            Evan8 = 2215,
            Evan9 = 2216,
            Evan10 = 2217,
            Evan11 = 2218,

            Mercedes = 2002,
            Mercedes1 = 2300,
            Mercedes2 = 2310,
            Mercedes3 = 2311,
            Mercedes4 = 2312,

            Phantom = 2003,
            Phantom1 = 2400,
            Phantom2 = 2410,
            Phantom3 = 2411,
            Phantom4 = 2412,

            Luminosu1 = 2700,
            Luminosu2 = 2710,
            Luminosu3 = 2711,
            Luminosu4 = 2712,

            Citizen = 3000,

            DemonSlayer = 3001,
            DemonSlayer1 = 3100,
            DemonSlayer2 = 3110,
            DemonSlayer3 = 3111,
            DemonSlayer4 = 3112,

            BattleMage1 = 3200,
            BattleMage2 = 3210,
            BattleMage3 = 3211,
            BattleMage4 = 3212,

            WildHunter1 = 3300,
            WildHunter2 = 3310,
            WildHunter3 = 3311,
            WildHunter4 = 3312,

            Mechanic1 = 3500,
            Mechanic2 = 3510,
            Mechanic3 = 3511,
            Mechanic4 = 3512,

            Mihail = 5000,
            Mihail1 = 5100,
            Mihail2 = 5110,
            Mihail3 = 5111,
            Mihail4 = 5112,

            Kaiser = 6000,
            Kaiser1 = 6100,
            Kaiser2 = 6110,
            Kaiser3 = 6111,
            Kaiser4 = 6112,
        }
    }
}
