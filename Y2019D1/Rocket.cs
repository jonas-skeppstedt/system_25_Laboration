namespace Y2019D1
{
    public static class Rocket
    {
        public static int FuelCount(int value)
        {
            var returnValue = value / 3 - 2;

            return returnValue <= 0 ? 0 : returnValue;
        }
    }
}
