package com.example.avbal.myapplication.Services;

import android.net.wifi.WifiManager;

import com.example.avbal.myapplication.Main.ApplicationContext;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.util.Observable;

import static com.example.avbal.myapplication.Utility.getBroadcastAddress;
import static com.example.avbal.myapplication.Utility.getIPAddress;

public class DiscoveryService extends Observable
{
    public static final int DATAPORT = 8888;
    public static final int DISCOVERYPORT = 8887;
    public static final String WHERE_ARE_YOU = "WHERE ARE YOU";

    private WifiManager wifiManager;
    private String status = ApplicationContext.StateNone;

    private void UpdateStatus(String newStatus)
    {
        if (!status.equals(newStatus))
        {
            status = newStatus;
            setChanged();
            notifyObservers(status);
        }
    }

    public DiscoveryService()
    {
        this.wifiManager = ApplicationContext.Instance.getWifiManager();
    }

    public String getStatus()
    {
        return status;
    }

    public void Shutdown()
    {
        status = ApplicationContext.StateNone;
    }

    public void Launch()
    {
        Thread thread = new Thread(new Runnable()
        {
            public void run()
            {
                try
                {
                    UpdateStatus(ApplicationContext.StateOnline);

                    InetAddress broadcastAddress = getBroadcastAddress(wifiManager);

                    byte[] dataBuffer = new byte[15000];
                    DatagramPacket packet = new DatagramPacket(dataBuffer, dataBuffer.length);
                    DatagramSocket socket = new DatagramSocket(null);
                    socket.bind(new InetSocketAddress(broadcastAddress, DISCOVERYPORT));
                    socket.setBroadcast(true);
                    socket.setSoTimeout(ApplicationContext.Timeout);

                    UpdateStatus(ApplicationContext.StateOnline);
                    do
                    {
                        try
                        {
                            socket.receive(packet);
                            //Packet received
                            String message = new String(packet.getData()).trim();
                            ProcessReceivedData(message);
                        }
                        catch (IOException e)
                        {
                            System.out.println(e.getMessage());
                        }
                    }
                    while(!status.equals(ApplicationContext.StateNone));
                    socket.close();
                    UpdateStatus(ApplicationContext.StateNone);
                }
                catch (IOException e)
                {
                    e.printStackTrace();
                }

                UpdateStatus(ApplicationContext.StateNone);
            }
        });

        thread.start();
    }

    private void ProcessReceivedData(String data)
    {
        if (WHERE_ARE_YOU.equals(data))
        {
            SendBroadcastMessage();
        }
    }

    private void SendBroadcastMessage()
    {
        Thread thread = new Thread(new Runnable()
        {
            public void run()
            {
                String localAddress = getIPAddress(true) + ":" + DATAPORT;
                try
                {
                    byte[] msgOut = localAddress.getBytes();
                    InetAddress broadcastAddress = getBroadcastAddress(wifiManager);

                    DatagramPacket dp = new DatagramPacket(msgOut, msgOut.length, broadcastAddress, DATAPORT);
                    DatagramSocket ds = new DatagramSocket();
                    ds.setBroadcast(true);
                    ds.send(dp);
                    ds.close();
                }
                catch (IOException e)
                {
                    e.printStackTrace();
                }
            }
        });

        thread.start();
    }
}
