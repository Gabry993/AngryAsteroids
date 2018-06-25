using System.Collections.Generic;

/*
Simple object to hold 3 float values.
Should be refactoredand placed in its own file
*/
public struct Triple
{
    public float position;
    public float velocity;
    public float acceleration;
}


/*
Here we have all global variables needed for the game.
They are useful to provide system-wide information to many classes at the same time.
We don't care about concurrent access as the behaviour is very simple (usually they are written only by one class) and in practice this work well for the game
*/
public class Globals
{
    private static int _mode; //0 is catapult, 1 is rocket
    private static int _hmode; //not needed anymore
    private static bool _exploded;	//needed to avoid the game to reset before explosion animation occurs
    private static float _hapkitPosition;	//position of the hapkit handle
    private static bool _hapkit;	//true if we are playing with a Hapkit, flase if we want to play with the keyboar
    private static bool _releaseToFire;	//true to enable release to fire function
    private static Queue<Triple> _toFireQueue = new Queue<Triple>();	//Queue in which we store values used to detect the release to fire event

    public static Queue<Triple> ToFireQueue
    {
        get
        {
            return _toFireQueue;
        }
        set
        {
        	_toFireQueue = value;
        }
    }
    public static bool ReleaseToFire
    {
        get
        {
        	return _releaseToFire;
        }
        set
        {
            _releaseToFire = value;
        }
    }
    public static bool Hapkit
    {
        get
        {
            return _hapkit;
        }
        set
        {
            _hapkit = value;
        }
    }
    public static bool Exploded
    {
        get
        {
            return _exploded;
        }
        set
        {
            _exploded = value;
        }
    }
    public static float HapkitPosition
    {
        get
        {
            return _hapkitPosition;
        }
        set
        {
            _hapkitPosition = value;
        }
    }

    public static int Mode
    {
        get
        {
            return _mode;
        }
        set
        {
            _mode = value;
        }
    }

    public static int HapkitState
    {
        get
        {
             return _hmode;
        }
        set
        {
            _hmode = value;
        }
    }
    // Perhaps extend this to have Read-Modify-Write static methods
    // for data integrity during concurrency? Situational.
}