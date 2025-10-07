using Ant0nRocket.Lib.Extensions;

namespace Ant0nRocket.Lib.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            for (var i = 0; i < 1000; i++)
            {
                var guid = Guid.NewGuid();

                var d1 = DateTime.UtcNow;
                var seqGuid = guid.ToUnixEpochGuid(d1);
                var d2 = seqGuid.GetDateTimeUtc();

                System.Console.WriteLine($"{d1:O} | {seqGuid} | {d2:O}");
            }
            System.Console.ReadLine();
        }
    }
}
