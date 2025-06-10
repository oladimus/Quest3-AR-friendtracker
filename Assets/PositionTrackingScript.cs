using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using TMPro;

public class PhonePositionReceiver : MonoBehaviour
{
    public int listenPort = 9050;
    public Transform phoneTracker; // The object that will follow the phone
    public Transform playerHead; // Quest3 position
    public TextMeshProUGUI distanceText;
    private UdpClient udpClient;
    private Thread receiveThread;
    private volatile bool running = true;

    private Vector3 latestPosition = Vector3.zero;
    private Quaternion latestRotation = Quaternion.identity;

    void Start()
    {
        if (playerHead == null && Camera.main != null)
        {
            playerHead = Camera.main.transform;
        }

        udpClient = new UdpClient(listenPort);
        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void Update()
    {
        if (phoneTracker != null)
        {
            phoneTracker.position = latestPosition;
            phoneTracker.rotation = latestRotation;
        }

        if (phoneTracker != null && playerHead != null)
        {
            float distance = Vector3.Distance(playerHead.position, phoneTracker.position);
            distanceText.text = $"Distance to Friend: {distance:F2} m";
        }

    }

    void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
        while (running)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string json = Encoding.UTF8.GetString(data);
                PositionData posData = JsonUtility.FromJson<PositionData>(json);
                Debug.Log(json);

                latestPosition = new Vector3(posData.x, posData.y, posData.z);
                latestRotation = new Quaternion(posData.qx, posData.qy, posData.qz, posData.qw);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("UDP Receive error: " + ex.Message);
            }
        }
    }

    void OnApplicationQuit()
    {
        running = false;
        receiveThread?.Abort();
        udpClient?.Close();
    }

    [System.Serializable]
    class PositionData
    {
        public float x, y, z;
        public float qx, qy, qz, qw;
    }
}
