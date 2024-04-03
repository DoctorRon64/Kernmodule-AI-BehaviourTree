using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static T GetRandomElement<T>(this T[] input)
    {
        return input[Random.Range(0, input.Length)];
    }

    public static T GetRandomElement<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    public static T Choose<T>(params T[] input)
    {
        return input[Random.Range(0, input.Length)];
    }

    public static T GetLast<T>(this List<T> list)
    {
        return list[list.Count - 1];
    }
    public static T GetLast<T>(this T[] list)
    {
        return list[list.Length - 1];
    }
    public static void Shuffle<T>(this List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(0, list.Count);
            T element = list[i];
            list[i] = list[rnd];
            list[rnd] = element;
        }
    }

    public static float Remap(float value, float minOld, float maxOld, float minNew, float maxNew)
    {
        return (value - minOld) / (maxOld - minOld) * (maxNew - minNew) + minNew;
    }

    public static string PrintList<T>(this List<T> list)
    {
        string res = "{";
        for (int i = 0; i < list.Count; i++)
        {
            res += list[i].ToString();
            if(i != list.Count - 1) { res += ","; }
        }
        res += "}";
        Debug.Log(res);
        return res;
    }

    public static int ManhattanDistance(Vector2Int pos1, Vector2Int pos2)
    {
        return Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y);
    }



    /// <summary>
    /// Use this to quickly create a random color array for example
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="size"></param>
    /// <param name="pickFrom"></param>
    /// <returns></returns>
    public static T[] CreateArrayAndFillRandom<T>(int size, T[] pickFrom)
    {
        T[] array = new T[size];
        for (int i = 0; i < size; i++)
        {
            array[i] = pickFrom.GetRandomElement();
        }
        return array;
    }
}
