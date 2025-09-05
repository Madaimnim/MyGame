using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    //可Inspector自行定義輸入視窗
    #region class DialogueLine
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;         // 誰在講話
        [TextArea(2, 5)]
        public string content;         // 說的內容 (TextArea 讓 Inspector 好輸入多行)
    }
    #endregion

    //變數
    #region
    public static DialogueManager Instance;

    [Header("UI 元素")]
    public GameObject dialoguePanel;    // 對話 UI Panel
    public TMP_Text speakerText;        // 顯示講話的人
    public TMP_Text dialogueText;       // 顯示對話內容

    [Header("對話內容 (Inspector 填)")]
    public DialogueLine[] dialogueLines;  // 陣列，每個元素是一行對話

    public float typingSpeed = 0.05f;   // 打字機速度

    private int currentLineIndex = 0;
    private bool isTyping = false;

    public bool isDialogueRunning { get; private set; } = false;
    #endregion

    //生命週期
    #region
    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // 避免重複生成
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);  // 場景切換時不銷毀
    }

    private void Start() {
        dialoguePanel.SetActive(false);
    }
    private void Update() {
        if (!dialoguePanel.activeSelf) return;
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
        {
            ShowNextLine();
        }
    }
    #endregion

    public void StartDialogue() {
        if (dialogueLines.Length == 0) return;
        dialoguePanel.SetActive(true);

        isDialogueRunning = true;

        currentLineIndex = 0;
        ShowNextLine();
    }

    public void ShowNextLine() {
        if (isTyping) return;

        if (currentLineIndex < dialogueLines.Length)
        {
            DialogueLine line = dialogueLines[currentLineIndex];
            speakerText.text = line.speaker;
            StartCoroutine(TypeLine(line.content));
            currentLineIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    private IEnumerator TypeLine(string line) {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void EndDialogue() {
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        speakerText.text = "";

        isDialogueRunning = false;
    }
}