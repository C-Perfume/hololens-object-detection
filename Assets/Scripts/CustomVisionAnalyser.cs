//using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityEngine.Satbyul
{
    #region JSON test
    public class Analy<Pre>
    {
        public string id; 
        public string project;
        public string iteration;
        public string created;
        List<Pre> predictions;
        public List<Pre> ToPre() { return predictions; }
        public Analy(List<Pre> predictions) { this.predictions = predictions; }
    }

    public class Pre
    {
        public double probability;
        public string tagId;
        public string tagName;
        public BBox boundingBox;
        public string tagType;
    }

    public class BBox
    {
        public double left;
        public double top;
        public double width;
        public double height;
    }

    //public class test
    //{
    //    public int num;
    //    public int idx;
    //    public string name;
    //    public int pw;
    //}

    #endregion

    public class CustomVisionAnalyser : MonoBehaviour
    {
        /// <summary>
        /// Unique instance of this class
        /// </summary>
        public static CustomVisionAnalyser Instance;

        /// <summary>
        /// Insert your prediction key here
        /// </summary>
        private string predictionKey = "85e5ecaeee8c4a19ba3c6bb818295118";

        /// <summary>
        /// Insert your prediction endpoint here
        /// </summary>
        private string predictionEndpoint = "https://sblee1-prediction.cognitiveservices.azure.com/customvision/v3.1/Prediction/1e48f432-7a20-4e29-8bfa-cadfac02a8b4/detect/iterations/Iteration1/image";
        /// <summary>
        /// Bite array of the image to submit for analysis
        /// </summary>
        [HideInInspector] public byte[] imageBytes;

        #region JSON test methods
        List<Pre> pres = new List<Pre>();
        //int cnt = 0;
        //public void ShowJson()
        //{
        //    var filename = string.Format(@"JSON{0}.txt", cnt);
        //    var filePath = Path.Combine(Application.persistentDataPath, filename);

        //    Analy a = new Analy();
        //    string json = ReadJson(filePath);
        //    if (json != "nothing")
        //    {
        //        a = JsonUtility.FromJson<Analy>(json);
        //        Logger.Log("id = " + a.id);
        //    }
        //    else
        //    {
        //        Logger.Log(json);
        //    }
        //    //SceneOrganiser.Instance.FinaliseLabel(test);
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

        //void Start()
        //{
        //    test a = new test();
        //    a.num = 1;
        //    a.idx = 2;
        //    a.name = "test";
        //    a.pw = 1234;

        //    string atxt = JsonUtility.ToJson(a);
        //    Logger.Log($"a = {atxt}");

        //    test b = new test();
        //    b = JsonUtility.FromJson<test>(atxt);
        //    Logger.Log($"b = {b.num}, {b.idx}, {b.name}, {b.pw}");
        //}

        #endregion

        private void Awake()
        {
            // Allows this instance to behave like a singleton
            Instance = this;
        }

        /// <summary>
        /// Call the Computer Vision Service to submit the image.
        /// </summary>
        /// 
        public IEnumerator AnalyseLastImageCaptured(string imagePath)
        {
            WWWForm webForm = new WWWForm();

            using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(predictionEndpoint, webForm))
            {
                // Gets a byte array out of the saved image
                imageBytes = GetImageAsByteArray(imagePath);

                unityWebRequest.SetRequestHeader("Content-Type", "application/octet-stream");
                unityWebRequest.SetRequestHeader("Prediction-Key", predictionKey);

                // The upload handler will help uploading the byte array with the request
                unityWebRequest.uploadHandler = new UploadHandlerRaw(imageBytes);
                unityWebRequest.uploadHandler.contentType = "application/octet-stream";

                // The download handler will help receiving the analysis from Azure
                unityWebRequest.downloadHandler = new DownloadHandlerBuffer();

                // Send the request
                yield return unityWebRequest.SendWebRequest();

                string jsonResponse = unityWebRequest.downloadHandler.text;

                // Create a texture. Texture size does not matter, since
                // LoadImage will replace with the incoming image size.
                Texture2D tex = new Texture2D(1, 1);
                tex.LoadImage(imageBytes);
                SceneOrganiser.Instance.quadRenderer.material.SetTexture("_MainTex", tex);

                //int preidx = jsonResponse.IndexOf("predictions");
                //int regularidx = jsonResponse.IndexOf("Regular");
                //Logger.Log($"{jsonResponse}".Substring(preidx - 1, regularidx + 10 - preidx).Replace(",", "\n"));

                //AnalysisRootObject analysisRootObject = JsonConvert.DeserializeObject<AnalysisRootObject>(jsonResponse);

                Analy<Pre> ana = JsonUtility.FromJson<Analy<Pre>>(jsonResponse);
                pres = ana.ToPre();
                if (pres.Count >= 0) Logger.Log($"Pre_Cnt = {pres.Count}");
                else Logger.Log("smaller tnan 0");

                //SceneOrganiser.Instance.FinaliseLabel(analysisRootObject);
            }
        }

        /// <summary>
        /// Returns the contents of the specified image file as a byte array.
        /// </summary>
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);

            BinaryReader binaryReader = new BinaryReader(fileStream);

            return binaryReader.ReadBytes((int)fileStream.Length);
        }
    }

}
