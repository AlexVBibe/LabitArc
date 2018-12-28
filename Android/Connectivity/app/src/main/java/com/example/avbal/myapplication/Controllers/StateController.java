package com.example.avbal.myapplication.Controllers;

import com.example.avbal.myapplication.Client.ClientContext;
import com.example.avbal.myapplication.Main.ApplicationContext;
import com.example.avbal.myapplication.Services.DiscoveryService;
import com.example.avbal.myapplication.Services.GadgetService;

import java.util.Observable;
import java.util.Observer;

public class StateController implements Observer
{
    private DiscoveryService discoveryService;
    private GadgetService gadgetService;
    private ClientContext clientContext;

    public StateController()
    {
        gadgetService = new GadgetService();
        discoveryService = new DiscoveryService();

        update(gadgetService, ApplicationContext.StateNone);
        update(discoveryService, ApplicationContext.StateNone);

        discoveryService.addObserver(this);
        gadgetService.addObserver(this);
    }

    public void shutDown()
    {
        if (clientContext != null)
            clientContext.getSensorMonitor().removeAndStopAllSensors();
    }

    @Override
    public void update(Observable o, Object arg)
    {
        if (o instanceof DiscoveryService )
        {
            String status = (String)arg;
            switch (status)
            {
                case ApplicationContext.StateOnline:
                    break;
                case ApplicationContext.StatePairing:
                    break;
                case ApplicationContext.StateNone:
                    break;
            }
        }
        else if (o instanceof GadgetService )
        {
            String status = (String)arg;
            switch (status)
            {
                case ApplicationContext.StateConnected:
                    clientContext = gadgetService.getClientContext();
                    discoveryService.Shutdown();
                    break;
                case ApplicationContext.StateAvailable:
                    break;
                case ApplicationContext.StateDisconnected:
                case ApplicationContext.StateNone:
                    if (clientContext != null)
                        clientContext = null;
                    gadgetService.Launch();
                    break;
            }

            if (!gadgetService.getStatus().equals(ApplicationContext.StateConnected)
                    && discoveryService.getStatus().equals(ApplicationContext.StateNone))
            {
                discoveryService.Launch();
            }
        }
    }
}
