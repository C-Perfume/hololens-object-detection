using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityEngine.Satbyul
{
      //public class test
    //{
    //    public int num;
    //    public int idx;
    //    public string name;
    //    public int pw;
    //}

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

        /// <summary>
        /// Initializes this class
        /// </summary>
        /// 
        private void Awake()
        {
            // Allows this instance to behave like a singleton
            Instance = this;
        }

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

                int preidx = jsonResponse.IndexOf("predictions");
                int regularidx = jsonResponse.IndexOf("Regular");
                Logger.Log($"P{jsonResponse}".Substring(preidx + 1, regularidx+8 - preidx).Replace(",", "\n"));

                // The response will be in JSON format, therefore it needs to be deserialized
                AnalysisRootObject analysisRootObject = new AnalysisRootObject();
                analysisRootObject = JsonConvert.DeserializeObject<AnalysisRootObject>(jsonResponse);
                //analysisRootObject = JsonUtility.FromJson<AnalysisRootObject>(jsonResponse);
                Logger.Log(analysisRootObject.predictions[0].tagName);
              
                SceneOrganiser.Instance.FinaliseLabel(analysisRootObject);
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
