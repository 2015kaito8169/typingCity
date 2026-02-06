using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TypingGame : MonoBehaviour {
    private string[] words = { "singou", "tikatetu", "denki", "biru", "kouji" };
    private string[] kanji = { "信号", "地下鉄", "電気", "ビル", "工事" };
    private int idx;
    private string typed = "";
    private float power = 100f;
    private float totalPower = 0f;
    private int score = 0;
    private bool isPlaying = true;

    void Start() {
        GameObject cam = new GameObject("MainCamera");
        Camera c = cam.AddComponent<Camera>();
        c.backgroundColor = new Color(0f, 0.05f, 0.1f);
        c.clearFlags = CameraClearFlags.SolidColor;
        cam.transform.position = new Vector3(0, 0, -10);
        Next();
    }

    void Update() {
        if (!isPlaying) return;
        power -= Time.deltaTime * 2.5f;
        if (power <= 0) { isPlaying = false; StartCoroutine(SendAll()); }
        foreach (char c in Input.inputString) {
            if (typed.Length < words[idx].Length && words[idx][typed.Length] == c) {
                typed += c;
                if (typed == words[idx]) {
                    score += words[idx].Length * 10;
                    power = Mathf.Min(100f, power + 15f);
                    totalPower += 15f;
                    Next();
                }
            }
        }
    }

    void OnGUI() {
        GUIStyle s = new GUIStyle();
        s.fontSize = 50; s.alignment = TextAnchor.MiddleCenter; s.normal.textColor = Color.cyan;
        GUI.Label(new Rect(0, Screen.height/2-100, Screen.width, 200), isPlaying ? typed + "\n" + kanji[idx] : "BLACKOUT", s);
        s.fontSize = 20; s.alignment = TextAnchor.UpperLeft; s.normal.textColor = Color.white;
        GUI.Label(new Rect(20, 20, 600, 200), "POWER: " + (int)power + "%\nSCORE: " + score + "\nTOTAL: " + (int)totalPower, s);
    }

    void Next() { idx = Random.Range(0, words.Length); typed = ""; }

    IEnumerator SendAll() {
        string key = "/sSp3O+cEf5Bco80+ESi+TmpCwxlZ0ndsywZzuJzumCrOkkZmSgk5ueG7yYws2j8h0RhIvtU3f7AyxOzWuakRg==";
        yield return StartCoroutine(Post(1, (float)score, key));
        yield return StartCoroutine(Post(2, totalPower, key));
    }

    IEnumerator Post(int bNo, float val, string key) {
        WWWForm f = new WWWForm();
        f.AddField("score", val.ToString());
        using (UnityWebRequest w = UnityWebRequest.Post("https://unityroom.com" + bNo + "/scores", f)) {
            w.SetRequestHeader("Authorization", "Bearer " + key);
            yield return w.SendWebRequest();
        }
    }
}
