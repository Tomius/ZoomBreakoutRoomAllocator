using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ZoomBreakoutRoomAllocator
{
    class Program
    {
        static int NumRepeats(int person, List<int> group, List<HashSet<int>> allPreviousBreakoutRooms)
        {
            var repeats = 0;
            foreach (var previousRoom in allPreviousBreakoutRooms)
            {
                if (previousRoom.Contains(person))
                {
                    foreach (var member in group)
                    {
                        if (previousRoom.Contains(member))
                        {
                            repeats++;
                        }
                    }
                }
            }
            return repeats;
        }

        static Random rng = new Random();
        static int BestGroupIndex(int person, int maxPeoplePerRoom, List<List<int>> allocatedRooms, List<HashSet<int>> allPreviousBreakoutRooms, out int repeatCount)
        {
            int minIndex = -1;
            int minValue = int.MaxValue;
            int minGroupSize = 1000;

            /*minIndex = rng.Next(0, allocatedRooms.Count);
            minValue = NumRepeats(person, allocatedRooms[minIndex], allPreviousBreakoutRooms);*/
            for (int i = 0; i < allocatedRooms.Count; ++i)
            {
                if (allocatedRooms[i].Count < maxPeoplePerRoom)
                {
                    var repeats = NumRepeats(person, allocatedRooms[i], allPreviousBreakoutRooms);
                    if (repeats < minValue || (repeats == minValue && allocatedRooms[i].Count < minGroupSize))
                    {
                        minValue = repeats;
                        minIndex = i;
                        minGroupSize = allocatedRooms[i].Count;
                    }
                }
            }

            if (minIndex == -1)
            {
                throw new Exception("Couldn't put a person to any group!");
            }

            repeatCount = minValue;
            return minIndex;
        }

        static void Main(string[] args)
        {
            var initialFileName = "Participants.txt";

            Random rnd = new Random();
            var participants = File.ReadLines(initialFileName).Where(x => x.Length != 0).OrderBy(x => rnd.Next()).ToArray();
            Dictionary<string, int> particitantIndex = new Dictionary<string, int>();
            for (int i = 0; i < participants.Length; ++i)
            {
                particitantIndex.Add(participants[i], i);
            }

            Console.WriteLine("Please enter the number of breakout rooms to allocate");
            var roomCountStr = Console.ReadLine();
            int roomCount;

            while (!int.TryParse(roomCountStr, out roomCount))
            {
                Console.WriteLine($"{roomCountStr} is not an integer, please enter a number");
                roomCountStr = Console.ReadLine();
            }

            List<HashSet<int>> allPreviousBreakoutRooms = new List<HashSet<int>>();
            int previousRoomCount = 0;
            string Filename() => $"PreviousBreakoutRooms_{previousRoomCount}.txt";
            while (File.Exists(Filename()))
            {
                var lines = File.ReadLines(Filename()).ToArray();
                HashSet<int> current = new HashSet<int>();
                for (int i = 0; i < lines.Length; ++i)
                {
                    if (lines[i].Length == 0)
                    {
                        if (current.Count != 0)
                        {
                            allPreviousBreakoutRooms.Add(current);
                            current = new HashSet<int>();
                        }
                    }
                    else
                    {
                        if (particitantIndex.TryGetValue(lines[i], out int value))
                        {
                            current.Add(value);
                        }
                        else
                        {
                            // It's okay that someone was in a breakout room, and has now left
                        }
                    }
                }
                previousRoomCount++;
            }

            using StreamWriter file = new StreamWriter(Filename());

            List<List<int>> allocatedRooms = new List<List<int>>();
            for (int i = 0; i < roomCount; ++i)
            {
                allocatedRooms.Add(new List<int>());
            }

            int maxPeoplePerRoom = NewMethod(participants, roomCount);

            int totalRepeats = 0;
            for (int p = 0; p < participants.Length; ++p)
            {
                var idx = BestGroupIndex(p, maxPeoplePerRoom, allocatedRooms, allPreviousBreakoutRooms, out int repeats);
                totalRepeats += repeats;
                allocatedRooms[idx].Add(p);
            }

            foreach (var room in allocatedRooms)
            {
                foreach (var personId in room)
                {
                    Console.WriteLine(participants[personId]);
                    file.WriteLine(participants[personId]);
                }
                Console.WriteLine("");
                file.WriteLine("");
            }

            Console.WriteLine($"Total repeats: {totalRepeats}");
        }

        private static int NewMethod(string[] participants, int roomCount)
        {
            return (participants.Length + roomCount - 1) / roomCount;
        }
    }
}
