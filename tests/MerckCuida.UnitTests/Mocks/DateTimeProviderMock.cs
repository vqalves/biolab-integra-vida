using MerckCuida.Infrastructure.Interfaces;

namespace MerckCuida.UnitTests.Mocks
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
