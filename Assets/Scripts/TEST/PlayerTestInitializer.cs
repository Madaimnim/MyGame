using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTestInitializer : MonoBehaviour
{
    [Header("測試腳色設定")]
    public PlayerStatsTemplate PlayerStatsTemplate;
    public PlayerStatsRuntime Rt;
    public Player CurrentPlayer;

    public int SkillId;
    public bool Trigger_SetPlayers; // 點這個會觸發
    //public Button ResetPlayersButton;

    [Header("Debug監控設定")]
    public bool IsMonitorEnabled = true; // 是否啟用監視器
    public TextMeshProUGUI OutputText; // 顯示內容的 Text（或 TextMeshProUGUI）
    public float refreshInterval = 0.2f; // 每0.2秒更新一次
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
            Trigger_SetPlayers = false; // 重置
            Debug.Log("執行 SetPlayers()");
            SetPlayers(); // 呼叫你的方法
            EditorUtility.SetDirty(this);
        }
    }
#endif

    //[Button("執行 SetPlayers")]
    public void SetPlayers()
    {
        CurrentPlayer = FindObjectOfType<Player>();
        var runner = new CoroutineRunnerAdapter(CurrentPlayer);
        CurrentPlayer.Initialize(Rt);

        //裝配測試技能
        if (Rt.SkillPool.TryGetValue(SkillId, out var skill)) {
            CurrentPlayer.SkillComponent.EquipSkill(0, SkillId);
        }
    }


    private void Update()
    {
        if (Rt == null || OutputText == null) return;

        //分頁切換（Q、E）
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
        Debug.Log($"切換頁面 → {_pageNames[currentPage]}");
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

    // === 各頁面內容 ===
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
        sb.AppendLine($"技能槽1 ID:{CurrentPlayer.SkillComponent.SkillSlots[0].SkillId}");
        sb.AppendLine($"技能槽1 HasSkill:{CurrentPlayer.SkillComponent.SkillSlots[0].HasSkill}");
        sb.AppendLine($"技能槽1 DetectStrategy:{CurrentPlayer.SkillComponent.SkillSlots[0].DetectStrategy!=null}");
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
                sb.AppendLine($"  └ Power: {stat.Power}, MoveSpeed: {stat.MoveSpeed}, KnockbackPower: {stat.KnockbackPower}, FloatPower: {stat.FloatPower}, Weight: {stat.Weight}");
            }
        }
    }

    public void SetTarget(PlayerStatsRuntime runtime)
    {
        Rt = runtime;
        UpdateDisplay();
    }
}