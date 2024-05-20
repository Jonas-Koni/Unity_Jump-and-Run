using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


//!!! system.random and unityengine.random are mixed in workspace!
//RandomPolynomialSpreadNumberGenerator
public class RandomPolynomialSpreadNumber
{
    public static float GetRandomNumber(float exponentAmplifier, float startValue, float endValue)
    {
        if(startValue >= endValue)
        {
            throw new InvalidDataException("startValue is greater than or equal to endValue!");
        }

        if(exponentAmplifier == 0)
        {
            throw new InvalidOperationException("Exponent is one! Use RandomNumberType.ConstantSpread instead!");
        }

        float distanceNumbers = endValue - startValue;
        float functionGenerateRandomNumber =
                Mathf.Pow(2, 2 * exponentAmplifier)
                * Mathf.Pow(
                    UnityEngine.Random.value - 0.5f,
                    2 * exponentAmplifier + 1)
                + 0.5f;
        float resultWithRange = functionGenerateRandomNumber * distanceNumbers + startValue;
        return resultWithRange;

    }

}


public class RandomConstantSpreadNumber
{
    public static float GetRandomNumber(float startValue, float endValue)
    {
        if (startValue >= endValue)
        {
            throw new InvalidDataException("startValue is greater than or equal to endValue!");
        }

        float distanceNumbers = Mathf.Abs(endValue - startValue);
        float result = UnityEngine.Random.value * distanceNumbers + startValue;
        return result;
    }
}
