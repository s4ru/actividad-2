using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class AuthHandler : MonoBehaviour
{

    public string ApiURL = "https://sid-restapi.onrender.com/api/";

    TMP_InputField UsernameInputField;
    TMP_InputField PasswordInputField;
    

    [SerializeField] TMP_InputField ScoreInputField;

    private string Token;

    private string Username;

    public List<TextMeshProUGUI> scoreBoard;

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
            StartCoroutine(GetPerfil(Username));
        }

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
        StartCoroutine(GetPerfil(json));
    }

    public void Score()
    {
        Scorecito scorecito = new Scorecito();
        scorecito.data = new DataUser();

        scorecito.username = Username;
        scorecito.data.score = int.Parse(ScoreInputField.text);

        string json = JsonUtility.ToJson(scorecito);
        StartCoroutine(SendScore(json));
    }

    IEnumerator SendScore(string json)
    {
        UnityWebRequest request = UnityWebRequest.Put(ApiURL + "usuarios", json);
        request.SetRequestHeader("Content-Type", "application/json");
        request.method = "PATCH";
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
                AuthData data = JsonUtility.FromJson<AuthData>(request.downloadHandler.text);

                Debug.Log("Se registro un score" + data.usuario.data.score);
            }
            else
            {
                Debug.Log(request.error);
            }
        }
    }

    IEnumerator SendRegister(string json)
    {
        UnityWebRequest request = UnityWebRequest.Put(ApiURL + "usuarios",json);
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
        UnityWebRequest request = UnityWebRequest.Get(ApiURL + "usuarios"+username);
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
                AuthData data = JsonUtility.FromJson<AuthData>(request.downloadHandler.text);
                Debug.Log("Sesion activa del usuario" + data.usuario.username);
                Debug.Log("Su score es" + data.usuario.data.score);
               

                var ScoreLeaderoard = data.usuarios.OrderByDescending(u => u.data.score).ToArray();

                for (int i = 0; i < scoreBoard.Count; i++)
                {
                    scoreBoard[i].text = ScoreLeaderoard[i].username + " Puntaje: " + ScoreLeaderoard[i].data.score;
                }

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
    public User usuario;
    public string token;
    public User[] usuarios;
    
}

[System.Serializable]
public class User
{
    public string _id;
    public string username;
    public bool estado;
    public DataUser data;

}

[System.Serializable]
public class DataUser
{
    public int score;
}

[System.Serializable]
public class Scorecito
{
    public string username;
    public DataUser data;
}