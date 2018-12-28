package com.example.avbal.myapplication.Client;

import com.example.avbal.myapplication.Controllers.SensorMonitor;
import com.example.avbal.myapplication.Main.ApplicationContext;
import com.example.avbal.myapplication.Main.EventHandler;
import com.example.avbal.myapplication.Sensors.SensorInfo;

import java.io.IOException;
import java.io.InputStream;
import java.net.Socket;
import java.util.List;

class ClientInThread implements Runnable
{
    private final ClientContext clientContext;
    private final Socket clientSocket;
    private final EventHandler eventHandler;

    public ClientInThread(Socket clientSocket, ClientContext clientContext, EventHandler eventHandler)
    {
        this.clientSocket = clientSocket;
        this.eventHandler = eventHandler;
        this.clientContext = clientContext;
    }

    public void run()
    {
        InputStream inputStream = null;
        try
        {
            inputStream = clientSocket.getInputStream();
            while (!Thread.currentThread().isInterrupted())
            {
                byte[] dataBytes = new byte[1024];
                int bytesRead = inputStream.read(dataBytes, 0, 1024);
                if (bytesRead > 0)
                {
                    String message = new String(dataBytes, 0, bytesRead);
                    while (inputStream.available() > 0)
                    {
                        bytesRead = inputStream.read(dataBytes, 0, 1024);
                        message += new String(dataBytes, 0, bytesRead);
                    }
                    ProcessReceivedMessage(message);
                }
            }

            clientSocket.close();
        }
        catch (IOException e)
        {
            e.printStackTrace();
        }

        if (eventHandler != null)
            eventHandler.handleEvent(ApplicationContext.StateDisconnected);
    }

    private void ProcessReceivedMessage(String message)
    {
        //TODO: parce commands
        String command = message.toUpperCase();
        switch (message.toUpperCase())
        {
            case "HELLO":
                SendMessage("Agent Service v.0.0.1");
                break;
            case "MODES":
                ProcessCommand("MODES", null);
                break;
            case "PAUSE":
                break;
            default:
                if (command.startsWith("SENSORS"))
                {
                    String delimiter = " ";
                    String[] bits = command.split(delimiter);
                    ProcessCommand(bits[0], bits);
                }
                break;
        }
    }

    private void ProcessCommand(String command, String[] args)
    {
        SensorMonitor monitor = clientContext.getSensorMonitor();
        if (command == "MODES")
        {
            List<SensorInfo> supportedSensors = monitor.SupportedSensors();
            StringBuilder sb = new StringBuilder();
            sb.append("<modes>");
            for(SensorInfo sensorInfo : supportedSensors)
            {
                String name = sensorInfo.getName();
                int type = sensorInfo.getType();
                String lineOfMessage = String.format("%s;%d", name, type);
                sb.append(lineOfMessage);
                sb.append("\r\n" );
            }
            sb.append("</modes>");

            String outMessage = sb.toString();
            System.out.println(outMessage);
            SendMessage(outMessage);
        }
        else if (command.equals("SENSORS"))
        {
            String action = args[1];
            for(int i = 2; i < args.length; i++)
            {
                int sensorType = Integer.parseInt(args[i]);
                if (action.equals("A"))
                    monitor.addAndRunSensor(sensorType, clientContext);
                else if (action.equals("R"))
                {
                    monitor.removeAndStopSensor(sensorType);
                }
            }
        }
    }

    private void SendMessage(String message)
    {
        ClientOutThread commThread = new ClientOutThread(clientSocket, message);
        new Thread(commThread).start();
    }
}