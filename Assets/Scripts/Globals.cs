public class Globals
{
    private static int _mode;
    private static bool _exploded;
    private static float _hapkitPosition;

    public static bool Exploded
    {
        get
        {
            // Reads are usually simple
            return _exploded;
        }
        set
        {
            // You can add logic here for race conditions,
            // or other measurements
            _exploded = value;
        }
    }
    public static float HapkitPosition
    {
        get
        {
            // Reads are usually simple
            return _hapkitPosition;
        }
        set
        {
            // You can add logic here for race conditions,
            // or other measurements
            _hapkitPosition = value;
        }
    }

    public static int Mode
    {
        get
        {
            // Reads are usually simple
            return _mode;
        }
        set
        {
            // You can add logic here for race conditions,
            // or other measurements
            _mode = value;
        }
    }
    // Perhaps extend this to have Read-Modify-Write static methods
    // for data integrity during concurrency? Situational.
}