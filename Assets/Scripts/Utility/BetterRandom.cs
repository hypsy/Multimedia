using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterRandom
{
    private int lastValue = -1;

    public BetterRandom()
    {
        lastValue = -1;
    }

    public int RandomNoRepeat(int min = 0, int max = 1)
    {
        int value = Random.Range(min, max);
        if (lastValue == -1)
            lastValue = value;
        if (lastValue == value) {
            value = (value + 1) % max;
            lastValue = value;
        }
        return value;
    }
}
