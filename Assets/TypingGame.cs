using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TypingGame : MonoBehaviour {
    public Text wordText;
    public Text statsText;

    private string[] displayWords = { "信号", "地下鉄", "電気", "ビル", "工事" };
    private string[] romaWords = { "singou", "tikatetu", "denki", "biru", "kouji" };
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
        wordText.rectTransform.anchoredPosition = new Vector2(0, 0);
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
        power -= Time.deltaTime * 2f;
        if (power <= 0) GameOver();
        CheckInput();
        UpdateUI();
    }

    void CheckInput() {
        string target = romaWords[currentIdx];
        foreach (char c in Input.inputString) {
            if (typedStr.Length < target.Length && target[typedStr.Length] == c) {
                typedStr += c;
                if (typedStr == target) {
                    score += target.Length * 10;
                    power = Mathf.Min(100f, power + 15f);
                    NextWord();
                }
            }
        }
    }

    void NextWord() {
        currentIdx = Random.Range(0, romaWords.Length);
        typedStr = "";
    }

    void UpdateUI() {
        wordText.text = typedStr + romaWords[currentIdx].Substring(typedStr.Length) + "\n" + displayWords[currentIdx];
        statsText.text = "POWER: " + Mathf.Floor(power) + "% | SCORE: " + score;
    }

    void GameOver() {
        isPlaying = false;
        wordText.text = "BLACKOUT";
        wordText.color = Color.red;
        string apiKey = "/sSp3O+cEf5Bco80+ESi+TmpCwxlZ0ndsywZzuJzumCrOkkZmSgk5ueG7yYws2j8h0RhIvtU3f7AyxOzWuakRg==";
        UnityroomApiClient.Instance.SendScore(1, (float)score, ScoreboardWriteMode.HighScoreDesc, apiKey);
    }
}
