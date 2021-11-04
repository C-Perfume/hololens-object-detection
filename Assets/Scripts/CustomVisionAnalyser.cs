using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityEngine.Satbyul
{
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
        private string predictionEndpoint = "https://sblee1-prediction.cognitiveservices.azure.com/customvision/v3.1/Prediction/e66ddd6f-b852-46d9-b74f-0e8b7ab82417/classify/iterations/Indoors/image";
        /// <summary>
        /// Bite array of the image to submit for analysis
        /// </summary>
        [HideInInspector] public byte[] imageBytes;


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

                int preidx = jsonResponse.IndexOf("predictions");
                int regularidx = jsonResponse.IndexOf("Regular");
                Logger.Log($"{jsonResponse}".Substring(preidx - 1, regularidx + 10 - preidx).Replace(",", "\n"));

                AnalysisRootObject analysisRootObject = JsonConvert.DeserializeObject<AnalysisRootObject>(jsonResponse);

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
