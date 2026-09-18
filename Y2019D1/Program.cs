using AdventOfCode.Common;

namespace Y2019D1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "input.txt");
            var text = File.ReadAllText(path);

            var numbers = Input.Numbers(text);






            //var massSum = 0;

            //foreach (var number in numbers)
            //{
            //    var mass = Rocket.FuelCount(number);
            //    massSum += mass;

            //    Console.WriteLine($"input: {number,10} - mass: {mass}");
            //}

            //Console.WriteLine("Total sum : " + massSum);







            //int startmass = 1969;

            //while (startmass > 0)
            //{
            //    var mass = Rocket.FuelCount(startmass);

            //    Console.WriteLine(mass);

            //    startmass = mass;
            //}



            var massSum = 0;

            foreach (var number in numbers)
            {
                int startmass = number;

                int subTotal = 0;

                while (startmass > 0)
                {
                    var mass = Rocket.FuelCount(startmass);

                    Console.WriteLine(mass);

                    subTotal += mass;
                    startmass = mass;
                }

                massSum += subTotal;
            }

            Console.WriteLine(massSum);
        }
    }
}
