using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using System.Threading.Tasks;
using System.Threading;


public sealed class SendMessageType {
    private string method;

    private SendMessageType(string method) {
        this.method = method;
    }

    public override string ToString() {
        return this.method;
    } 

    public static readonly SendMessageType Param1 = new SendMessageType("CS");
}

public sealed class RecvMessageType {
    private string method;

    private RecvMessageType(string method) {
        this.method = method;
    }

    public override string ToString() {
        return this.method;
    } 

    public static readonly RecvMessageType Prm = new RecvMessageType("Prm");
    public static readonly RecvMessageType Ok  = new RecvMessageType("Ok");
}

/*
    Singleton Serial Input Manager.
    Here we receive messages from the hapkit.
    Here we send messages to the hapkit to set the state (and thus select the right feedback)
*/
public class SerialInputManager : Singleton<SerialInputManager> {

    static readonly float Timeout = 0.5f;
    private float timer;

    // private hapkit params
    private string port;
    public SerialPort serial;
    private Queue<byte[]> msgQueue = new Queue<byte[]>();
    private List<byte> recvBuf = new List<byte>();


    private int expectedState;

    // on osx - ls /dev/cu* and choose usb modem
	void Start () {
        this.port = "COM4"; //THIS IS PC DEPENDANT (sometimes also changing usb port may change the port name, sometimes no on my Windows !wat!)
        this.serial = new SerialPort(this.port);
        this.serial.RtsEnable = true;
        this.serial.BaudRate = 2000000; 
        //this.serial.BaudRate = 921600; 
        this.serial.ReadTimeout = 1000;
        //this.serial.ReadBufferSize =  2 * 1024 * 1024; 
        //this.serial.WriteBufferSize =  2 * 1024 * 1024; 
        this.serial.Open();
        Debug.Assert(this.serial.IsOpen);
        this.expectedState = Globals.HapkitState;   //this is the state we expect to see from the hapkit (so, it's the state of the game that we want to set)
        print(expectedState);
        //WriteMessage(SendMessageType.Reset);
        /*
            Nice and easy way to run a task NOT on unity thread. Fundamental to make communication working 
        */
        Task.Factory.StartNew(() =>
        {
            while (true)
            {
                ThreadUpdate();
                Thread.Sleep(1);
            }
           
        });
    }

    void Update()
    {
        if (this.msgQueue.Count > 0) {
            byte[] msg = this.msgQueue.Peek();
            Instance.serial.Write(msg, 0, msg.Length);
        }
    }
	
	/*
        Read lines sent by the Hapkit and process them when they are terminated.
    */
	void ThreadUpdate () {
        try {
            this.serial.NewLine = "\n";
            if (this.serial.BytesToRead > 0)  {
                string msg = serial.ReadLine();
                processIncomingMessage(msg);
            }
        }
        catch (TimeoutException ex) {
            // this never triggers wat
            Debug.Log("Didn't receive anything from Arduino!"); 
        }
    }

    /*
        Process/parse incoming messages.
        Stores values useful to detect the release to fire event. -> Maybe could be refactored in its own method?
        Check if the state we get from hapkit matches the expected state. If not, try to send a message to set the expected state on the hapkit.  -> Maybe could be refactored in its own method?
    */
    private void processIncomingMessage(string msg) {
        if (msg.StartsWith(RecvMessageType.Prm.ToString())) {
            //Debug.Log("Params: " + msg);
            //this.timer = 0.0f;
            char[] whitespace = new char[] { ' ', '\t' };
            string[] words = msg.Split(whitespace);

            float position  = float.Parse(words[1].Replace(".", ",")); //Regions related: on italian systems floats want comma. Otherwise, the parser would simply drop the part after the number without warning you
            float velocity =  float.Parse(words[2].Replace(".", ","));
            float acceleration =  float.Parse(words[3].Replace(".", ","));
            int state =  int.Parse(words[5]);



            Globals.HapkitPosition = position; //save the position globally as its used to "move" the game

            /*
                Check if the state we get from hapkit matches the expected state. 
                If not, try to send a message to set the expected state on the hapkit.
                In that way we are sure that the state of the hapkit (i.e. the feedback produced) will match the game one in a reasonable time (user perceives it as in real time, so it works fine enough)
            */
            if (this.expectedState != state) {
                string str = String.Format("{0}\n",  this.expectedState); 
                byte[] snd = System.Text.Encoding.ASCII.GetBytes(str);
                this.serial.Write(snd, 0, snd.Length);
            } else {
                Globals.HapkitState = state;
            }

            
            Triple triple = new Triple();
            triple.position = position;
            triple.velocity = velocity;
            triple.acceleration = acceleration;
            //print(triple.position + " " + triple.velocity + " " + triple.acceleration);
            //Store 5 values in the global time window/queue to detect the release to fire
            if (Globals.ToFireQueue.Count > 5)
            {
                Globals.ToFireQueue.Dequeue();
            }
            Globals.ToFireQueue.Enqueue(triple);
        }
        else {
            //Debug.Log("bad");
            //Debug.Log(msg);
        }
    }
    /*
        The state is set indirectly: here, we set the expected state only. The global hapkit state is updated as already explained above.
    */
    public static void SetState(int state) {
        Instance.expectedState = state;
    }
}
