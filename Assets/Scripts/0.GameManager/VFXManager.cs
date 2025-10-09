using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }

    [System.Serializable]
    public class VFXPair
    {
        public string key;       
        public GameObject prefab;
    }

    [FormerlySerializedAs("EffectList")]
    public List<VFXPair> EffectList;
    private Dictionary<string, GameObject> _effectDtny = new();

    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var vfxPair in EffectList)
        {
            _effectDtny[vfxPair.key] = vfxPair.prefab;
        }
    }

    public void Play(string key, Vector3 position,SpriteRenderer targetRenderer=null) {
        if (!_effectDtny.ContainsKey(key))
        {
            Debug.LogWarning($"VFX {key} not found!");
            return;
        }

        GameObject vfx = Instantiate(_effectDtny[key], position, Quaternion.identity);

        // 如果有指定目標，把特效放在角色上層
        if (targetRenderer != null)
        {
            var sr = vfx.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingLayerID = targetRenderer.sortingLayerID;
                sr.sortingOrder = targetRenderer.sortingOrder + 1;
            }
        }


        //  嘗試抓 Animator
        Animator animator = vfx.GetComponent<Animator>();
        if (animator != null)
        {
            float clipLength = animator.GetCurrentAnimatorStateInfo(0).length;
            Destroy(vfx, clipLength);
            return;
        }

        //  嘗試抓 ParticleSystem
        ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            Destroy(vfx, ps.main.duration + ps.main.startLifetime.constantMax);
            return;
        }

        // 如果沒 Animator / ParticleSystem，就保險加個 2 秒銷毀
        Destroy(vfx, 2f);
    }


}
