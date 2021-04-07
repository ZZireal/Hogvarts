using System;
using UnityEngine;
using UnityEngine.UI;

using Firebase.Auth;
using Firebase.Database;

public class StatisticsControls : MonoBehaviour
{
    public InternetConnectionControls internetConnectionControls;
    public GameObject canvasStatistics;
    public Text playersCount;
    public Text playersGryffindor;
    public Text playersSlytherin;
    public Text playersHufflpuf;
    public Text playersRavenklow;

    public GameObject userStatisticsPanel;
    public Text userInfo;
    public Text userBravery;
    public Text userCunning;
    public Text userIntelligence;
    public Text userKindness;
    public Text userFaculty;

    public GameObject loadingCanvas;

    private string playersAllString;
    private string playersGryffindorString;
    private string playersSlytherinString;
    private string playersHufflpufString;
    private string playersRavenklowString;

    private string userFacultyString;
    private string userBraveryString;
    private string userCunningString;
    private string userIntelligenceString;
    private string userKindnessString;

    public void ShowCanvasStatistics()
    {
        if (!internetConnectionControls.IsInternetConnection())
        {
            internetConnectionControls.ShowInternetConnectionErrorToast();
            return;
        }

        loadingCanvas.SetActive(true);

        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            userStatisticsPanel.SetActive(true);
        }
        else
        {
            userStatisticsPanel.SetActive(false);
        }

        SetUserAndFacultyStatistics();
    }

    public void CloseCanvasStatistics()
    {
        canvasStatistics.SetActive(false);
    }

    public void ResetUserStatisticsUI()
    {
        userFaculty.text = "";
        userBravery.text = "";
        userCunning.text = "";
        userIntelligence.text = "";
        userKindness.text = "";
    }

    public async void SetUserAndFacultyStatistics()
    {
        await FirebaseDatabase.DefaultInstance.GetReference("faculties")
          .GetValueAsync().ContinueWith(task =>
          {
              if (task.IsFaulted)
              {
              }
              else if (task.IsCompleted)
              {
                  DataSnapshot snapshot = task.Result;

                  playersGryffindorString = snapshot.Child("gryffindor").Value.ToString();
                  playersSlytherinString = snapshot.Child("slytherin").Value.ToString();
                  playersRavenklowString = snapshot.Child("ravenklow").Value.ToString();
                  playersHufflpufString = snapshot.Child("hufflpuf").Value.ToString();
                  playersAllString = 
                  (Convert.ToInt32(playersGryffindorString) +
                  Convert.ToInt32(playersSlytherinString) + 
                  Convert.ToInt32(playersRavenklowString) + 
                  Convert.ToInt32(playersHufflpufString)).ToString();
                  ;
              };
          });

        playersCount.text = playersAllString;
        playersGryffindor.text = playersGryffindorString;
        playersRavenklow.text = playersRavenklowString;
        playersSlytherin.text = playersSlytherinString;
        playersHufflpuf.text = playersHufflpufString;

        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            await FirebaseDatabase.DefaultInstance.GetReference("players").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId)
            .GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                   
                    userFacultyString = snapshot.Child("faculty").Value.ToString();
                    userBraveryString = snapshot.Child("bravery").Value.ToString();
                    userCunningString = snapshot.Child("cunning").Value.ToString();
                    userKindnessString = snapshot.Child("kindness").Value.ToString();
                    userIntelligenceString = snapshot.Child("intelligence").Value.ToString();
                }
            });

            userFaculty.text = "Факультет: " + userFacultyString;
            userBravery.text = userBraveryString;
            userCunning.text = userCunningString;
            userKindness.text = userKindnessString;
            userIntelligence.text = userIntelligenceString;
        }

        canvasStatistics.SetActive(true);
        loadingCanvas.SetActive(false);
    }
}
