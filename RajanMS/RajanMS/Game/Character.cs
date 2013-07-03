using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace RajanMS.Game
{
    public sealed class Character
    {
        public ObjectId Id { get; set; }

        public Character()
        {
            SP = new byte[4];
        }

        public int AccountId { get; set; }
        public byte WorldId { get; set; }

        public int CharId { get; set; }
        public string Name { get; set; }
        public byte Gender { get; set; }
        public byte SkinColor { get; set; }
        public int FaceId { get; set; }
        public int HairId { get; set; }

        public byte Level { get; set; }
        public short Job { get; set; }
        public short Str { get; set; }
        public short Dex { get; set; }
        public short Int { get; set; }
        public short Luk { get; set; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int MP { get; set; }
        public int MaxMP { get; set; }
        public short AP { get; set; }
        public byte[] SP { get; set; }
        public int EXP { get; set; }
        public int Fame { get; set; }
        public int DemonSlayerAccessory { get; set; }
        public byte Fatigue { get; set; }
        public byte BattleRank { get; set; }
        public int BattlePoints { get; set; }
        public int BattleEXP { get; set; }

        public int MapId { get; set; }
        public byte SpawnPoint { get; set; }

        public int Ambition { get; set; }
        public int Insight { get; set; }
        public int Willpower { get; set; }
        public int Diligence { get; set; }
        public int Empathy { get; set; }
        public int Charm { get; set; }
        public short AmbitionGained { get; set; }
        public short InsightGained { get; set; }
        public short WillpowerGained { get; set; }
        public short DiligenceGained { get; set; }
        public short EmpathyGained { get; set; }
        public short CharmGained { get; set; }
    }
}
