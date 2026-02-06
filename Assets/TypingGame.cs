using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TypingGame : MonoBehaviour {
    private string[] words = { "singou", "denki", "biru" };
    private int idx;
    private string typed = "";
    private float power = 100f;
    private bool isPlaying = true;

    void Start() {
        GameObject cam = new GameObject("MainCamera");
        cam.AddComponent<Camera>().backgroundColor = Color.black;
        cam.transform.position = new Vector3(0, 0, -10);
        Next();
    }

    void Update() {
        if (!isPlaying) return;
        power -= Time.deltaTime * 3f;
        if (power <= 0) isPlaying = false;
        foreach (char c in Input.inputString) {
            if (words[idx][typed.Length] == c) {
                typed += c;
                if (typed == words[idx]) {
                    power = Mathf.Min(100f, power + 20f);
                    Next();
                }
            }
        }
    }

    void OnGUI() {
        GUI.skin.label.fontSize = 50;
        GUI.Label(new Rect(100, 100, 800, 200), isPlaying ? words[idx] + "\n" + typed : "GAME OVER");
        GUI.Label(new Rect(100, 300, 800, 100), "POWER: " + (int)power);
    }

    void Next() {
        idx = Random.Range(0, words.Length);
        typed = "";
    }

    void GameOver() {
        isPlaying = false;
        StartCoroutine(Post());
    }

    IEnumerator Post() {
        WWWForm f = new WWWForm();
        f.AddField("score", "100");
        using (UnityWebRequest w = UnityWebRequest.Post("https://unityroom.com", f)) {
            w.SetRequestHeader("Authorization", "Bearer /sSp3O+cEf5Bco80+ESi+TmpCwxlZ0ndsywZzuJzumCrOkkZmSgk5ueG7yYws2j8h0RhIvtU3f7AyxOzWuakRg==");
            yield return w.SendWebRequest();
        }
    }
}
