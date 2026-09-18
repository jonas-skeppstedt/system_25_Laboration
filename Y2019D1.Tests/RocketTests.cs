namespace Y2019D1.Tests
{
    public class RocketTests
    {
        [Theory]
        [InlineData(12, 2)]
        [InlineData(14, 2)]
        [InlineData(1969, 654)]
        [InlineData(100756, 33583)]
        public void FuelCount_ShouldReturnExpectedFuelMass(int value, int expected)
        {
            // Act
            var actual = Rocket.FuelCount(value);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
