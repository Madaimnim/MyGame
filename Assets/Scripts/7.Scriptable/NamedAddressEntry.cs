using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NamedAddressEntry 
{
    [Tooltip("語意化 Key，例如 Tutorial、第一關、MainMenu")]
    public string key;

    [Tooltip("Addressables 的 Address 名稱，必須跟 Inspector Address 欄位一致")]
    public string address;
}
