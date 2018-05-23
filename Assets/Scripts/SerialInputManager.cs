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

    public static readonly SendMessageType Param1 = new SendMessageType("P1");
    public static readonly SendMessageType Param2 = new SendMessageType("P2");
    public static readonly SendMessageType Params = new SendMessageType("PRM");
    public static readonly SendMessageType Reset  = new SendMessageType("RS");
    public static readonly SendMessageType Live   = new SendMessageType("LV");
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


    private int prevState;

    // on osx - ls /dev/cu* and choose usb modem
	void Start () {
        this.port = "COM3";
        this.serial = new SerialPort(this.port);
        this.serial.RtsEnable = true;
        //this.serial.BaudRate = 2000000; 
        this.serial.BaudRate = 921600; 
        this.serial.ReadTimeout = 1000;
        //this.serial.ReadBufferSize =  2 * 1024 * 1024; 
        //this.serial.WriteBufferSize =  2 * 1024 * 1024; 
        this.serial.Open();
        Debug.Assert(this.serial.IsOpen);
        this.prevState = Globals.HapkitState;
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
        print("UPDATING");
        //print(prevState);
        int newstate = Globals.HapkitState;
        //print(newstate);
       
        if (this.prevState != newstate){
            this.prevState = newstate;
            //print(newstate);
            string str = String.Format("{0}\n", newstate);
            print(str);
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(str);
            Instance.serial.Write(msg, 0, msg.Length);
        }

    }
	
	// Update is called once per frame
	void ThreadUpdate () {
        try {
            string msg = this.readIncomingMessage();
            if (msg != null) {
                processIncomingMessage(msg);
                //Debug.Log(msg);
            }
        }
        catch (TimeoutException ex) {
            // this never triggers wat
            Debug.Log("Didn't receive anything from Arduino!"); 
        }

        // update timer 
        /*this.timer += Time.deltaTime;
        if (this.timer >= Timeout) {
            this.SendNextMessageInQueue();
            this.timer = 0.0f;
        }*/
	}

    private string readIncomingMessage()  {
        //Debug.Log(this.recvBuf.Count);
        //Debug.Log(this.serial.BytesToRead);
        serial.NewLine = "\n";
        while (this.serial.BytesToRead > 0) {
            /*
            int chr = this.serial.ReadByte();
            byte by = Convert.ToByte(chr);
            if (by == '\n') {
            //if (this.recvBuf.Count > 10) {
                string msg = System.Text.Encoding.Default.GetString(this.recvBuf.ToArray());
                this.recvBuf.Clear();
                return msg;
            }
            this.recvBuf.Add(by);*/
            string msg = serial.ReadLine();
            //Debug.Log(msg);
            //this.serial.DiscardInBuffer();
            return msg;
        }
        return null;
    }

            
    private void processIncomingMessage(string msg) {
        //Debug.Log(msg);
        if (msg.StartsWith(RecvMessageType.Prm.ToString())) {
            //Debug.Log("Params: " + msg);
            //this.timer = 0.0f;
            char[] whitespace = new char[] { ' ', '\t' };
            string[] words = msg.Split(whitespace);
            //string[] words = msg.Split(' ');
            Globals.HapkitPosition = float.Parse(words[1].Replace('.', ','));

        }
        else if (msg.StartsWith(RecvMessageType.Ok.ToString())) {
            Debug.Log(msg);
            //Debug.Log("Ack");
            this.timer = 0.0f;
            this.msgQueue.Dequeue();
            this.SendNextMessageInQueue();
        }
        else {
            Debug.Log("bad");
            //Debug.Log(msg);
        }
    }

    private void SendNextMessageInQueue() {
        if (this.msgQueue.Count > 0) {
            byte[] msg = this.msgQueue.Peek();
            Debug.Log("snd");
            this.serial.Write(msg, 0, msg.Length);
        }
    }

    public void AddToWriteQueue(byte[] message) {
        this.msgQueue.Enqueue(message);
    }

    


    public static void WriteMessage(SendMessageType type, params float[] param) {
        string flt = String.Format("{0:0.#####}", param[0] );
        string str = null;
        if (type == SendMessageType.Param1)   { str = String.Format("{0} {1}\n", type.ToString(), flt); }
        //if (type == SendMessageType.Reset)  { str = String.Format("{0}\n", type.ToString()); }
        //if (type == SendMessageType.Param1) { str = String.Format("{0} {1}\n", type.ToString(), param[0]); }
        //if (type == SendMessageType.Param2) { str = String.Format("{0} {1} {2}\n", type.ToString(), param[0], param[1]); }
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(str);
        //Instance.AddToWriteQueue(msg);
        //Instance.SendNextMessageInQueue();
        Instance.serial.Write(msg, 0, msg.Length);
    }
}
