using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    //�iInspector�ۦ�w�q��J����
    #region class DialogueLine
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;         // �֦b����
        [TextArea(2, 5)]
        public string content;         // �������e (TextArea �� Inspector �n��J�h��)
    }
    #endregion

    //�ܼ�
    #region
    public static DialogueManager Instance;

    [Header("UI ����")]
    public GameObject dialoguePanel;    // ��� UI Panel
    public TMP_Text speakerText;        // ������ܪ��H
    public TMP_Text dialogueText;       // ��ܹ�ܤ��e

    [Header("��ܤ��e (Inspector ��)")]
    public DialogueLine[] dialogueLines;  // �}�C�A�C�Ӥ����O�@����

    public float typingSpeed = 0.05f;   // ���r���t��

    private int currentLineIndex = 0;
    private bool isTyping = false;

    public bool isDialogueRunning { get; private set; } = false;
    #endregion

    //�ͩR�g��
    #region
    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // �קK���ƥͦ�
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);  // ���������ɤ��P��
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