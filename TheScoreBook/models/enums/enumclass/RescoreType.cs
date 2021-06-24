using System;
using System.Collections.Generic;
using TheScoreBook.acessors;
using TheScoreBook.behaviours;
using TheScoreBook.exceptions;
using TheScoreBook.localisation;
using Xamarin.Forms;

namespace TheScoreBook.models.enums.enumclass
{
    public class RescoreType : EnumClass
    {
        public static readonly RescoreType LONG = new("LongPress", 0);
        public static readonly RescoreType DOUBLE = new("DoubleTap", 1);

        private RescoreType(string name, int id) : base(name, id) { }

        private static HashSet<int> usedIds;
        protected override void AddUsedIDToSet(int id)
        {
            usedIds ??= new();
            
            if (!usedIds.Add(id))
                throw new IDAlreadyInUseException($"ID {id} already in use for enum type {GetType()}");
        }

        public static Behavior<Button> GetCurrentSetting(Command command)
            => Settings.RescoreType.Id switch
            {
                0 => new LongButtonPressBehaviour() {Command = command},
                1 => new DoubleTapButtonBehaviour() {Command = command},
                _ => throw new IndexOutOfRangeException($"Unknown id")
            };
        
        public static explicit operator RescoreType(int id)
            => id switch
            {
                0 => LONG,
                1 => DOUBLE,
                _ => throw new IndexOutOfRangeException($"Unknown id")
            };

        public static explicit operator int(RescoreType type) => type.Id;
        public override string ToString() => LocalisationManager.Instance[Name];
    }
}