namespace Chronometer
{
    public interface IChronometer
    {
        void Start();

        void Stop();

        void Reset();

        string Lap();

        string GetTime { get; }

        List<string> Laps { get; }
    }
}
