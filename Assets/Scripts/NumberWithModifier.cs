using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

/// <summary>
/// Class used for virtually all number calculations to avoid
/// processing extremely high numbers.
/// </summary>
[Serializable]
public class NumberWithModifier {

    //The base number to display
    public float number;

    //The exponent for backing data
    public int exponent;

    //The unit associated with the exponent to display
    private static string[] m_tabUnits = new string[] { "", "K", "M", "B", "T", "aa", "bb", "cc", "dd", "ee", "ff", "gg", "hh", "ii", "jj", "kk", "ll", "mm", "nn", "oo", "pp", "qq", "rr", "ss", "tt", "uu", "vv", "ww", "xx", "yy", "zz" };

    /// <summary>
    /// Default constructor
    /// </summary>
    public NumberWithModifier()
    {
        number = 0;
        exponent = 0;
    }

    /// <summary>
    /// Parameterized Constructor
    /// </summary>
    /// <param name="num"> the base number</param>
    /// <param name="exp"> the exponent for tracking high numbers</param>
    public NumberWithModifier(float num, int exp)
    {
        number = num;
        exponent = exp;
    }
    
    /// <summary>
    /// Minus operator overload. Necessary for performing operations of high numbers
    /// </summary>
    /// <param name="num1">The number to subtract from</param>
    /// <param name="num2">The number to subtract</param>
    /// <returns>The total after subtraction</returns>
    public static NumberWithModifier operator -(NumberWithModifier num1, NumberWithModifier num2)
    {
        if (num1.exponent != num2.exponent)
        {
            int difference = num1.exponent - num2.exponent;
            while (difference > 0)
            {
                num2.number /= 1000.0f;
                num2.exponent++;
                difference--;
            }
        }
        NumberWithModifier newNum = new NumberWithModifier(num1.number - num2.number, num1.exponent);
        while (newNum.number < 1.0f && newNum.exponent > 0 )
        {
            newNum.number *= 1000.0f;
            newNum.exponent--;
        }
        return newNum;
    }

    /// <summary>
    /// To string function for displaying the number
    /// TODO: Probably shouldn't pass as a parameter
    /// </summary>
    /// <param name="num">The number to display</param>
    /// <returns></returns>
    public static string ToString(NumberWithModifier num)
    {
        string val = "";
        while (num.number < 1 && num.exponent >= 1)
        {
            num.number *= 1000;
            num.exponent -= 1;
        }
        if (num.number < 1)
        {
            val += "0";
        }

        val += num.number.ToString("##.##") + m_tabUnits[num.exponent];
        return val;
    }

    /// <summary>
    /// Greater than operator
    /// </summary>
    /// <param name="num1">The number to determine if it is greater</param>
    /// <param name="num2">The number to compare against</param>
    /// <returns></returns>
    public static bool operator >(NumberWithModifier num1, NumberWithModifier num2)
    {

        if (num1.exponent > num2.exponent)
        {
            return true;
        }
        if (num1.exponent == num2.exponent)
        {
            return num1.number > num2.number;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Greater than operator against a float
    /// </summary>
    /// <param name="num1">The number to compare</param>
    /// <param name="num2">The float to compare against</param>
    /// <returns></returns>
    public static bool operator >(NumberWithModifier num1, float num2)
    {
        int exponent = 1;
        float newNumber = num2;
        while (newNumber >= 1000.0f)
        {
            newNumber /= 1000.0f;
            exponent++;
        }
        return num1 > new NumberWithModifier(newNumber, exponent);
    }

    /// <summary>
    /// Less than operator against a float
    /// </summary>
    /// <param name="num1">The number to compare</param>
    /// <param name="num2">The float to compare against</param>
    /// <returns></returns>
    public static bool operator <(NumberWithModifier num1, float num2)
    {
        int exponent = 1;
        float newNumber = num2;
        while (newNumber >= 1000.0f)
        {
            newNumber /= 1000.0f;
            exponent++;
        }
        return num1 < new NumberWithModifier(newNumber, exponent);
    }

    /// <summary>
    /// Less than operator
    /// </summary>
    /// <param name="num1">The number to determine if it is less</param>
    /// <param name="num2">The number to compare against</param>
    /// <returns></returns>
    public static bool operator <(NumberWithModifier num1, NumberWithModifier num2)
    {

        if (num1.exponent < num2.exponent)
        {
            return true;
        }
        if (num1.exponent == num2.exponent)
        {
            return num1.number < num2.number;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Multiplication operator
    /// </summary>
    /// <param name="num1">Multiplier 1</param>
    /// <param name="num2">Multiplier 2</param>
    /// <returns></returns>
    public static NumberWithModifier operator *(NumberWithModifier num1, NumberWithModifier num2)
    {
        NumberWithModifier newNumber = new NumberWithModifier(num1.number * num2.number, num1.exponent + num2.exponent);
        while (newNumber.number >= 1000.0f)
        {
            newNumber.number /= 1000.0f;
            newNumber.exponent++;
        }
        return newNumber;
    }

    /// <summary>
    /// Multiplication operator with a float
    /// </summary>
    /// <param name="num1">Multiplier 1</param>
    /// <param name="num2">Float to multiply by</param>
    /// <returns></returns>
    public static NumberWithModifier operator *(NumberWithModifier num1, float num2)
    {
        NumberWithModifier tempNumber = new NumberWithModifier(num2, 0);
        return num1 * tempNumber;
    }

    /// <summary>
    /// Divide operator
    /// </summary>
    /// <param name="num1">Dividend</param>
    /// <param name="num2">Divisor</param>
    /// <returns>Quotient</returns>
    public static NumberWithModifier operator /(NumberWithModifier num1, NumberWithModifier num2)
    {
        NumberWithModifier newNumber = new NumberWithModifier(num1.number / num2.number, num1.exponent - num2.exponent);
        while (newNumber.number >= 1000.0f)
        {
            newNumber.number /= 1000.0f;
            newNumber.exponent++;
        }
        return newNumber;
    }

    /// <summary>
    /// Divide operator with float
    /// </summary>
    /// <param name="num1">Dividend</param>
    /// <param name="num2">Divisor</param>
    /// <returns>Quotient</returns>
    public static NumberWithModifier operator /(NumberWithModifier num1, float num2)
    {
        NumberWithModifier tempNumber = new NumberWithModifier(num2, 0);
        return num1 / tempNumber;
    }

    /// <summary>
    /// Addition operator
    /// </summary>
    /// <param name="num1">Additive 1</param>
    /// <param name="num2">Additive 2</param>
    /// <returns></returns>
    public static NumberWithModifier operator +(NumberWithModifier num1, NumberWithModifier num2)
    {
        if (num1.exponent == num2.exponent)
        {
            float newNumber = num1.number + num2.number;
            int exponent = num1.exponent;
            while (newNumber >= 1000.0f)
            {
                newNumber /= 1000;
                exponent += 1;
            }
            return new NumberWithModifier(newNumber, exponent);
        }
        else
        {
            int difference;
            float convertedNumber;
            if (num1.exponent > num2.exponent)
            {
                difference = num1.exponent - num2.exponent;
                convertedNumber = num2.number / (Mathf.Pow(1000.0f, difference));
                float newNumber = num1.number + convertedNumber;
                int exponent = num1.exponent;
                while (newNumber >= 1000.0f)
                {
                    newNumber /= 1000;
                    exponent += 1;
                }
                return new NumberWithModifier(newNumber, exponent);
            }
            else
            {
                difference = num2.exponent - num1.exponent;
                convertedNumber = num1.number / (Mathf.Pow(1000.0f, difference));
                float newNumber = num2.number + convertedNumber;
                int exponent = num2.exponent;
                while (newNumber >= 1000.0f)
                {
                    newNumber /= 1000;
                    exponent += 1;
                }
                return new NumberWithModifier(newNumber, exponent);
            }
            
        }
    }
}
