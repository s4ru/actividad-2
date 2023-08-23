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
    TMP_InputField ScoreInputField;

    private string Token;

    private string Username;


    void Start()
    {
        Token = PlayerPrefs.GetString("token");

        if (string.IsNullOrEmpty(Token) )
        {
            Debug.Log("No hay token almacenado");

        }
        else
        {
            Username = PlayerPrefs.GetString("username");
        }

        UsernameInputField = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>();
        PasswordInputField = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>();
        ScoreInputField = GameObject.Find("InputFieldScore").GetComponent<TMP_InputField>();
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

    public void Score()
    {
        AuthData authData = new AuthData();
        authData.score = ScoreInputField.text;

        string json = JsonUtility.ToJson(authData);

        StartCoroutine(SendScore(json));
    }

    IEnumerator SendScore(string json)
    {
        UnityWebRequest request = UnityWebRequest.Put(ApiURL + "/score", json);
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

                Debug.Log("Se registro un score" + data.score);
            }
            else
            {
                Debug.Log(request.error);
            }
        }
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

    IEnumerator GetPerfil(string username)
    {
        UnityWebRequest request = UnityWebRequest.Get(ApiURL + "usuarios/"+username);
        request.SetRequestHeader("x-token", Token);
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
                PlayerPrefs.SetString("token",data.token);
                PlayerPrefs.SetString("username", data.usuario.username);
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
    public string score;
}

[System.Serializable]
public class UserData
{
    public string _id;
    public string username;
    public bool estado;

}