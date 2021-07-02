using Ink.Runtime;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;

using Firebase.Auth;
using Firebase.Database;

public class GameControls : MonoBehaviour
{
    public InternetConnectionControls internetConnectionControls;
    public TextAsset inkJSON;
    private Story story;

    public Text storyText;
    public GameObject choiceView;
    public GameObject choiceViewPanel;
    public Button buttonPrefab;
    public GameObject canvasStart;
    public GameObject canvasPause;
    public GameObject canvas;
    public GameObject canvasTheEnd;
    private bool show = false;

    private bool isDownloadedStory = false;
    private string loadedStoryState = "";
    private string loadedStoryText = "";

    public GameObject loadingText;
    public GameObject goMenuButton;
    public GameObject characterPanel;

    public Text playerBravery;
    public Text playerCunning;
    public Text playerIntelligence;
    public Text playerKindness;
    public Text playerFaculty;
    public Text playerGalleons;
    public Text playerPoints;

    private int pointsBefore = 0;
    private int pointsFirebase = 0;

    private async void ResetFacultiesPoints()
    {
        string faculty = "";
        int myPoints = 0;

        await FirebaseDatabase.DefaultInstance.GetReference("players").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId)
          .GetValueAsync().ContinueWith(task =>
          {
              if (task.IsFaulted)
              {
              }
              else if (task.IsCompleted)
              {
                  DataSnapshot snapshot = task.Result;
                  myPoints = Convert.ToInt32(snapshot.Child("points").Value);
                  faculty = GetFacultyKeyFromName((snapshot.Child("faculty").Value).ToString());
              }
          });

        await FirebaseDatabase.DefaultInstance.GetReference("facultiesPoints").Child(faculty)
            .GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    pointsFirebase = Convert.ToInt32(snapshot.Value);
                }
            });

        if (myPoints != 0)
        {
            await FirebaseDatabase.DefaultInstance.GetReference("facultiesPoints").Child(faculty)
              .GetValueAsync().ContinueWith(task =>
              {
                  if (task.IsFaulted)
                  {
                  }
                  else if (task.IsCompleted)
                  {
                      DataSnapshot snapshot = task.Result;
                      pointsFirebase = Convert.ToInt32(snapshot.Value);
                  }
              });

            await FirebaseDatabase.DefaultInstance.RootReference.Child("facultiesPoints").Child(faculty).SetRawJsonValueAsync((pointsFirebase - myPoints).ToString());
        }
    }

    private async void SetFacultiesPoints()
    {
        string faculty = GetFacultyKeyFromName(story.variablesState["faculty"].ToString());

        int differencePoints = Convert.ToInt32(story.variablesState["points"]) - pointsBefore;

        if (differencePoints != 0)
        {
            await FirebaseDatabase.DefaultInstance.GetReference("facultiesPoints").Child(faculty)
              .GetValueAsync().ContinueWith(task =>
              {
                  if (task.IsFaulted)
                  {
                  }
                  else if (task.IsCompleted)
                  {
                      DataSnapshot snapshot = task.Result;
                      pointsFirebase = Convert.ToInt32(snapshot.Value);
                  }
              });

            await FirebaseDatabase.DefaultInstance.RootReference.Child("facultiesPoints").Child(faculty).SetRawJsonValueAsync((pointsFirebase + differencePoints).ToString());
        }
    }

    async void CheckUserSaved()
    {
        bool isSave = false;

        if (!internetConnectionControls.IsInternetConnection())
        {
            internetConnectionControls.ShowInternetConnectionErrorToast();
            SceneManager.LoadScene("MainMenu");
        }

        await FirebaseDatabase.DefaultInstance.GetReference("saves").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId)
          .GetValueAsync().ContinueWith(task =>
          {
              if (task.IsFaulted)
              {
              }
              else if (task.IsCompleted)
              {
                  DataSnapshot snapshot = task.Result;
                  if (snapshot.Value != null)
                  {
                      isSave = true;
                  }
              };
          });

        if (isSave)
        {
            canvasStart.SetActive(isSave);
        }
        else
        {
            LoadNewGame();
        }
    }

    public void LoadNewGame()
    {
        canvasStart.SetActive(false);
        PrepareNewGame();
        RefreshTextAndChoicePanel();
    }

    private void PrepareNewGame()
    {
        ResetFacultiesPoints();
        RedistributePlayer();
        DeleteStoryProgress();
    }

    private async void DeleteStoryProgress()
    {
        if (!internetConnectionControls.IsInternetConnection())
        {
            UnityAndroidExtras.instance.makeToast("Ошибка! Отсутствует интернет-сосединение. Ваш прогресс не будет сохранен.", 0);
            return;
        }

        await FirebaseDatabase.DefaultInstance.RootReference.Child("saves").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).RemoveValueAsync();
    }

    private async void SaveStoryProgress()
    {
        if (!internetConnectionControls.IsInternetConnection())
        {
            UnityAndroidExtras.instance.makeToast("Ошибка! Отсутствует интернет-сосединение. Ваш прогресс не будет сохранен.", 0);
            return;
        }

        string saved = story.state.ToJson() + "";

        await FirebaseDatabase.DefaultInstance.RootReference.Child("saves").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("state").SetValueAsync(saved);
    }

    private string GetPlayerJson()
    {
        return JsonUtility.ToJson(new Player(story.variablesState["bravery"].ToString(),
            story.variablesState["cunning"].ToString(),
            story.variablesState["intelligence"].ToString(),
            story.variablesState["kindness"].ToString(),
            story.variablesState["faculty"].ToString(),
            story.variablesState["galleons"].ToString(),
        story.variablesState["points"].ToString()));
    }

    private void SavePlayerParameters()
    {
        FirebaseDatabase.DefaultInstance.RootReference.Child("players").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).SetRawJsonValueAsync(GetPlayerJson());
    }

    private async void RedistributePlayer()
    {
        string facultyFirebaseNewKey = "";
        int facultyFirebaseNewValue = 0;

        string facultyFirebaseOldKey = "";
        int facultyFirebaseOldValue = 0;

        facultyFirebaseNewKey = GetFacultyKeyFromName(story.variablesState["faculty"].ToString());

        await FirebaseDatabase.DefaultInstance.RootReference.Child("players").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("faculty").GetValueAsync()
          .ContinueWith(task =>
          {
              if (task.IsFaulted)
              {
              }
              else if (task.IsCompleted)
              {
                  DataSnapshot snapshot = task.Result;
                  facultyFirebaseOldKey = snapshot.Value.ToString();
                  facultyFirebaseOldKey = GetFacultyKeyFromName(facultyFirebaseOldKey);
              }
          });

        if (facultyFirebaseOldKey == facultyFirebaseNewKey)
        {
            SavePlayerParameters();
            return;
        }

        await FirebaseDatabase.DefaultInstance.GetReference("faculties").Child(facultyFirebaseOldKey)
          .GetValueAsync().ContinueWith(task =>
          {
              if (task.IsFaulted)
              {
              }
              else if (task.IsCompleted)
              {
                  DataSnapshot snapshot = task.Result;
                  facultyFirebaseOldValue = Convert.ToInt32(snapshot.Value) - 1;
              }
          });

        await FirebaseDatabase.DefaultInstance.GetReference("faculties").Child(facultyFirebaseNewKey)
          .GetValueAsync().ContinueWith(task =>
          {
              if (task.IsFaulted)
              {
              }
              else if (task.IsCompleted)
              {
                  DataSnapshot snapshot = task.Result;
                  facultyFirebaseNewValue = Convert.ToInt32(snapshot.Value) + 1;
              };
          });

        await FirebaseDatabase.DefaultInstance.RootReference.Child("faculties").Child(facultyFirebaseOldKey).SetRawJsonValueAsync(facultyFirebaseOldValue.ToString());
        await FirebaseDatabase.DefaultInstance.RootReference.Child("faculties").Child(facultyFirebaseNewKey).SetRawJsonValueAsync(facultyFirebaseNewValue.ToString());
        SavePlayerParameters();
    }

    public async void LoadStoryFromSave()
    {
        if (!internetConnectionControls.IsInternetConnection())
        {
            internetConnectionControls.ShowInternetConnectionErrorToast();
            SceneManager.LoadScene("MainMenu");
        }

        await FirebaseDatabase.DefaultInstance.GetReference("saves").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("state")
          .GetValueAsync().ContinueWith(task =>
          {
              if (task.IsFaulted)
              {
              }
              else if (task.IsCompleted)
              {
                  DataSnapshot snapshot = task.Result;
                  loadedStoryState = snapshot.Value + "";
                  isDownloadedStory = true;
              };
          });

        story.state.LoadJson(loadedStoryState);
        canvasStart.SetActive(false);
        RefreshTextAndChoicePanel();
    }

    void Start()
    {
        story = new Story(inkJSON.text);
        CheckUserSaved();
    }

    string GetFacultyKeyFromName(string facultyName)
    {
        switch (facultyName)
        {
            case "Слизерин":
                return "slytherin";
            case "Гриффиндор":
                return "gryffindor";
            case "Хаффлпафф":
                return "hufflpuf";
            case "Равенкло":
                return "ravenklow";
            default:
                return "noFaculty";
        }
    }

    string RefactoredStoryText(string storyChunk)
    {
        string refactoredStoryChunk = "";
        String[] text = storyChunk.Split(new char[] { '`' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string someText in text)
        {
            refactoredStoryChunk += someText;
            refactoredStoryChunk += "\n\n";
        }

        return refactoredStoryChunk;
    }

    void RefreshTextAndChoicePanel()
    {
        EraseChoicePanel();

        if (isDownloadedStory)
        {
            storyText.text = RefactoredStoryText(story.currentText);
            isDownloadedStory = false;
        }

        else
        {
            string s = LoadStoryChunk();
            storyText.text = RefactoredStoryText(s);
        }

        foreach (Choice choice in story.currentChoices)
        {
            Button choiceButton = Instantiate(buttonPrefab) as Button;
            choiceButton.transform.SetParent(choiceViewPanel.transform, false);
            Text choiceText = choiceButton.GetComponentInChildren<Text>();
            choiceText.text = choice.text;

            choiceButton.onClick.AddListener(delegate
            {
                pointsBefore = Convert.ToInt32(story.variablesState["points"]);
                ChooseStoryChoice(choice);
            });
        }

        if (story.currentChoices.Count > 0)
        {
        }
        else
        {
            canvasTheEnd.SetActive(true);
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void EraseChoicePanel()
    {
        for (int i = 0; i < choiceViewPanel.transform.childCount; i++)
        {
            Destroy(choiceViewPanel.transform.GetChild(i).gameObject);
        }
    }

    void ChooseStoryChoice(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);
        RefreshTextAndChoicePanel();
        SetFacultiesPoints();

        RedistributePlayer();
        SaveStoryProgress();
    }

    public void SwitchCanvasPause()
    {
        show = !show;
        canvasPause.SetActive(show);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPlayerParametersCanvasPause();
            SwitchCanvasPause();
        }
    }

    void SetPlayerParametersCanvasPause()
    {
        playerBravery.text = "Храбрость: " + story.variablesState["bravery"];
        playerCunning.text = "Хитрость: " + story.variablesState["cunning"];
        playerIntelligence.text = "Интеллект: " + story.variablesState["intelligence"];
        playerKindness.text = "Мягкость: " + story.variablesState["kindness"];
        playerFaculty.text = "Факультет: " + story.variablesState["faculty"];
        playerGalleons.text = "Галлеонов: " + story.variablesState["galleons"];
        playerPoints.text = "Заработано баллов: " + story.variablesState["points"];
    }

    string LoadStoryChunk()
    {
        string text = "";
        if (story.canContinue)
        {
            text = story.ContinueMaximally();
        }
        return text;
    }
}
