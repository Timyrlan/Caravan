using System;
using System.Collections;
using System.Linq;
using System.Text;
using CrvService.Contracts;
using CrvService.Contracts.Base;
using CrvService.Contracts.Entities;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

// ReSharper disable UseObjectOrCollectionInitializer

namespace Assets.Scripts.World
{
    public class CaravanServerHttpConnector
    {
        public IEnumerator ProcessWorld(PingRequest request, Action<PingRequest, PingResponse> callback)
        {
            var requestString = JsonConvert.SerializeObject(request, SuperClass.JsonSettings);

            if (request.ClientCommands.Any())
            {
                var a = "";
                Debug.Log("Ping withCommand: " + requestString);
            }

            using var unityWebRequest = new UnityWebRequest("http://localhost:8066/ping", "POST");
            var bodyRaw = Encoding.UTF8.GetBytes(requestString);
            unityWebRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");

            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
            {
                Debug.LogError($"Ping error. isNetworkError='{unityWebRequest.isNetworkError}', isHttpError='{unityWebRequest.isHttpError}', error:'{unityWebRequest.error}'");
                callback(request, null);
            }
            else
            {
                var responseStr = unityWebRequest.downloadHandler.text;

                var response = JsonConvert.DeserializeObject<PingResponse>(responseStr);

                if (response.Status == null || response.Status.Code != ResponseStatusEnum.Success)
                {
                    Debug.LogError($"Ping error. response code='{response.Status?.Code}', error='{response.Status?.ErrorMessage}'");
                }
                else
                {
                    Debug.Log("Ping ok'");
                    callback(request, response);
                }
            }
        }


        public IEnumerator GetNewWorld(GetNewWorldRequest request, Action<GetNewWorldRequest, PingResponse> callback)
        {
            var requestString = JsonConvert.SerializeObject(request);

            using var unityWebRequest = new UnityWebRequest("http://localhost:8066/GetNewWorld", "POST");
            var bodyRaw = Encoding.UTF8.GetBytes(requestString);
            unityWebRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");

            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
            {
                Debug.LogError($"Ping error. isNetworkError='{unityWebRequest.isNetworkError}', isHttpError='{unityWebRequest.isHttpError}', error:'{unityWebRequest.error}'");
                callback(request, null);
            }
            else
            {
                var responseStr = unityWebRequest.downloadHandler.text;

                var response = JsonConvert.DeserializeObject<PingResponse>(responseStr);

                if (response.Status == null || response.Status.Code != ResponseStatusEnum.Success)
                {
                    Debug.LogError($"Ping error. response code='{response.Status?.Code}', error='{response.Status?.ErrorMessage}'");
                }
                else
                {
                    Debug.Log("Ping ok'");
                    callback(request, response);
                }
            }
        }
    }
}