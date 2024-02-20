using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class NewHandler : MonoBehaviour
{
    private string url = "https://rickandmortyapi.com/api";
    //private string url = "https://pokeapi.co/api/v2";
    private string fakedApiUrl = "https://my-json-server.typicode.com/SheiinX/DistribuidosAssignment";

    Coroutine sendRequest_GetCharacters;
    public RawImage[] rawImages;
    public TextMeshProUGUI[] nameSpace;
    public TextMeshProUGUI nameUserSpace;
    private int imageIndex = 0;

    public void SendRequest(int uid)
    {
        if(sendRequest_GetCharacters == null)
        {
            StartCoroutine("GetUserData", uid);
            imageIndex = 0;
        }
    }
    
    IEnumerator GetCharacter(int id)
    {
        UnityWebRequest request = UnityWebRequest.Get(url + "/character/" + id);
        //UnityWebRequest request = UnityWebRequest.Get(url + "/pokemon-form/" + id);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                // Debug.Log(request.downloadHandler.text);
                Character character = JsonUtility.FromJson<Character>(request.downloadHandler.text);
                Debug.Log(character.name);

                StartCoroutine("DownloadImage", character);
            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }
        }
    }

    IEnumerator GetUserData(int id)
    {
        UnityWebRequest request = UnityWebRequest.Get(fakedApiUrl + "/users/" + id);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                //Debug.Log(request.downloadHandler.text);
                UserData user = JsonUtility.FromJson<UserData>(request.downloadHandler.text);

                Debug.Log(user.username);
                nameUserSpace.text = user.username;

                foreach (int cardID in user.deck)
                {
                    StartCoroutine("GetCharacter", cardID);
                }

            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }

        }
    }

    IEnumerator DownloadImage(Character character)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(character.image);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            rawImages[imageIndex].texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            imageIndex++;

            nameSpace[imageIndex-1].text = character.name;
        }
    }
}

[System.Serializable]
public class UserData
{

    public int id;
    public string username;
    public int[] deck;
}

[System.Serializable]
public class CharactersList
{
    public Character[] results;
    public CharacterInfo info;
}
[System.Serializable]
public class Character
{
    public int id;
    public string name;
    public string species;
    public string image;

}

[System.Serializable]
public class CharacterInfo
{
    public int count;
    public int page;
    public int prev;
    public string next;
}