using MerckCuida.Infrastructure.Interfaces;

namespace MerckCuida.Infrastructure
{
    public sealed class AgeCalculator
    {
        public static int Calculate(DateTime nascimento, IDateTimeProvider dateTimeProvider)
        {
            DateTime now = dateTimeProvider.Current().Date;

            int age = now.Year - nascimento.Year;
            if (nascimento > now.AddYears(-age))
                age--;

            return age;
        }
    }
}
