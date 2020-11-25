﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
using System.IO;
using UnityEngine.Networking;

public class ApiController : MonoBehaviour
{

    public string uploadImagesUrl = "http://127.0.0.1:5000/uploadImages";
    public string loginUrl = "http://127.0.0.1:5000/login";
    public GameObject imageClassGameObject;

    // Start is called before the first frame update
    void Start()
    {
        EventsManager.instance.UploadImagesTrigger += requestImageUpload;
        EventsManager.instance.LoginTrigger += requestLogin;
    }

    public void requestLogin(int id, string username, string password, bool remember_login)
    {
        Debug.Log("Start Login");
        StartCoroutine(Login(username, password, remember_login, loginUrl));
    }

    public static IEnumerator Login(string username, string password, bool remember_login, string url)
    {
        LoginRequest requestBody = new LoginRequest();
        requestBody.email = username;
        requestBody.password = password;
        requestBody.remember_login = remember_login;

        string requestString = JsonUtility.ToJson(requestBody) ?? "";
        UnityWebRequest www = UnityWebRequest.Put(url, requestString);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Login complete!");
        }
    }

    public void requestImageUpload(int id)
    {
        Debug.Log("Start Images Upload");
        ImageClassificationScript imageClassification = imageClassGameObject.GetComponent<ImageClassificationScript>();
        List<string> framesList = imageClassification.framesList;
        StartCoroutine(UploadImages(framesList, uploadImagesUrl));
    }

    public static IEnumerator UploadImages(List<string> frames, string url)
    {
        WWWForm form = new WWWForm();
        for (int i = 0; i < frames.Count; i++)
        {
            form.AddField("frame" + i, frames[i]);
        }

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Upload images complete!");
        }

    }
}