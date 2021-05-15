using System;
using TheScoreBook.acessors;
using TheScoreBook.behaviours;
using TheScoreBook.localisation;
using Xamarin.Forms;

namespace TheScoreBook.models.enums.enumclass
{
    public class RescoreType : EnumClass
    {
        public static RescoreType LONG => new("LongPress", 0);
        public static RescoreType DOUBLE => new("DoubleTap", 1);

        public RescoreType(string name, int id) : base(name, id)
        {
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