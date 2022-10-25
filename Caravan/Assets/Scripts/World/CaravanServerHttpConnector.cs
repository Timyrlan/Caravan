using System;
using System.Collections;
using System.Text;
using CrvService.Shared.Contracts.Dto;
using CrvService.Shared.Contracts.Dto.Base;
using CrvService.Shared.Contracts.Entities;
using CrvService.Shared.Logic;
using CrvService.Shared.Logic.ClientSide.Server;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

// ReSharper disable UseObjectOrCollectionInitializer

namespace Assets.Scripts.World
{
    public class CaravanServerHttpConnector : ICaravanServerConnector
    {
        public IEnumerator ProcessWorld(IProcessWorldRequest request, Action<IProcessWorldRequest, IProcessWorldResponse> callback)
        {
            var requestBody = ToDtoMapper.Map(request);
            var requestString = JsonConvert.SerializeObject(requestBody);

            using var unityWebRequest = new UnityWebRequest("http://172.17.45.48:8066/ping", "POST");
            //var unityWebRequest = new UnityWebRequest("http://192.168.0.101:8066/ping", "POST");
            //var unityWebRequest = new UnityWebRequest("http://localhost:8066/ping", "POST");
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

                var response = JsonConvert.DeserializeObject<ProcessWorldResponse>(responseStr);

                if (response.Status == null || response.Status.Code != (int) ResponseStatusEnum.Success)
                {
                    Debug.LogError($"Ping error. response code='{response.Status?.Code}', error='{response.Status?.ErrorMessage}'");
                }
                else
                {
                    Debug.Log("Ping ok'");
                    var result = ToClientSideMapper.Map(response);
                    callback(request, result);
                }
            }
        }


        public IEnumerator GetNewWorld(IGetNewWorldRequest request, Action<IGetNewWorldRequest, IProcessWorldResponse> callback)
        {
            var requestBody = ToDtoMapper.Map(request);
            var requestString = JsonConvert.SerializeObject(requestBody);

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

                var response = JsonConvert.DeserializeObject<ProcessWorldResponse>(responseStr);

                if (response.Status == null || response.Status.Code != (int) ResponseStatusEnum.Success)
                {
                    Debug.LogError($"Ping error. response code='{response.Status?.Code}', error='{response.Status?.ErrorMessage}'");
                }
                else
                {
                    Debug.Log("Ping ok'");
                    var result = ToClientSideMapper.Map(response);
                    callback(request, result);
                }
            }
        }
    }
}