using MerckCuida.Infrastructure;
using MerckCuida.UnitTests.Mocks;

namespace MerckCuida.UnitTests
{
    public class AgeCalculatorTests
    {
        [Fact]
        public void MenorDeIdade()
        {
            var nascimento = DateTime.Parse("2000-02-01");
            var atual = new DateTimeProviderMock(DateTime.Parse("2018-01-31"));

            var idade = AgeCalculator.Calculate(nascimento, atual);
            Assert.Equal(17, idade);
        }

        [Fact]
        public void MaiorDeIdade()
        {
            var nascimento = DateTime.Parse("2000-02-01");
            var atual = new DateTimeProviderMock(DateTime.Parse("2018-02-01"));

            var idade = AgeCalculator.Calculate(nascimento, atual);
            Assert.Equal(18, idade);
        }
    }
}
