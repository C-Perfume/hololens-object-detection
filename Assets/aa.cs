using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


//public class A
//{
//    public string id;
//    public string project;
//    public string iteration;
//    public string created;
//    public List<TL> predictions;
//}

//public class Serial<TL>
//{
//    public List<TL> predictions;
//    public List<TL> ToList() { return predictions; }
//    public Serial(List<TL> predictions) { this.predictions = predictions; }

// }

//public class TL
//{
//    public double probability;
//    public string tagId;
//    public string tagName;
//    public T boundingBox;
//    public string tagType;
//    public TL(double pro, string id, string name, T bb, string type) 
//    {   this.probability = pro;
//        this.tagId = id;
//        this.tagName = name;
//        this.boundingBox = bb;
//        this.tagType = type;
//    }
//}

//public class Wrap<T>
//{
//  public T[] boundingBox;
//  public T[] ToArray() { return boundingBox; }
//  public Wrap(T[] boundingBox) { this.boundingBox = boundingBox; }
//}
//public class T
//{
//    public double left;
//    public double top;
//    public double width;
//    public double height;

//    public T(double left, double top, double width, double height)
//    {   this.left = left;
//        this.top = top;
//        this.width = width;
//        this.height = height;
//    }
//}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }
        
    [Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}

[Serializable]
public class PlayerInfo
{
    public string name;
    public int lives;
    public float strength;
}

public class aa : MonoBehaviour
{

    int cnt = 0;

    //public void ShowJson(string path)
    //{
    //    string json = ReadJson(path);
    //    if (json != "nothing")
    //    {

    //        var testList = new List<TL>();
    //            JsonUtility.FromJson<Serial<TL>>(json).ToList();
    //        print($"Cnt = {testList.Count}");

    //    }
    //    else
    //    {
    //        print(json);
    //    }
    //}

    //void WriteText(string json, string path)
    //{
    //    if (File.Exists(path)) File.Delete(path);
    //    File.WriteAllText(path, json);
    //    cnt++;
    //}

    //string ReadJson(string filePath)
    //{
    //    string json = "nothing";

    //    if (File.Exists(filePath))
    //    {
    //        StreamReader reader = new StreamReader(filePath);
    //        json = reader.ReadToEnd();
    //        reader.Close();
    //    }

    //    return json;
    //}

    void Start()
    {
        var filename = string.Format(@"JSON{0}.txt", cnt);
        var filePath = Path.Combine(Application.persistentDataPath, filename);

        //    //ShowJson(filePath);
        //    T[] Bb = new T[1];
        //    var bb = new T(1, 2, 3, 4);
        //    Bb[0] = bb;
        //    string strbb = JsonUtility.ToJson(bb);
        //    string strBb = JsonUtility.ToJson(new Wrap<T>(Bb));
        //    print(strbb);
        //    print(strBb);

        //    T[] testbb = JsonUtility.FromJson<Wrap<T>>(strBb).ToArray();

        //    var t1 = new TL(0, "no.0", "0번", testbb[0], "N");
        //    var t2 = new TL(1, "no.1", "1번", bb, "Y");

        //    var pre = new List<TL>();
        //    pre.Add(t1);
        //    pre.Add(t2);

        //    string strT1 = JsonUtility.ToJson(t1);
        //    string strPre = JsonUtility.ToJson(pre);
        //    string strSerialPre = JsonUtility.ToJson(new Serial<TL>(pre));

        //    print(strT1);
        //    print(strPre);
        //    print(strSerialPre);

        PlayerInfo[] playerInfo = new PlayerInfo[1];

        playerInfo[0] = new PlayerInfo();
        playerInfo[0].name = "12341234";
        playerInfo[0].lives = 5;
        playerInfo[0].strength = 25.0f;

        string toJson = JsonHelper.ToJson(playerInfo, prettyPrint: true);
        print(toJson);

    }

}
