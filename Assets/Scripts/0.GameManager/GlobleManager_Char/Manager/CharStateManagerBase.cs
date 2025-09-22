using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ҪO�A�]���Oabstract�ҥH�S������A�䤺�e�]�S����ڪ�statesDtny�A���q�~�Ӫ����餤�A���ΦU�۪�statsDtny
public abstract class CharStateManagerBase<TStat,TSkill> : MonoBehaviour 
    where TStat : CharStats<TSkill>
    where TSkill : SkillBase 
{
    protected Dictionary<int, TStat> statesDtny = new Dictionary<int, TStat>();

    protected abstract bool IsValidId(int id);

    //�NList<T>�ǤJ�A�æs�istatsDtny( <T>.Id �A <T> )
    protected void SetStates(IEnumerable<TStat> statsList) {
        statesDtny.Clear();
        foreach (var stat in statsList)
        {
            if (!IsValidId(stat.Id))
            {
                Debug.LogError($"[{GetType().Name}] Id {stat.Id} ���b�X�k�϶�");
                continue;
            }
            if (statesDtny.ContainsKey(stat.Id))
            {
                Debug.LogError($"[{GetType().Name}] �o�{���ƪ� Id {stat.Id}");
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
        Debug.LogError($"[{GetType().Name}] �䤣�� Id {id} �����A");
        return null;
    }

    public bool TryGetState(int id, out TStat state) {
        return statesDtny.TryGetValue(id, out state);
    }
}

