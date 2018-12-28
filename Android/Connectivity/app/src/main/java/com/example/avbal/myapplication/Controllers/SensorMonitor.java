package com.example.avbal.myapplication.Controllers;

import android.app.Activity;
import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorManager;

import com.example.avbal.myapplication.Sensors.SensorDataHandler;
import com.example.avbal.myapplication.Sensors.SensorInfo;
import com.example.avbal.myapplication.Sensors.SensorListener;

import java.util.ArrayList;
import java.util.List;

import static android.hardware.SensorManager.SENSOR_DELAY_NORMAL;

public class SensorMonitor
{
    // supported sensors list, think about SOLID implementation
    private final int[] supportedSensors = new int[] {1, 2, 14, 4, 16, 9, 10, 11, 20, 3, 22, 15};
    private final SensorManager sensorManager;
    private final List<SensorListener> sensorListeners;

    public SensorMonitor(Activity activity)
    {
        sensorListeners = new ArrayList<SensorListener>();
        sensorManager = (SensorManager)activity.getSystemService(Context.SENSOR_SERVICE);
    }

    public void addAndRunSensor(int type, SensorDataHandler handler)
    {
        Sensor sensor = sensorManager.getDefaultSensor(type);
        if (sensor != null)
        {
            SensorListener sensorListener = new SensorListener(sensor, handler);
            sensorListeners.add(sensorListener);
            sensorManager.registerListener(sensorListener, sensor, SENSOR_DELAY_NORMAL);
        }
    }

    public void removeAndStopAllSensors()
    {
        for(SensorListener listener : sensorListeners)
        {
            sensorManager.unregisterListener(listener, listener.getSensor());
        }
        sensorListeners.clear();
    }

    public void removeAndStopSensor(int type)
    {
        for(SensorListener listener : sensorListeners)
        {
            if (listener.getSensor().getType() == type)
            {
                sensorManager.unregisterListener(listener, listener.getSensor());
                sensorListeners.remove(listener);
                break;
            }
        }
    }

    public List<SensorInfo> SupportedSensors()
    {
        List<SensorInfo> result = new ArrayList<SensorInfo>();
        List<Sensor> sensors = sensorManager.getSensorList(Sensor.TYPE_ALL);
        for (Sensor sensor: sensors)
        {
            int sensorType = sensor.getType();
            if (IsSensorSupported(sensorType))
            {
                String sensorName = sensor.getName();
                if (!sensorName.toUpperCase().contains("-WAKEUP"))
                {
                    result.add(new SensorInfo(sensorName, sensorType));
                }
            }
        }
        return result;
    }

    private Boolean IsSensorSupported(int sensorType)
    {
        for(int type : supportedSensors)
            if (type == sensorType)
                return true;
        return false;
    }
}
