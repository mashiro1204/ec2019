using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;
 
public class SerialHandler : MonoBehaviour
{
    public string portName = "COM2";
    public int baudRate = 9600;
 
    private SerialPort serialPort_;
    private Thread thread_;
    private bool isRunning_ = false;
 
    private string message_;
    private bool isNewMessageReceived_ = false;
 
    void Start()
    {
        Debug.LogWarning("Start");
        Open();
    }
 
    void Update()
    {
        Debug.LogWarning("Serial-Update");
        if (isNewMessageReceived_)
        {
            OnDataReceived(message_);
        }
    }
    void OnDestroy()
    {
        Debug.LogWarning("OnDestroy");
        Close();
    }
 
    private void Open()
    {
        serialPort_ = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
        serialPort_.Open();
 
        isRunning_ = true;
 
        thread_ = new Thread(Read);
        thread_.Start();
    }
 
    private void Read()
    {
        Debug.LogWarning("Read1");
        while (isRunning_ && serialPort_ != null && serialPort_.IsOpen)
        {
        Debug.LogWarning("Read2");
            try
            {
                message_ = serialPort_.ReadLine();
//                Debug.LogWarning(message_);
                isNewMessageReceived_ = true;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }
 
    private void Close()
    {
        isRunning_ = false;
 
        if (thread_ != null && thread_.IsAlive)
        {
            thread_.Join();
        }
 
        if (serialPort_ != null && serialPort_.IsOpen)
        {
            serialPort_.Close();
            serialPort_.Dispose();
        }
    }
 
    void OnDataReceived(string message)
    {
        Debug.LogWarning("OnDataReceived1");
        var data = message.Split(
                new string[] { "\t" }, System.StringSplitOptions.None);
        if (data.Length < 2) return;
 
        try
        {
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }
 
}