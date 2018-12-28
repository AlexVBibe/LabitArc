package com.example.avbal.myapplication.Services;

import com.example.avbal.myapplication.Client.ClientContext;
import com.example.avbal.myapplication.Main.ApplicationContext;
import com.example.avbal.myapplication.Main.EventHandler;
import com.example.avbal.myapplication.Sensors.SensorData;
import com.example.avbal.myapplication.Sensors.SensorDataHandler;

import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.Observable;

public class GadgetService extends Observable implements SensorDataHandler
{
    private ServerSocket serverSocket;
    private Socket clientSocket;
    private ClientContext clientContext;
    private Thread serverThread = null;
    private String status = ApplicationContext.StateNone;

    public String getStatus()
    {
        return status;
    }

    public ClientContext getClientContext()
    {
        return clientContext;
    }

    @Override
    public void OnSensorDataReceived(SensorData sensorData)
    {
        SendMessage(sensorData.toString());
    }

    public void Launch()
    {
        this.serverThread = new Thread(new ServerThread());
        this.serverThread.start();
    }

    private void UpdateStatus(String newStatus)
    {
        if (!status.equals(newStatus))
        {
            status = newStatus;
            setChanged();
            notifyObservers(status);
        }
    }

    private void SendMessage(String message)
    {
        clientContext.SendMessage(message);
    }

    ///
    ///
    ///
    class ServerThread implements Runnable, EventHandler
    {
        public void run()
        {
            try
            {
                serverSocket = new ServerSocket(DiscoveryService.DATAPORT);
                serverSocket.setSoTimeout(ApplicationContext.Timeout);
                UpdateStatus(ApplicationContext.StateAvailable);

                while (!Thread.currentThread().isInterrupted() && clientContext == null)
                {
                    try
                    {
                        clientSocket = serverSocket.accept();
                        clientContext = new ClientContext(clientSocket, this);

                        UpdateStatus(ApplicationContext.StateConnected);
                    }
                    catch (IOException e)
                    {
                        clientContext = null;
                    }
                }

                UpdateStatus(ApplicationContext.StateConnected);
            }
            catch (IOException e)
            {
                e.printStackTrace();
            }
        }

        @Override
        public void handleEvent(Object args)
        {
            try
            {
                serverSocket.close();
            }
            catch (IOException e)
            {
                e.printStackTrace();
            }

            clientContext = null;
            UpdateStatus(ApplicationContext.StateDisconnected);
        }
    }
}
