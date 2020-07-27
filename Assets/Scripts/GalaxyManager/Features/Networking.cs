using System;
using UnityEngine;
using Galaxy.Api;
using System.Collections;
using Helpers;

public class Networking : MonoBehaviour
{
    #region Variables

    // PACKETS
    // Variables for storing packets
    private GameObject[] balls;
    private GalaxyID receiverID = null;
    private byte[] switchPacketBuffer = new byte[12];
    private byte[] ballPacketBuffer = new byte[198];
    // Networking listeners
    private NetworkingListener networkListener;

    private IEnumerator sendPacketCoroutine;
    public float waitTime=.015f;

    #endregion

    #region Behaviors

    void OnEnable()
    {
        ListenersInit();
        balls = GameObject.Find("PoolBalls").GetComponent<PoolBalls>().poolBalls;

        if (receiverID == null)
        {
            receiverID = GalaxyManager.Instance.Matchmaking.GetSecondPlayerID();
            Debug.Log("receiverID was null. ReceiverID set to: " + receiverID);
        }
        sendPacketCoroutine = SendPacketCoroutine(waitTime);
        StartCoroutine(sendPacketCoroutine);
    }


    void OnDisable()
    {
        StopCoroutine(sendPacketCoroutine);
        ListenersDispose();
    }

    #endregion

    #region Listeners methods

    // Istantiantes listeners
    private void ListenersInit()
    {
        Listener.Create(ref networkListener);
    }

    // Disposes listeners
    private void ListenersDispose()
    {
        Listener.Dispose(ref networkListener);
    }

    #endregion

    #region Methods

    /* Sends a byte array packet to a user or lobby specified by GalaxyID
    Note: we use a P2P connection in our game */
    public void SendP2PPacket(GalaxyID galaxyID, byte[] data, P2PSendType sendType = P2PSendType.P2P_SEND_RELIABLE, byte channel = 0)
    {
        uint dataSize = (uint)data.Length;
        try
        {
            GalaxyInstance.Networking().SendP2PPacket(galaxyID, data, dataSize, sendType, channel);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogError("Couldn't send packet for reason: " + e);
        }
    }

    // Prepares a byte array with information on players shot and sends it using SendP2PPacket method
    public void SendPacketWithPlayerShot(GameManager.FoulEnum foul, GameManager.BallColorEnum ballOn, int shotScore)
    {
        Debug.Log("Packet switch sent");
        
        SendPacketWithBallPositions();
        GetBytesFromInt((int)foul, 0, ref switchPacketBuffer);
        GetBytesFromInt((int)ballOn, 4, ref switchPacketBuffer);
        GetBytesFromInt(shotScore, 8, ref switchPacketBuffer);
        SendP2PPacket(receiverID, switchPacketBuffer, P2PSendType.P2P_SEND_RELIABLE, (byte)1);
    }

    // Prepares a byte array with information on balls positions and status and sends it using SendP2PPacket method
    void SendPacketWithBallPositions()
    {
        Debug.Log("Packet position sent");

        int startIndex = 0;
        for (int i = 0; i < balls.Length; i++)
        {
            startIndex = i * 9;
            GetByteFromBool(balls[i].activeInHierarchy, startIndex, ref ballPacketBuffer);
            GetBytesFromFloat(balls[i].GetComponent<Rigidbody>().position.x, startIndex + 1, ref ballPacketBuffer);
            GetBytesFromFloat(balls[i].GetComponent<Rigidbody>().position.z, startIndex + 5, ref ballPacketBuffer);
        }
        SendP2PPacket(receiverID, ballPacketBuffer);
    }

    // Reads a received packet with information on players shot
    void ReadPacketWithPlayerShot(byte[] switchPacketReceived)
    {
        Debug.Log("Packet switch received");

        GameManager.FoulEnum foul;
        GameManager.BallColorEnum ballOn;
        int shotScore;

        if (switchPacketReceived != null)
        {
            foul = (GameManager.FoulEnum)GetIntFromBytes(0, ref switchPacketReceived);
            ballOn = (GameManager.BallColorEnum)GetIntFromBytes(4, ref switchPacketReceived);
            shotScore = GetIntFromBytes(8, ref switchPacketReceived);

            GameManager.Instance.foul = foul;
            GameManager.Instance.ballOn = ballOn;
            GameManager.Instance.shotScore = shotScore;

            GameManager.Instance.ShotShort();
        }
    }

    // Reads a received packet with information balls positions
    void ReadPacketWithBallPositions(byte[] positionPacketReceived)
    {
        Debug.Log("Packet position received");

        int startIndex = 0;
        float x;
        float z;
        bool active;
        if (positionPacketReceived != null)
        {
            for (int i = 0; i < balls.Length; i++)
            {
                startIndex = i * 9;
                active = GetBoolFromByte(startIndex, ref positionPacketReceived);
                x = GetFloatFromBytes(startIndex + 1, ref positionPacketReceived);
                z = GetFloatFromBytes(startIndex + 5, ref positionPacketReceived);
                balls[i].SetActive(active);
                balls[i].GetComponent<Rigidbody>().MovePosition(new Vector3(x, balls[i].GetComponent<Rigidbody>().position.y, z));
            }
        }
    }

    #endregion

    #region Listeners

    /* Gets callbacks when a P2P packet is received
    Note: We use channel 0 for sending balls positions and channel 1 for sending player shot stats */
    private class NetworkingListener : GlobalNetworkingListener
    {
        GalaxyID senderID = new GalaxyID();
        byte[] packetReceived;
        public override void OnP2PPacketAvailable(uint msgSize, byte channel)
        {
            packetReceived = new byte[msgSize];
            if (GalaxyInstance.Networking().PeekP2PPacket(packetReceived, msgSize, ref msgSize, ref senderID, channel))
            {
                switch (channel)
                {
                    case (byte)0:
                        GalaxyManager.Instance.Networking.ReadPacketWithBallPositions(packetReceived);
                        break;
                    case (byte)1:
                        GalaxyManager.Instance.Networking.ReadPacketWithPlayerShot(packetReceived);
                        break;
                }
            }
        }
    }

    #endregion

    #region Helpers

    // Helper function for converting float to byte array
    void GetBytesFromFloat(float position, int index, ref byte[] byteArray)
    {
        byte[] bytesFromPosition;
        bytesFromPosition = BitConverter.GetBytes(position);
        for (int i = 0; i < bytesFromPosition.Length; i++)
        {
            byteArray[index + i] = bytesFromPosition[i];
        }
    }

    // Helper function for converting integer to byte array
    void GetBytesFromInt(int score, int index, ref byte[] byteArray)
    {
        byte[] bytesFromScore;
        bytesFromScore = BitConverter.GetBytes(score);
        for (int i = 0; i < bytesFromScore.Length; i++)
        {
            byteArray[index + i] = bytesFromScore[i];
        }
    }

    // Helper function for converting boolean to byte array
    void GetByteFromBool(bool active, int index, ref byte[] byteArray)
    {
        byte byteFromActive;
        byteFromActive = Convert.ToByte(active);
        byteArray[index] = byteFromActive;
    }

    // Helper function for converting byte array to float 
    float GetFloatFromBytes(int index, ref byte[] byteArray)
    {
        float position;
        position = BitConverter.ToSingle(byteArray, index);
        return position;
    }

    // Helper function for converting byte array to integer
    int GetIntFromBytes(int index, ref byte[] byteArray)
    {
        int score;
        score = BitConverter.ToInt32(byteArray, index);
        return score;
    }

    // Helper function for converting byte array to boolean
    bool GetBoolFromByte(int index, ref byte[] byteArray)
    {
        bool active;
        active = Convert.ToBoolean(byteArray[index]);
        return active;
    }

    // Coroutine for sending P2P packets
    IEnumerator SendPacketCoroutine(float waitTime)
    {
        for (;;)
        {
            if (((Online2PlayerGameManager)GameManager.Instance).me.Active)
            {
                SendPacketWithBallPositions();
            }
            yield return new WaitForSecondsRealtime(waitTime);
        }
    }

    #endregion

}