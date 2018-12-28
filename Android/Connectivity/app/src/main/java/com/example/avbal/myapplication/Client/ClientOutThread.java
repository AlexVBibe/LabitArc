package com.example.avbal.myapplication.Client;

import java.io.IOException;
import java.io.OutputStream;
import java.io.PrintStream;
import java.net.Socket;

class ClientOutThread implements Runnable
{
    private Socket clientSocket;
    private String message;

    public ClientOutThread(Socket clientSocket, String message)
    {
        this.clientSocket = clientSocket;
        this.message = message;
    }

    public void run()
    {
        OutputStream outputStream = null;
        try
        {
            outputStream = clientSocket.getOutputStream();
            PrintStream printStream = new PrintStream(outputStream);
            printStream.print(message);
        } catch (IOException e)
        {
            e.printStackTrace();
        }
    }
}
