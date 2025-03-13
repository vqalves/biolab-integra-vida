using BiolabIntegraVida.Infrastructure.Interfaces;

namespace BiolabIntegraVida.Web.Configuration
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Current()
        {
            return DateTime.Now;
        }
    }
}
