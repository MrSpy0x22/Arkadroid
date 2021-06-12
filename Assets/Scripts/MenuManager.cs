using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Text StatsNumRow;
    public Text StatsNameRow;
    public Text StatsScoresRow;

    private void Start()
    {
        string path = GameManager.GetAppDataPath();
        Directory.CreateDirectory(path);
        if (!File.Exists(path + "/records.txt"))
        {
            File.Create(path + "/records.txt");
        }
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(1);
    }
    public void LoadMenuScene()
    {
        SceneManager.LoadScene(0);
    }
    public void UpdateStats()
    {
        string[] table = new string[] { "" , "" , "" };
        int cnt = 1;

        foreach (Record r in GameManager.LoadRecords())
        {
            table[0] += string.Format("{0}\n" , cnt);
            table[1] += string.Format("{0}\n", r.Name);
            table[2] += string.Format("{0}\n", r.Scores);
            
            cnt++;
        }

        StatsNumRow.text = table[0];
        StatsNameRow.text = table[1];
        StatsScoresRow.text = table[2];
    }
}
