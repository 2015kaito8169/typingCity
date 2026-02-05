using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TypingGame : MonoBehaviour {
    public Text wordText;
    public Text statsText;

    private string[] displayWords = { "信号", "地下鉄", "高周集", "電気", "ビル","こんにちは","アメリカ","ひっかけ問題だよ };
    private string[] romaWords = { "singou", "tikatetu", "koujuusyuu", "denki", "biru","konnitiha","amerika","taipingushithi };
    private int currentIdx;
    private string typedStr;
    private float power = 100f;
    private int score = 0;
    private bool isPlaying = true;

    Color themeBlue = new Color(0f, 1f, 1f);
    Color bgDark = new Color(0f, 0.03f, 0.08f);

    void Start() {
        SetupScene();
        NextWord();
    }

    void SetupScene() {
        // --- カメラとUIの自動生成 ---
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.backgroundColor = bgDark;
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
        wordText.fontSize = 60;
        wordText.alignment = TextAnchor.MiddleCenter;
        wordText.color = themeBlue;
        wordText.rectTransform.anchoredPosition = new Vector2(0, 0);

        GameObject statsObj = new GameObject("StatsText");
        statsObj.transform.SetParent(canvasObj.transform);
        statsText = statsObj.AddComponent<Text>();
        statsText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        statsText.fontSize = 25;
        statsText.color = Color.white;
        statsText.rectTransform.anchoredPosition = new Vector2(0, 150);
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
            if (target[typedStr.Length] == c) {
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
        string target = romaWords[currentIdx];
        // 打ったところをグレーにする演出
        wordText.text = $"<color=#888888>{typedStr}</color>{target.Substring(typedStr.Length)}\n<size=40>{displayWords[currentIdx]}</size>";
        wordText.supportRichText = true;
        statsText.text = $"電力量: {Mathf.Floor(power)}% | スコア: {score}";
    }

    void GameOver() {
        isPlaying = false;
        wordText.text = "BLACKOUT";
        wordText.color = Color.red;
        string apiKey = "/sSp3O+cEf5Bco80+ESi+TmpCwxlZ0ndsywZzuJzumCrOkkZmSgk5ueG7yYws2j8h0RhIvtU3f7AyxOzWuakRg==";
        UnityroomApiClient.Instance.SendScore(1, (float)score, ScoreboardWriteMode.HighScoreDesc, apiKey);
    }
}
