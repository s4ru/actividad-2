using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class AuthHandler : MonoBehaviour
{

    public string ApirURL = "https://sid-restapi.onrender.com/";

    TMP_InputField UsernameInputField;
    TMP_InputField PasswordInputField;
    
    void Start()
    {
        UsernameInputField = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>();
        PasswordInputField = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>();
    }

    public void Registrar()
    {
        AuthData auhtData = new AuthData();
        auhtData username = UsernameInputField.text;
        auhtData password = PasswordInputField.text;

        string json = JsonUtility.ToJson(auhtData);

        StartCoroutine(SendRegister(json));
    }

    IEnumerator SendRegister(string json)
    {
        UnityWebRequest request = UnityWebRequest.Put(ApirURL + "/usuarios",json);
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

}

[System.Serializable]
public class AuthData
{
    public string username;
    public string password;
    public UserData usuario;
}

[System.Serializable]
public class UserData
{
    public string _id;
    public string username;
    public bool estado;
}