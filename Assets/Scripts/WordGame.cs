using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using TMPro;

public class WordGame : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI englishWordText;   // 英文單字顯示
    public TextMeshProUGUI chineseWordText;   // 中文意思顯示
    public Button nextWordButton;             // 下一個單字按鈕

    [Header("Word Data")]
    public string wordFilePath = "words.txt"; // 單字文件路徑（放在Assets/Resources目錄下）

    private List<WordData> wordsList = new List<WordData>();
    private int currentWordIndex = -1;

    // 單字數據結構
    [System.Serializable]
    public class WordData
    {
        public string englishWord;  // 英文單字
        public string chineseMeaning; // 中文意思
    }

    void Start()
    {
        // 讀取單字文件
        LoadWordsFromFile();

        // 設置按鈕事件
        if (nextWordButton != null)
            nextWordButton.onClick.AddListener(ShowNextWord);

        // 顯示第一個單字
        ShowNextWord();
    }

    // 從文件讀取單字列表
    void LoadWordsFromFile()
    {
        try
        {
            // 從Resources文件夾中讀取文件
            TextAsset wordFile = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(wordFilePath));

            if (wordFile != null)
            {
                string[] lines = wordFile.text.Split('\n');

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    // 假設每行格式為: 英文單字,中文意思
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

    // 顯示下一個隨機單字
    public void ShowNextWord()
    {
        if (wordsList.Count == 0)
        {
            Debug.LogWarning("單字列表為空！");
            return;
        }

        // 隨機選擇一個單字
        int newIndex = Random.Range(0, wordsList.Count);

        // 確保不連續重複同一個單字
        if (wordsList.Count > 1)
        {
            while (newIndex == currentWordIndex)
            {
                newIndex = Random.Range(0, wordsList.Count);
            }
        }

        currentWordIndex = newIndex;

        // 顯示單字信息
        DisplayCurrentWord();
    }

    // 顯示當前單字信息
    void DisplayCurrentWord()
    {
        WordData currentWord = wordsList[currentWordIndex];

        if (englishWordText != null)
            englishWordText.text = currentWord.englishWord;

        if (chineseWordText != null)
            chineseWordText.text = currentWord.chineseMeaning;
    }
}
