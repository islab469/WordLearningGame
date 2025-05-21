using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using TMPro;
using System.Collections;  // 需要使用 Coroutine

public class WordGame : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI englishWordText;   // 英文單字顯示
    public TextMeshProUGUI chineseWordText;   // 中文意思顯示
    public Button nextWordButton;             // 下一個單字按鈕

    [Header("Animation Settings")]
    public float fadeDuration = 0.5f;         // 淡入淡出時間
    public float buttonClickScale = 0.9f;     // 按鈕縮放比例
    public float buttonClickDuration = 0.1f;  // 按鈕縮放時間

    [Header("Word Data")]
    public string wordFilePath = "words.txt"; // 單字文件路徑（放在Assets/Resources目錄下）

    private List<WordData> wordsList = new List<WordData>();
    private int currentWordIndex = -1;

    [System.Serializable]
    public class WordData
    {
        public string englishWord;  // 英文單字
        public string chineseMeaning; // 中文意思
    }

    void Start()
    {
        LoadWordsFromFile();

        if (nextWordButton != null)
        {
            nextWordButton.onClick.AddListener(() =>
            {
                StartCoroutine(ButtonClickAnimation(nextWordButton));
                ShowNextWordWithFade();
            });
        }

        ShowNextWordWithFade();
    }

    void LoadWordsFromFile()
    {
        try
        {
            TextAsset wordFile = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(wordFilePath));

            if (wordFile != null)
            {
                string[] lines = wordFile.text.Split('\n');

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] parts = line.Trim().Split(',');

                    if (parts.Length >= 2)
                    {
                        WordData word = new WordData
                        {
                            englishWord = parts[0].Trim(),
                            chineseMeaning = parts[1].Trim()
                        };

                        wordsList.Add(word);
                    }
                }

                Debug.Log($"成功加載了 {wordsList.Count} 個單字");
            }
            else
            {
                Debug.LogError($"無法加載單字文件: {wordFilePath}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"讀取單字文件時出錯: {e.Message}");
        }
    }

    public void ShowNextWord()
    {
        if (wordsList.Count == 0)
        {
            Debug.LogWarning("單字列表為空！");
            return;
        }

        int newIndex = Random.Range(0, wordsList.Count);

        if (wordsList.Count > 1)
        {
            while (newIndex == currentWordIndex)
            {
                newIndex = Random.Range(0, wordsList.Count);
            }
        }

        currentWordIndex = newIndex;
        DisplayCurrentWord();
    }

    void DisplayCurrentWord()
    {
        WordData currentWord = wordsList[currentWordIndex];

        if (englishWordText != null)
            englishWordText.text = currentWord.englishWord;

        if (chineseWordText != null)
            chineseWordText.text = currentWord.chineseMeaning;
    }

    // 帶淡入淡出效果的顯示下一個單字
    public void ShowNextWordWithFade()
    {
        StartCoroutine(FadeTextOutIn());
    }

    IEnumerator FadeTextOutIn()
    {
        bool englishDone = false;
        bool chineseDone = false;

        // 開始同時淡出
        StartCoroutine(FadeTextAlpha(englishWordText, 1, 0, fadeDuration, () => englishDone = true));
        StartCoroutine(FadeTextAlpha(chineseWordText, 1, 0, fadeDuration, () => chineseDone = true));

        // 等待兩個都淡出完成
        yield return new WaitUntil(() => englishDone && chineseDone);

        // 換字
        ShowNextWord();

        englishDone = false;
        chineseDone = false;

        // 開始同時淡入
        StartCoroutine(FadeTextAlpha(englishWordText, 0, 1, fadeDuration, () => englishDone = true));
        StartCoroutine(FadeTextAlpha(chineseWordText, 0, 1, fadeDuration, () => chineseDone = true));

        // 等待兩個都淡入完成
        yield return new WaitUntil(() => englishDone && chineseDone);
    }

    // 修改FadeTextAlpha，加入結束callback
    IEnumerator FadeTextAlpha(TextMeshProUGUI text, float from, float to, float duration, System.Action onComplete)
    {
        float elapsed = 0f;
        Color c = text.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            text.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        text.color = new Color(c.r, c.g, c.b, to);

        // 呼叫結束回調
        onComplete?.Invoke();
    }


    // 按鈕點擊縮放動畫
    IEnumerator ButtonClickAnimation(Button button)
    {
        Vector3 originalScale = button.transform.localScale;
        Vector3 targetScale = originalScale * buttonClickScale;

        float elapsed = 0f;
        while (elapsed < buttonClickDuration)
        {
            elapsed += Time.deltaTime;
            button.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / buttonClickDuration);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < buttonClickDuration)
        {
            elapsed += Time.deltaTime;
            button.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / buttonClickDuration);
            yield return null;
        }

        button.transform.localScale = originalScale;
    }
}
