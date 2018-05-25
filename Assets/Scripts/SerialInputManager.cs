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
        this.port = "COM4";
        this.serial = new SerialPort(this.port);
        this.serial.RtsEnable = true;
        this.serial.BaudRate = 2000000; 
        //this.serial.BaudRate = 921600; 
        this.serial.ReadTimeout = 1000;
        //this.serial.ReadBufferSize =  2 * 1024 * 1024; 
        //this.serial.WriteBufferSize =  2 * 1024 * 1024; 
        this.serial.Open();
        Debug.Assert(this.serial.IsOpen);
        this.expectedState = Globals.HapkitState;
        //WriteMessage(SendMessageType.Reset);
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
	
	// Update is called once per frame
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

    private void processIncomingMessage(string msg) {
        if (msg.StartsWith(RecvMessageType.Prm.ToString())) {
            //Debug.Log("Params: " + msg);
            //this.timer = 0.0f;
            char[] whitespace = new char[] { ' ', '\t' };
            string[] words = msg.Split(whitespace);

            float position  = float.Parse(words[1]);
            float velocity =  float.Parse(words[2]);
            float acceleration =  float.Parse(words[3]);
            int state =  int.Parse(words[4]);



            Globals.HapkitPosition = position;

            if (this.expectedState != state) {
                string str = String.Format("State {0}\n",  this.expectedState); 
                byte[] snd = System.Text.Encoding.ASCII.GetBytes(str);
                this.serial.Write(snd, 0, snd.Length);
            } else {
                Globals.HapkitState = state;
            }


            /*Couple couple = new Couple();
            couple.position = pos;
            couple.acceleration = float.Parse(words[3].Replace('.', ','));
            if (Globals.ToFireQueue.Count > 10)
            {
                Globals.ToFireQueue.Dequeue();
            }
            Globals.ToFireQueue.Enqueue(couple);*/
        }
        else {
            Debug.Log("bad");
        }
    }
    public static void SetState(int state) {
        Instance.expectedState = state;
    }
}
