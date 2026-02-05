using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public enum ScoreboardWriteMode { HighScoreDesc, HighScoreAsc, LastScore }

public class UnityroomApiClient : MonoBehaviour {
    private static UnityroomApiClient instance;
    public static UnityroomApiClient Instance {
        get {
            if (instance == null) {
                GameObject obj = new GameObject("UnityroomApiClient");
                instance = obj.AddComponent<UnityroomApiClient>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    public void SendScore(int boardNo, float score, ScoreboardWriteMode mode, string apiKey) {
        StartCoroutine(PostScore(boardNo, score, mode, apiKey));
    }

    private IEnumerator PostScore(int boardNo, float score, ScoreboardWriteMode mode, string apiKey) {
        string url = $"https://unityroom.com{boardNo}/scores";
        WWWForm form = new WWWForm();
        form.AddField("score", score.ToString());
        form.AddField("mode", mode.ToString().ToLower());

        using (UnityWebRequest www = UnityWebRequest.Post(url, form)) {
            www.SetRequestHeader("Authorization", "Bearer " + apiKey);
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success) {
                Debug.Log("Score sent successfully!");
            }
        }
    }
}
