package com.example.avbal.myapplication.Client;

import com.example.avbal.myapplication.Controllers.SensorMonitor;
import com.example.avbal.myapplication.Main.ApplicationContext;
import com.example.avbal.myapplication.Main.EventHandler;
import com.example.avbal.myapplication.Sensors.SensorData;
import com.example.avbal.myapplication.Sensors.SensorDataHandler;

import java.net.Socket;

public class ClientContext implements SensorDataHandler
{
    private final Socket clientSocket;
    private final SensorMonitor sensorMonitor;

    public ClientContext(Socket clientSocket, EventHandler handler)
    {
        this.clientSocket = clientSocket;
        this.sensorMonitor = new SensorMonitor(ApplicationContext.Instance.MainActivity);

        ClientInThread commThread = new ClientInThread(clientSocket, this, handler);
        new Thread(commThread).start();
    }

    public SensorMonitor getSensorMonitor()
    {
        return sensorMonitor;
    }

    public void SendMessage(String message)
    {
        ClientOutThread commThread = new ClientOutThread(clientSocket, message);
        new Thread(commThread).start();
    }

    @Override
    public void OnSensorDataReceived(SensorData sensorData)
    {
        SendMessage(sensorData.toString());
    }
}
