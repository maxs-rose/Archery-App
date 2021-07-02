using System;
using TheScoreBook.acessors;
using TheScoreBook.behaviours;
using TheScoreBook.localisation;
using TheScoreBook.models.enums;
using Xamarin.Forms;

namespace TheScoreBook.Ui.enums
{
    public class RescoreType : EnumClass
    {
        public static readonly RescoreType LONG = new("LongPress", 0);
        public static readonly RescoreType DOUBLE = new("DoubleTap", 1);

        private RescoreType(string name, int id) : base(name, id) { }

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