using System;
using System.Collections.Generic;
using System.Data.Common;
using TheScoreBook.exceptions;

namespace TheScoreBook.models.enums.enumclass
{
    public class ScoringType : EnumClass
    {
        public static readonly ScoringType TenZone = new("TenZone", 0);
        public static readonly ScoringType FiveZone = new("FiveZone", 1);

        private ScoringType(string name, int id) : base(name, id) { }
        
        private static HashSet<int> usedIds;
        protected override void AddUsedIDToSet(int id)
        {
            usedIds ??= new();
            
            if (!usedIds.Add(id))
                throw new IDAlreadyInUseException($"ID {id} already in use for enum type {GetType()}");
        }

        public override string ToString()
            => Id switch
            {
                0 => "X, 10, 9, 8, ..., 1, M",
                1 => "9, 7, 5, 3, 1, M",
                _ => throw new ArgumentOutOfRangeException()
            };

        public Score MaxScore() => Id == 0 ? Score.X : Score.NINE;

        public static explicit operator ScoringType(string obj)
            => obj switch
            {
                "TenZone" => TenZone,
                "FiveZone" => FiveZone,
                _ => throw new InvalidCastException()
            };
    }
}