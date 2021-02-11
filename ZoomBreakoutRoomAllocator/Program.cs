using System;
using System.IO;
using System.Linq;

namespace ZoomBreakoutRoomAllocator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                throw new Exception($"Usage: {nameof(ZoomBreakoutRoomAllocator)}.exe path/to/participantsName.txt");
            }

            Random rnd = new Random();
            var participants = File.ReadLines(args[0]).Where(x => x.Length != 0).OrderBy(x => rnd.Next()).ToArray();

            Console.WriteLine("Please enter the number of breakout rooms to allocate");
            var roomCountStr = Console.ReadLine();
            int roomCount;

            while (!int.TryParse(roomCountStr, out roomCount))
            {
                Console.WriteLine($"{roomCountStr} is not an integer, please enter a number");
                roomCountStr = Console.ReadLine();
            }



            int p = 0;
            for (int i = 0; i < roomCount; ++i)
            {
                Console.WriteLine($"Room {i + 1}:");
                for (; p < (i+1)*(participants.Length / roomCount); ++p)
                {
                    Console.WriteLine(participants[p]);
                }
                Console.WriteLine("");
            }
        }
    }
}
