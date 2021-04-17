using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUsername : MonoBehaviour
{
    public string username;
    public LeaderboardManager leaderboardManager;
    public MasteryReportTeacher masteryReport;
    

    public Button button;
    void  Start()
    {
        leaderboardManager = GameObject.FindGameObjectWithTag("TeacherUI").GetComponent<LeaderboardManager>();     
        button.onClick.AddListener(TaskOnClick);
    }
    public void setUsername(string username)
    {
        this.username = username;
    }
    public void TaskOnClick()
    {
        masteryReport = GameObject.FindGameObjectWithTag("MasteryReport").GetComponent<MasteryReportTeacher>();
        masteryReport.DisplayMastery(leaderboardManager.userIDList[username]);
    }
}
