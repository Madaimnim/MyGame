using UnityEngine;

public interface IPlayerFactory
{
    GameObject CreatPlayer(PlayerStatsRuntime runtime,Transform parent);
}
