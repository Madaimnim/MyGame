using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_PlayerController : MonoBehaviour
{
    public Transform PlayerUiParent;


    //指標指向的角色ID
    public int CurrentPlayerId => _currentPlayerId;
    private int _currentPlayerId = -1;

    private void OnEnable() {
        GameManager.Instance.OnAllDataLoaded += OnAllDataLoaded;
    }
    private void OnDisable() {
        GameManager.Instance.OnAllDataLoaded -= OnAllDataLoaded;
    }

    public void InitialUIPlayer() {
        var unlockedPlayerIDs = PlayerUtility.UnlockedIdList;
        if (unlockedPlayerIDs.Count == 0) return;
        if (!unlockedPlayerIDs.Contains(_currentPlayerId)) _currentPlayerId = unlockedPlayerIDs.First();
    } 

    //撥放角色動畫
    public void PlayUIAttackAnimation(string animationName) {
        var rt = PlayerUtility.Get(_currentPlayerId);
        rt.UiObject?.GetComponent<Animator>()?.Play(Animator.StringToHash(animationName));
    }

    private void OnAllDataLoaded() {
        InitialUIPlayer();
    }
}
