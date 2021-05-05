using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropsManager : MonoBehaviour
{
    #region Singleton
    private static DropsManager _instance = null;
    public static DropsManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    [Range(0, 100)]
    public float BuffChance;
    [Range(0, 100)]
    public float DebuffChance;
    public List<Drop> AvailableBuffs;
    public List<Drop> AvailableDebuffs;
}
