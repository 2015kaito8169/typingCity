using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class WordData {
    public string k; // かな
    public string[] r; // ローマ字パターン
}

public class TypingGame : MonoBehaviour {
    public Text wordText;
    public Text kanaText;
    public Text statsText;
    public RectTransform powerBar;

    private float power = 100f;
    private int score = 0;
    private float speed = 1.0f;
    private string typedStr = "";
    private WordData currentWord;
    private bool isPlaying = true;

   new WordData { k = "しんごう", r = new[] { "SHINGOU" } },
        new WordData { k = "ちかてつ", r = new[] { "CHIKATETSU", "TIKATETSU", "CHIKATETU", "TIKATETU" } },
        new WordData { k = "つうしん", r = new[] { "TSUUSHIN", "TUUSHIN", "TSUUSIN", "TUUSIN" } },
        new WordData { k = "はつでんき", r = new[] { "HATSUDENKI", "HATUDENKI" } },
        new WordData { k = "でんりょく", r = new[] { "DENRYOKU" } },
        new WordData { k = "ふっかつ", r = new[] { "FUKKATSU", "HUKKATSU", "FUKKATU", "HUKKATU" } },
        new WordData { k = "エネルギー", r = new[] { "ENERUGI-" } },
        new WordData { k = "たいぴんぐ", r = new[] { "TAIPINGU" } },
        new WordData { k = "ていでん", r = new[] { "TEIDEN" } },
        new WordData { k = "がいろとう", r = new[] { "GAIROUTOU" } },
        new WordData { k = "ビルディング", r = new[] { "BIRUDINGU" } },
        new WordData { k = "こうじょう", r = new[] { "KOUJOU" } },
        new WordData { k = "こうえん", r = new[] { "KOUEN" } },
        new WordData { k = "でんせん", r = new[] { "DENSEN" } },
        new WordData { k = "へんあつき", r = new[] { "HENATSUKI", "HENATUKI" } },
        new WordData { k = "しゅうり", r = new[] { "SHUURI", "SYUURI" } },
        new WordData { k = "てんけん", r = new[] { "TENKEN" } },
        new WordData { k = "ぎじゅつ", r = new[] { "GIJUTSU", "GIJUTU" } },
        new WordData { k = "さぎょう", r = new[] { "SAGYOU" } },
        new WordData { k = "かんり", r = new[] { "KANRI" } },
        new WordData { k = "どりょく", r = new[] { "DORYOKU" } },
        new WordData { k = "ほうしゅう", r = new[] { "HOUSHUU" } },
        new WordData { k = "システム", r = new[] { "SHISUTEMU", "SISUTEMU" } },
        new WordData { k = "ネットワーク", r = new[] { "NETTO-WA-KU" } },
        new WordData { k = "バッテリー", r = new[] { "BATTERI-" } },
        new WordData { k = "タービン", r = new[] { "TA-BIN" } },
        new WordData { k = "ボルト", r = new[] { "BORUTO" } },
        new WordData { k = "アンペア", r = new[] { "ANPEA" } },
        new WordData { k = "ワット", r = new[] { "WATTO" } },
        new WordData { k = "きぼう", r = new[] { "KIBOU" } },
        new WordData { k = "みらい", r = new[] { "MIRAI" } },
        new WordData { k = "ぜんりょく", r = new[] { "ZENRYOKU" } },
        new WordData { k = "スピード", r = new[] { "SUPI-DO" } },
        new WordData { k = "ありがとう", r = new[] { "ARIGATOU" } }
    };

    void Start() {
        // ※本来はエディタで設定しますが、コードで仮のカメラとUIを生成する処理をここに入れます
        SetupScene();
        NextWord();
    }

    void Update() {
        if (!isPlaying) return;

        // 時間経過で電力量減少
        power -= Time.deltaTime * 1.2f * speed;
        speed += 0.00001f;
        UpdateUI();

        if (power <= 0) GameOver();

        // キー入力判定
        foreach (char c in Input.inputString) {
            string key = c.ToString().ToUpper();
            CheckInput(key);
        }
    }

    void CheckInput(string input) {
        string nextStr = typedStr + input;
        // 入力した文字列が、いずれかのローマ字パターンの先頭と一致するか
        bool isMatch = currentWord.r.Any(pattern => pattern.StartsWith(nextStr));

        if (isMatch) {
            typedStr = nextStr;
            // 単語が完成したか判定
            if (currentWord.r.Any(pattern => pattern == typedStr)) {
                score += typedStr.Length * 10;
                power = Mathf.Min(100f, power + 6f);
                NextWord();
            } else {
                RenderWord();
            }
        }
    }

    void NextWord() {
        currentWord = words[UnityEngine.Random.Range(0, words.Count)];
        typedStr = "";
        kanaText.text = currentWord.k;
        RenderWord();
    }

    void RenderWord() {
        // UnityのTextコンポーネントに反映
        string basePattern = currentWord.r.FirstOrDefault(p => p.StartsWith(typedStr)) ?? currentWord.r[0];
        string remain = basePattern.Substring(typedStr.length);
        wordText.text = $"<color=white>{typedStr}</color><color=grey>{remain}</color>";
    }

    void UpdateUI() {
        statsText.text = $"電力量: {Mathf.Floor(power)}% | 発電中: {score}W";
        if (powerBar != null) powerBar.localScale = new Vector3(power / 100f, 1, 1);
    }

       // --- この部分を探すか、新しく貼り付けてください ---
    void GameOver() {
        isPlaying = false;
        wordText.text = "BLACKOUT";
        wordText.color = Color.red;
 　　　string apiKey = "/sSp3O+cEf5Bco80+ESi+TmpCwxlZ0ndsywZzuJzumCrOkkZmSgk5ueG7yYws2j8h0RhIvtU3f7AyxOzWuakRg==";
        UnityroomApiClient.Instance.SendScore(1, (float)score, ScoreboardWriteMode.HighScoreDesc);
        
        Debug.Log("ゲームオーバー！最終スコア: " + score);
    }

　　 void SetupScene() {
        using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TypingGame : MonoBehaviour {
    // --- CSSのデザインをC#で再現する変数 ---
    Color themeBlue = new Color(0f, 1f, 1f); // #0ff
    Color bgDark = new Color(0f, 0.03f, 0.08f); // #000814

    void SetupScene() {
        // 1. 背景（CSSのbody相当）
        GameObject bg = new GameObject("Background");
        var bgImage = bg.AddComponent<Image>();
        bgImage.color = bgDark;
        bg.transform.SetParent(GameObject.Find("Canvas").transform);

        // 2. ビル（CSSの.building相当）を自動生成
        for (int i = 0; i < 8; i++) {
            GameObject building = new GameObject("Building" + i);
            var img = building.AddComponent<Image>();
            img.color = new Color(0.06f, 0.06f, 0.06f); // #111
            building.transform.SetParent(bg.transform);
            
            // 位置とサイズをCSS通りに計算
            RectTransform rt = building.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(75, Random.Range(100, 220));
            rt.anchoredPosition = new Vector2(-300 + (i * 90), -200);
        }

        // 3. テキストの光（CSSのbox-shadow相当）
        // Unityでは同じテキストを少しずらして重ねることで「光」を表現します
        wordText.color = themeBlue;
        var shadow = wordText.gameObject.AddComponent<Shadow>();
        shadow.effectColor = themeBlue;
        shadow.effectDistance = new Vector2(2, -2);
    }
}
