using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TypingGame : MonoBehaviour {
    private string[] romaWords = { "singou", "tikatetu", "denki", "biru", "kouji" };
    private string[] displayWords = { "信号", "地下鉄", "電気", "ビル", "工事" };
    private int currentIdx;
    private string typedStr = "";
    private float power = 100f;
    private float totalPower = 0f;
    private int score = 0;
    private bool isPlaying = true;

    void Start() {
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.backgroundColor = new Color(0f, 0.03f, 0.08f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        camObj.transform.position = new Vector3(0, 0, -10);
        NextWord();
    }

    void Update() {
        if (!isPlaying) return;
        power -= Time.deltaTime * 2.5f;
        if (power <= 0) GameOver();
        foreach (char c in Input.inputString) {
            string target = romaWords[currentIdx];
            if (typedStr.Length < target.Length && target[typedStr.Length] == c) {
                typedStr += c;
                if (typedStr == target) {
                    score += target.Length * 10;
                    power = Mathf.Min(100f, power + 15f);
                    totalPower += 15f;
                    NextWord();
                }
            }
        }
    }

    void OnGUI() {
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = 40;
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.normal.textColor = Color.cyan;

        string target = romaWords[currentIdx];
        string display = typedStr + target.Substring(typedStr.Length) + "\n" + displayWords[currentIdx];
        GUI.Label(new Rect(0, Screen.height/2 - 100, Screen.width, 200), display, labelStyle);

        labelStyle.fontSize = 20;
        labelStyle.alignment = TextAnchor.UpperLeft;
        labelStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(20, 20, 600, 150), $"POWER: {Mathf.Floor(power)}%\nSCORE: {score}\nTOTAL: {Mathf.Floor(totalPower)}", labelStyle);

        if (!isPlaying) {
            labelStyle.fontSize = 80;
            labelStyle.normal.textColor = Color.red;
            labelStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "BLACKOUT", labelStyle);
        }
    }

    void NextWord() {
        currentIdx = Random.Range(0, romaWords.Length);
        typedStr = "";
    }

    void GameOver() {
        if (!isPlaying) return;
        isPlaying = false;
        string apiKey = "/sSp3O+cEf5Bco80+ESi+TmpCwxlZ0ndsywZzuJzumCrOkkZmSgk5ueG7yYws2j8h0RhIvtU3f7AyxOzWuakRg==";
        StartCoroutine(PostScore(1, (float)score, apiKey));
        StartCoroutine(PostScore(2, totalPower, apiKey));
    }

    IEnumerator PostScore(int boardNo, float scoreVal, string key) {
        string url = "https://unityroom.com" + boardNo + "/scores";
        WWWForm form = new WWWForm();
        form.AddField("score", scoreVal.ToString());
        form.AddField("mode", "high_score_desc");
        using (UnityWebRequest www = UnityWebRequest.Post(url, form)) {
            www.SetRequestHeader("Authorization", "Bearer " + key);
            yield return www.SendWebRequest();
        }
    }
}
