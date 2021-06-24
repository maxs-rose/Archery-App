using System;
using System.Collections.Generic;
using TheScoreBook.exceptions;
using Xamarin.Forms;

namespace TheScoreBook.models.enums.enumclass
{
    public class Score : EnumClass
    {
        public static readonly Score X = new("X", 11, 10);
        public static readonly Score TEN = new("10", 10, 10);
        public static readonly Score NINE = new("9", 9, 9);
        public static readonly Score EIGHT = new("8", 8, 8);
        public static readonly Score SEVEN = new("7", 7, 7);
        public static readonly Score SIX = new("6", 6, 6);
        public static readonly Score FIVE = new("5", 5, 5);
        public static readonly Score FOUR = new("4", 4, 4);
        public static readonly Score THREE = new("3", 3, 3);
        public static readonly Score TWO = new("2", 2, 2);
        public static readonly Score ONE = new("1", 1, 1);
        public static readonly Score MISS = new("M", 0, 0);

        public int Value { get; }
        private Score(string name, int id, int value) : base(name, id) => Value = value;


        private static HashSet<int> usedIds;
        protected override void AddUsedIDToSet(int id)
        {
            usedIds ??= new();
            
            if (!usedIds.Add(id))
                throw new IDAlreadyInUseException($"ID {id} already in use for enum type {GetType()}");
        }

        public static explicit operator Color(Score s)
        {
            if (s is null)
                return Color.Gray;

            switch (s.Id)
            {
                case 11:
                case 10:
                case 9:
                    return Color.Yellow;
                case 8:
                case 7:
                    return Color.Red;
                case 6:
                case 5:
                    return Color.Blue;
                case 4:
                case 3:
                    return Color.Black;
                default:
                    return Color.Black;
            }
        }
        
        public static explicit operator Score(int id)
            => id switch
            {
                11 => X,
                10 => TEN,
                9 => NINE,
                8 => EIGHT,
                7 => SEVEN,
                6 => SIX,
                5 => FIVE,
                4 => FOUR,
                3 => THREE,
                2 => TWO,
                1 => ONE,
                0 => MISS,
                _ => throw new InvalidCastException($"{id} is not a valid score id")
            };

        public static explicit operator Score(string name)
            => name switch
            {
                "X" => X,
                "10" => TEN,
                "9" => NINE,
                "8" => EIGHT,
                "7" => SEVEN,
                "6" => SIX,
                "5" => FIVE,
                "4" => FOUR,
                "3" => THREE,
                "2" => TWO,
                "1" => ONE,
                "M" => MISS,
                _ => null
            };

        public bool IsFiveZoneScore()
            => Value % 2 != 0;

        public static bool operator >(Score a, Score b)
            => a.Value > b.Value;

        public static bool operator <(Score a, Score b)
            => a.Value < b.Value;

        public static bool operator <=(Score a, Score b)
            => a.Value <= b.Value;

        public static bool operator >=(Score a, Score b)
            => a.Value >= b.Value;
    }
}