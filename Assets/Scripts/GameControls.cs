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
        RedistributePlayer();
        SaveStoryProgress();
    }

    private void SaveStoryProgress()
    {
        if (!internetConnectionControls.IsInternetConnection())
        {
            UnityAndroidExtras.instance.makeToast("Ошибка! Отсутствует интернет-сосединение. Ваш прогресс не будет сохранен.", 0);
            return;
        }
        string saved = story.state.ToJson() + "";
        FirebaseDatabase.DefaultInstance.RootReference.Child("saves").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("state").SetValueAsync(saved);
        FirebaseDatabase.DefaultInstance.RootReference.Child("saves").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("story").SetValueAsync(storyText.text + "");
    }

    private string GetPlayerJson()
    {
        return JsonUtility.ToJson(new Player(story.variablesState["bravery"].ToString(),
            story.variablesState["cunning"].ToString(),
            story.variablesState["intelligence"].ToString(),
            story.variablesState["kindness"].ToString(),
            story.variablesState["faculty"].ToString()));
    }

    private void SavePlayerParameters()
    {
        FirebaseDatabase.DefaultInstance.RootReference.Child("players").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).SetRawJsonValueAsync(GetPlayerJson());
    }

    private async void RedistributePlayer() {
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

        await FirebaseDatabase.DefaultInstance.GetReference("saves").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("story")
          .GetValueAsync().ContinueWith(task =>
          {
              if (task.IsFaulted)
              {
              }
              else if (task.IsCompleted)
              {
                  DataSnapshot snapshot = task.Result;
                  isDownloadedStory = true;
                  loadedStoryText = snapshot.Value + "";
              };
          });

        storyText.text = loadedStoryText;

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

    void RefreshTextAndChoicePanel()
    {
        EraseChoicePanel();

        // GameObject newChoiceView = Instantiate(choiceViewPrefab) as GameObject;
        // newChoiceView.transform.SetParent(this.transform, false);

        if (isDownloadedStory)
        {
            storyText.text = loadedStoryText;
            isDownloadedStory = false;
        }
        else
        {
            storyText.text = LoadStoryChunk();
        }

        foreach (Choice choice in story.currentChoices)
        {
            Button choiceButton = Instantiate(buttonPrefab) as Button;
            choiceButton.transform.SetParent(choiceViewPanel.transform, false);
            Text choiceText = choiceButton.GetComponentInChildren<Text>();
            choiceText.text = choice.text;

            choiceButton.onClick.AddListener(delegate
            {
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
