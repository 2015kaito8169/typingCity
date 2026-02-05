using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class UnityroomApiClient : MonoBehaviour {
    public static UnityroomApiClient Instance {
        get {
            if (instance == null) {
                instance = new GameObject("UnityroomApiClient").AddComponent<UnityroomApiClient>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }
    private static UnityroomApiClient instance;

    public void SendScore(int boardNo, float score, string mode, string apiKey = "") {
        StartCoroutine(PostScore(boardNo, score, apiKey));
    }

    private IEnumerator PostScore(int boardNo, float score, string apiKey) {
        WWWForm form = new WWWForm();
        form.AddField("score", score.ToString());
        using (UnityWebRequest www = UnityWebRequest.Post($"https://unityroom.com{boardNo}/scores", form)) {
            if (!string.IsNullOrEmpty(apiKey)) www.SetRequestHeader("Authorization", "Bearer " + apiKey);
            yield return www.SendWebRequest();
        }
    }
}

public enum ScoreboardWriteMode { HighScoreDesc, HighScoreAsc, LastScore }
