using BiolabIntegraVida.Infrastructure.Interfaces;

namespace BiolabIntegraVida.UnitTests.Mocks
{
    public class DateTimeProviderMock : IDateTimeProvider
    {
        private DateTime Value { get; init; }

        public DateTimeProviderMock(DateTime value)
        {
            this.Value = value;
        }

        public DateTime Current()
        {
            return Value;
        }
    }
}
