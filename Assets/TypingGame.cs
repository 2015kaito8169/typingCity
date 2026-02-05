using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class TypingGame : MonoBehaviour {
    public Text wordText;
    public Text statsText;
    private string[] romaWords = { "singou", "tikatetu", "denki", "biru", "kouji" };
    private string[] displayWords = { "信号", "地下鉄", "電気", "ビル", "工事" };
    private int currentIdx;
    private string typedStr = "";
    private float power = 100f;
    private int score = 0;
    private bool isPlaying = true;

    void Start() {
        SetupScene();
        NextWord();
    }

    void SetupScene() {
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.backgroundColor = new Color(0f, 0.03f, 0.08f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        camObj.transform.position = new Vector3(0, 0, -10);

        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject wordObj = new GameObject("WordText");
        wordObj.transform.SetParent(canvasObj.transform);
        wordText = wordObj.AddComponent<Text>();
        wordText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        wordText.fontSize = 50;
        wordText.alignment = TextAnchor.MiddleCenter;
        wordText.color = Color.cyan;
        wordText.rectTransform.anchoredPosition = Vector2.zero;
        wordText.rectTransform.sizeDelta = new Vector2(800, 200);

        GameObject statsObj = new GameObject("StatsText");
        statsObj.transform.SetParent(canvasObj.transform);
        statsText = statsObj.AddComponent<Text>();
        statsText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        statsText.fontSize = 25;
        statsText.color = Color.white;
        statsText.rectTransform.anchoredPosition = new Vector2(0, 150);
        statsText.rectTransform.sizeDelta = new Vector2(800, 100);
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
                    NextWord();
                }
            }
        }
        wordText.text = typedStr + romaWords[currentIdx].Substring(typedStr.Length) + "\n" + displayWords[currentIdx];
        statsText.text = "POWER: " + Mathf.Floor(power) + "% | SCORE: " + score;
    }

    void NextWord() {
        currentIdx = Random.Range(0, romaWords.Length);
        typedStr = "";
    }

    void GameOver() {
        isPlaying = false;
        wordText.text = "BLACKOUT";
        wordText.color = Color.red;
        // unityroom API 直接送信（ここにあなたのAPIキーを貼り付けてください）
        string apiKey = "/sSp3O+cEf5Bco80+ESi+TmpCwxlZ0ndsywZzuJzumCrOkkZmSgk5ueG7yYws2j8h0RhIvtU3f7AyxOzWuakRg==";
        StartCoroutine(PostScore(1, (float)score, apiKey));
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
