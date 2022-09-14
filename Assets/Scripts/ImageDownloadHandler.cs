using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

//to make singleton
public class ImageDownloadHandler : MonoBehaviour
{
    public void DownloadImage(string imageUrl, string fileName, Action<Texture2D> onSuccess, Action<Exception> onFailure, bool reDownload = false)
    {

        if (!reDownload)
        {
            Texture2D texture = GetImage(fileName);

            if (texture != null)
            {
                onSuccess?.Invoke(texture);
                return;
            }
        }

        StartCoroutine(IE_ImageDownloader(imageUrl, texture => {
            SaveImage(texture, fileName);
            onSuccess?.Invoke(texture);
        }, onFailure));
    }

    // coroutine for downloading image
    private IEnumerator IE_ImageDownloader(string imageUrl, Action<Texture2D> onSuccess, Action<Exception> onFailure)
    {
        // create web request and send
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        // check if download succeeded
        if (request.result == UnityWebRequest.Result.Success)
        {
            //extract texture
            DownloadHandlerTexture textureDownloadHandler = (DownloadHandlerTexture)request.downloadHandler;
            Texture2D texture = textureDownloadHandler.texture;

            if (texture == null)
            {
                onFailure?.Invoke(new Exception("image not available"));
                yield break;
            }


            onSuccess?.Invoke(texture);
            yield break;
        }

        // error occured when downloading image 
        onFailure?.Invoke(new Exception(request.error));
    }

    // save image in device for later use
    private static void SaveImage(Texture2D image, string fileName)
    {
        /*string savePath =Path.Combine(Application.persistentDataPath, "data");
        savePath = Path.Combine(savePath, "Images");
        savePath = Path.Combine(savePath, fileName);*/
        string savePath = Application.persistentDataPath + "/data/";
        try
        {
            // check if directory exists, if not create it
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            byte[] bytes = image.EncodeToPNG();
            File.WriteAllBytes(savePath + fileName, bytes);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    // get texture stored in device if exists, if doesn't exists, return null
    private static Texture2D GetImage(string fileName)
    {
        /*string savePath = Path.Combine(Application.persistentDataPath, "data");
        savePath = Path.Combine(savePath, "Images");
        savePath = Path.Combine(savePath, fileName);*/

        string savePath = Application.persistentDataPath + "/data/" + fileName;

        try
        {
            //first check if texture exists , if exists, start fetching
            if (File.Exists(savePath))
            {
                byte[] bytes = File.ReadAllBytes(savePath);
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(bytes);
                return texture;
            }

            return null; // texture not found so return null
        }
        catch (Exception e)
        {
            return null;
        }
    }
}
