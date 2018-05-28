using System.Collections.Generic;
public struct Triple
{
    public float position;
    public float velocity;
    public float acceleration;
}

public class Globals
{
    private static int _mode;
    private static int _hmode;
    private static bool _exploded;
    private static float _hapkitPosition;
    private static bool _hapkit;
    private static bool _releaseToFire;
    private static Queue<Triple> _toFireQueue = new Queue<Triple>();

    public static Queue<Triple> ToFireQueue
    {
        get
        {
            // Reads are usually simple
            return _toFireQueue;
        }
        set
        {
            // You can add logic here for race conditions,
            // or other measurements
            _toFireQueue = value;
        }
    }
    public static bool ReleaseToFire
    {
        get
        {
            // Reads are usually simple
            return _releaseToFire;
        }
        set
        {
            // You can add logic here for race conditions,
            // or other measurements
            _releaseToFire = value;
        }
    }
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