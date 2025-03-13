using MerckCuida.Infrastructure.Interfaces;

namespace MerckCuida.Web.Configuration
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Current()
        {
            return DateTime.Now;
        }
    }
}
