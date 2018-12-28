package com.example.avbal.myapplication.Sensors;

import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

public class SensorListener  implements SensorEventListener
{
    private final int magnitude = 10;
    private final int precision = 3;
    private final int scale;
    private final Sensor sensor;
    private final int sensorType;
    private final SensorDataHandler dataHandler;
    //
    private final List<SensorData> accumulator;
    private long timeStamp = 0;
    private SensorData maximum;
    private SensorData minimum;
    private float lastValue = 0;

    public SensorListener(Sensor sensor, SensorDataHandler dataHandler)
    {
        this.sensor = sensor;
        this.sensorType = sensor.getType();
        this.dataHandler = dataHandler;
        this.accumulator = new ArrayList<>();

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
        if (maximum == null && accumulator.size() < 50)
        {
            SensorData sensorData = new SensorData(new float[]{x, y, z},
                                                   event.values.length,
                                                   event.timestamp);
            accumulator.add(sensorData);
            if (accumulator.size() == 50)
            {
                System.out.println(String.format("size %d", accumulator.size()));
                Collections.sort(accumulator, new ValueComparator(1));
                maximum = accumulator.get(0);
                minimum = accumulator.get(accumulator.size() - 1);
                accumulator.clear();

                System.out.println(String.format("Min %f Max %f", minimum.values[0], maximum.values[0]));
            }
            return;
        }

        if (x > -0.3 && x < -0.01)
            x = 0;

        if (lastValue == x)
            return;
        
        SensorData sensorData = new SensorData(new float[]{x, y, z}, event.values.length, event.timestamp);
        dataHandler.OnSensorDataReceived(sensorData);
    }

    public float valueXFilter()
    {
        return 0.0f;
    }

    private static float round (float value, int scale)
    {
        return (float) Math.round(value * scale) / scale;
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy)
    {
    }

    // compares object using x value of event
    class ValueComparator implements Comparator<SensorData>
    {
        private final int index;
        public ValueComparator(int index)
        {
            this.index = index;
        }

        @Override
        public int compare(SensorData o1, SensorData o2)
        {
            Float value1 = o1.values[index];
            return value1.compareTo(o2.values[index]);
        }
    }
}
