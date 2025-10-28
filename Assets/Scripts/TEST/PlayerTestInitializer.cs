using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTestInitializer : MonoBehaviour
{
    [Header("���ո}��]�w")]
    public PlayerStatsTemplate PlayerStatsTemplate;
    public PlayerStatsRuntime Rt;
    public Player CurrentPlayer;

    public int SkillId;
    public bool Trigger_SetPlayers; // �I�o�ӷ|Ĳ�o
    //public Button ResetPlayersButton;

    [Header("Debug�ʱ��]�w")]
    public bool IsMonitorEnabled = true; // �O�_�ҥκʵ���
    public TextMeshProUGUI OutputText; // ��ܤ��e�� Text�]�� TextMeshProUGUI�^
    public float refreshInterval = 0.2f; // �C0.2���s�@��
    private float _timer;
    public int currentPage = 0;  // 0 = Player, 1 = Skill


    private readonly string[] _pageNames = { "Player", "Skill" };

    private void Awake()
    {
        Rt = new PlayerStatsRuntime(PlayerStatsTemplate);
        //ResetPlayersButton.onClick.AddListener(SetPlayers);
    }


    private void Start()
    {
        SetPlayers();
    }



#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Trigger_SetPlayers)
        {
            Trigger_SetPlayers = false; // ���m
            Debug.Log("���� SetPlayers()");
            SetPlayers(); // �I�s�A����k
            EditorUtility.SetDirty(this);
        }
    }
#endif

    //[Button("���� SetPlayers")]
    public void SetPlayers()
    {
        CurrentPlayer = FindObjectOfType<Player>();
        var runner = new CoroutineRunnerAdapter(CurrentPlayer);
        CurrentPlayer.Initialize(Rt);

        //�˰t���էޯ�
        if (Rt.SkillPool.TryGetValue(SkillId, out var skill)) {
            CurrentPlayer.SkillComponent.EquipSkill(0, SkillId);
        }
    }


    private void Update()
    {
        if (Rt == null || OutputText == null) return;

        //���������]Q�BE�^
        if (Input.GetKeyDown(KeyCode.Q)) ChangePage(-1);
        if (Input.GetKeyDown(KeyCode.E)) ChangePage(1);

        _timer += Time.deltaTime;
        if (_timer < refreshInterval) return;
        _timer = 0f;

        if(IsMonitorEnabled)UpdateDisplay();
            else OutputText.text = "";
    }
    private void ChangePage(int dir)
    {
        currentPage = (currentPage + dir + _pageNames.Length) % _pageNames.Length;
        Debug.Log($"�������� �� {_pageNames[currentPage]}");
    }
    private void UpdateDisplay()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"<b>=== Page {_pageNames[currentPage]} ===</b>\n");

        switch (currentPage)
        {
            case 0:
                DisplayPlayerRuntime(sb);
                break;
            case 1:
                DisplaySkillData(sb);
                break;
        }

        OutputText.text = sb.ToString();
    }

    // === �U�������e ===
    private void DisplayPlayerRuntime(StringBuilder sb)
    {
        sb.AppendLine("<b>=== Player Runtime ===</b>");

        var s = Rt.StatsData;
        sb.AppendLine($"ID: {s.Id}");
        sb.AppendLine($"Name: {s.Name}");
        //sb.AppendLine($"Level: {s.Level}");
        //sb.AppendLine($"Power: {s.Power}");
        //sb.AppendLine($"MoveSpeed: {s.MoveSpeed}");
        //sb.AppendLine($"KnockbackPower: {s.KnockbackPower}");
        //sb.AppendLine($"FloatPower: {s.FloatPower}");
        //sb.AppendLine($"Weight: {s.Weight}");
        //sb.AppendLine($"HP: {Rt.CurrentHp}/{Rt.MaxHp}");
        //sb.AppendLine($"Exp: {Rt.Exp}");
        sb.AppendLine($"SkillSlotCount: {Rt.SkillSlotCount}");
        sb.AppendLine($"�ޯ��1 ID:{CurrentPlayer.SkillComponent.SkillSlots[0].SkillId}");
        sb.AppendLine($"�ޯ��1 HasSkill:{CurrentPlayer.SkillComponent.SkillSlots[0].HasSkill}");
        sb.AppendLine($"�ޯ��1 DetectStrategy:{CurrentPlayer.SkillComponent.SkillSlots[0].DetectStrategy!=null}");
        //sb.AppendLine($"CanRespawn: {Rt.CanRespawn}");
        //sb.AppendLine($"MoveStrategy: {Rt.MoveStrategy}");
        //sb.AppendLine($"Unlocked Skills: {string.Join(" , ", Rt.UnlockedSkillIdList)}");
    }

    private void DisplaySkillData(StringBuilder sb)
    {
        sb.AppendLine("<b>=== Skill Runtime ===</b>");
        foreach (var kv in Rt.SkillPool)
        {
            var skill = kv.Value as PlayerSkillRuntime;
            if (skill != null)
            {
                var stat = skill.StatsData;
                sb.AppendLine($"[Skill ID {kv.Key}] {stat.Name}");
                sb.AppendLine($"  Lv: {skill.SkillLevel}  Used: {skill.SkillUsageCount}/{skill.NextLevelCount}");
                sb.AppendLine($"  Cooldown: {skill.Cooldown}");
                sb.AppendLine($"  �| Power: {stat.Power}, MoveSpeed: {stat.MoveSpeed}, KnockbackPower: {stat.KnockbackPower}, FloatPower: {stat.FloatPower}, Weight: {stat.Weight}");
            }
        }
    }

    public void SetTarget(PlayerStatsRuntime runtime)
    {
        Rt = runtime;
        UpdateDisplay();
    }
}