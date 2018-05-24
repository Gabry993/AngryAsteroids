public class Globals
{
    private static int _mode;
    private static int _hmode;
    private static bool _exploded;
    private static float _hapkitPosition;
    private static bool _hapkit;

        public static bool Hapkit
    {
        get
        {
            // Reads are usually simple
            return _hapkit;
        }
        set
        {
            // You can add logic here for race conditions,
            // or other measurements
            _hapkit = value;
        }
    }
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

    public static int HapkitState
    {
        get
        {
            // Reads are usually simple
            return _hmode;
        }
        set
        {
            // You can add logic here for race conditions,
            // or other measurements
            _hmode = value;
        }
    }
    // Perhaps extend this to have Read-Modify-Write static methods
    // for data integrity during concurrency? Situational.
}