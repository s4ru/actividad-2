using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class AuthHandler : MonoBehaviour
{

    public string ApiURL = "https://sid-restapi.onrender.com/";

    TMP_InputField UsernameInputField;
    TMP_InputField PasswordInputField;
    
    void Start()
    {
        UsernameInputField = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>();
        PasswordInputField = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>();
    }

    public void Registrar()
    {
        AuthData authData = new AuthData();
        authData.username = UsernameInputField.text;
        authData.password = PasswordInputField.text;

        string json = JsonUtility.ToJson(authData);

        StartCoroutine(SendRegister(json));
    }

    public void Login()
    {
        AuthData authData = new AuthData();
        authData.username = UsernameInputField.text;
        authData.password = PasswordInputField.text;

        string json = JsonUtility.ToJson(authData);

        StartCoroutine(SendLogin(json));
    }


    IEnumerator SendRegister(string json)
    {
        UnityWebRequest request = UnityWebRequest.Put(ApiURL + "/usuarios",json);
        request.SetRequestHeader("Content-Type", "application/json");
        request.method = "POST";
        yield return request.SendWebRequest();

         if(request.isNetworkError)
        {
            Debug.Log("NETWORK ERROR :" + request.error);
        }
         else
        {
            Debug.Log(request.downloadHandler.text);
            if(request.responseCode == 200)
            {
                AuthData data = JsonUtility.FromJson<AuthData>(request.downloadHandler.text);

                Debug.Log("Se registro el usuario con id" + data.usuario._id);
            }
            else
            {
                Debug.Log(request.error);
            }
        }
    }


    IEnumerator SendLogin(string json)
    {
        UnityWebRequest request = UnityWebRequest.Put(ApiURL + "auth/login", json);
        request.SetRequestHeader("Content-Type", "application/json");
        request.method = "POST";
        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.Log("NETWORK ERROR :" + request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
            if (request.responseCode == 200)
            {
                AuthData data = JsonUtility.FromJson<AuthData>(request.downloadHandler.text);

                Debug.Log("Inicio sesion el usuario" + data.usuario.username);
                Debug.Log(data.token);
            }
            else
            {
                Debug.Log(request.error);
            }
        }
    }
}

[System.Serializable]
public class AuthData
{
    public string username;
    public string password;
    public UserData usuario;
    public string token;
}

[System.Serializable]
public class UserData
{
    public string _id;
    public string username;
    public bool estado;
}