using System;

namespace Chronometer // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Chronometer chronometer = new Chronometer();

            string cmd = Console.ReadLine();

            while (cmd != "exit")
            {
                switch (cmd)
                {
                    case "start":
                        Task.Run(() =>
                       {
                           chronometer.Start();
                       });

                        break;
                    case "stop":
                        chronometer.Stop();
                        break;
                    case "lap":
                        Console.WriteLine(chronometer.Lap());
                        break;
                    case "laps":

                        if (!chronometer.Laps.Any())
                        {
                            Console.WriteLine("Laps: no laps");
                            break;
                        }

                        else
                        {
                            Console.WriteLine("Laps: ");
                            for (int i = 0; i < chronometer.Laps.Count; i++)
                            {
                                Console.WriteLine($"{i}. {chronometer.Laps[i]}");
                            }
                        }
                        break;
                    case "reset":
                        chronometer.Reset();
                        break;
                    case "time":
                        Console.WriteLine(chronometer.GetTime);
                        break;
                    default:
                        break;
                }

                cmd = Console.ReadLine();
            }
        }
    }
}