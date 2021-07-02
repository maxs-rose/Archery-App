using System.Linq;
using System.Reflection;
using TheScoreBook.Ui.factories;

namespace TheScoreBook.Ui
{
    public class Services
    {
        private static IRoundFactory _roundFactory;
        private static IRoundDataFactory _roundDataFactory;
        
        static Services()
        {
            _roundDataFactory = new RoundDataFactory();
            _roundFactory = new RoundFactory(_roundDataFactory);
        }

        public static T Create<T>(params object[] roundCreationParameters) 
            => new Services().CreateRoundOrRoundData<T>(roundCreationParameters);
        
        private T CreateRoundOrRoundData<T>(params object[] creationParameters)
        {
            var serviceFields = GetType().GetFields(BindingFlags.Static | BindingFlags.NonPublic);
            var methods = serviceFields
                .SelectMany(f => f.FieldType.GetMethods())
                .Where(m => m.Name.Contains("Create"));

            // ReSharper disable once ReplaceWithSingleCallToSingle
            var selectedMethod = methods
                .Where(m => m.ReturnType == typeof(T))
                .Where(m => m.GetParameters().Length == creationParameters.Length)
                .Where(m 
                    => m.GetParameters()
                        .Select(p => p.ParameterType)
                        .SequenceEqual(creationParameters
                            .Select(cp => cp.GetType())))
                .Single();
            
            return (T) selectedMethod.Invoke(
                serviceFields.Single(pt => pt.FieldType == selectedMethod.ReflectedType).GetValue(null),
                creationParameters);
        }
    }
}