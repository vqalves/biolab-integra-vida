using BiopasIntegraVida.Infrastructure.Interfaces;

namespace BiopasIntegraVida.Web.Configuration
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Current()
        {
            return DateTime.Now;
        }
    }
}
