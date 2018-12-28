package com.example.avbal.myapplication.Sensors;

import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;

public class SensorListener  implements SensorEventListener
{
    private final int magnitude = 10;
    private final int precision = 3;
    private final int scale;
    private final Sensor sensor;
    private final int sensorType;
    private final SensorDataHandler dataHandler;

    public SensorListener(Sensor sensor, SensorDataHandler dataHandler)
    {
        this.sensor = sensor;
        this.sensorType = sensor.getType();
        this.dataHandler = dataHandler;

        this.scale = (int) Math.pow(magnitude, precision);
    }

    public Sensor getSensor()
    {
        return sensor;
    }

    @Override
    public void onSensorChanged(SensorEvent event)
    {
        if (event == null || event.sensor.getType() != sensorType)
            return;

        float x = round(event.values[0], scale);
        float y = round(event.values[1], scale);
        float z = round(event.values[2], scale);

        SensorData sensorData = new SensorData(new float[] {x, y, z}, event.values.length, event.timestamp);
        dataHandler.OnSensorDataReceived(sensorData);
    }

    private static float round (float value, int scale)
    {
        return (float) Math.round(value * scale) / scale;
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy)
    {
    }
}
