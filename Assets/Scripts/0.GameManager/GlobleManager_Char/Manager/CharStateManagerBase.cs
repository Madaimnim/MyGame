using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//模板，因為是abstract所以沒有實體，其內容也沒有實際的statesDtny，須從繼承的實體中，取用各自的statsDtny
public abstract class CharStateManagerBase<TStat,TSkill> : MonoBehaviour 
    where TStat : CharStats<TSkill>
    where TSkill : SkillBase 
{
    protected Dictionary<int, TStat> statesDtny = new Dictionary<int, TStat>();

    protected abstract bool IsValidId(int id);

    //將List<T>傳入，並存進statsDtny( <T>.Id ， <T> )
    protected void SetStates(IEnumerable<TStat> statsList) {
        statesDtny.Clear();
        foreach (var stat in statsList)
        {
            if (!IsValidId(stat.Id))
            {
                Debug.LogError($"[{GetType().Name}] Id {stat.Id} 不在合法區間");
                continue;
            }
            if (statesDtny.ContainsKey(stat.Id))
            {
                Debug.LogError($"[{GetType().Name}] 發現重複的 Id {stat.Id}");
                continue;
            }

            statesDtny[stat.Id] = stat;
        }
    }

    public TStat GetState(int id) {
        if (statesDtny.TryGetValue(id, out var state))
        {
            return state;
        }
        Debug.LogError($"[{GetType().Name}] 找不到 Id {id} 的狀態");
        return null;
    }

    public bool TryGetState(int id, out TStat state) {
        return statesDtny.TryGetValue(id, out state);
    }
}

