using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Properties

    public static GameManager i;

    [Header("MainProperties")]
    [SerializeField] private int gameSceneIndex;

    [Header("Loading")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;

    [SerializeField] private string addText;

    [Header("Cover")]
    [SerializeField] private Image coverImage;

    [SerializeField] private List<Sprite> coverImageSprites = new();

    [Header("Transition")]
    [SerializeField] private Material transitionMat;
    [SerializeField] private float transitionTime = 1f;
    [SerializeField] private string propertyName = "_Progress";

    [SerializeField] private bool disableTransitionInMainMenu = true;

    List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

    private bool canTransition = false;

    #endregion

    #region Mono

    private void Awake()
    {
        if (i == null)
        {
            i = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        /*if (transitionMat.GetFloat(propertyName) != 0f)
        {
            transitionMat.SetFloat(propertyName, 0f);
        }*/

        Time.timeScale = 1f;

        progressBar = loadingScreen.GetComponentInChildren<Slider>();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        transitionMat.SetFloat(propertyName, 1f);
    }

    #endregion

    #region Scene Loading

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //DataPersistenceManager.i.LoadGame();

        if (canTransition)
        {
            loadingScreen.SetActive(false);

            StartCoroutine(TransitionInCoroutine());
        }

        canTransition = true;
    }

    #endregion

    #region Transitions
    private IEnumerator TransitionInCoroutine()
    {
        float currentTime = 0;
        while (currentTime < transitionTime)
        {
            currentTime += Time.unscaledDeltaTime;
            transitionMat.SetFloat(propertyName, Mathf.Clamp01(currentTime / transitionTime));
            yield return null;
        }
    }

    private IEnumerator TransitionOutCoroutine()
    {
        float currentTime = transitionTime;
        while (currentTime > 0f)
        {
            currentTime -= Time.unscaledDeltaTime;
            transitionMat.SetFloat(propertyName, Mathf.Clamp01(currentTime / transitionTime));
            yield return null;
        }
    }
    #endregion

    #region Main Methods
    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;

        StartCoroutine(LoadScene(0));
    }

    public void StartGame()
    {
        Time.timeScale = 1f;

        StartCoroutine(LoadScene(gameSceneIndex));
    }

    public void StartGameByIndex(int index)
    {
        Time.timeScale = 1f;

        StartCoroutine(LoadScene(index));
    }

    public void StartGameByName(string name)
    {
        Time.timeScale = 1f;

        int index = Helper.GetSceneIndexByName(name);
        StartCoroutine(LoadScene(index));
    }

    public void Restart()
    {
        Time.timeScale = 1f;

        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex));
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion

    private IEnumerator LoadScene(int index)
    {
        progressBar.value = 0f;
        progressText.text = 0f + addText;

        StartCoroutine(TransitionOutCoroutine());

        yield return new WaitForSeconds(transitionTime);

        loadingScreen.SetActive(true);

        //SelectRandomCoverImage();

        yield return new WaitForSeconds(0.2f);

        AsyncOperation op = SceneManager.LoadSceneAsync(index);
        scenesLoading.Add(op);
        op.allowSceneActivation = false;

        StartCoroutine(GetSceneLoadProgress());
    }

    /*private void SelectRandomCoverImage()
    {
        int coverIndex = UnityEngine.Random.Range(0, coverImageSprites.Count);

        coverImage.sprite = coverImageSprites[coverIndex];
    }*/

    public IEnumerator GetSceneLoadProgress()
    {
        for (int i = 0; i < scenesLoading.Count; i++)
        {
            scenesLoading[i].allowSceneActivation = false;

            // 1. Load Unity scene
            while (scenesLoading[i].progress < 0.9f)
            {
                float progress = Mathf.Clamp01(scenesLoading[i].progress / 0.9f) * 0.9f;

                progressBar.value = progress;
                progressText.text = Math.Round((double)progress, 2) * 100f + addText;

                yield return null;
            }

            // 2. Do your custom loading for last 10%
            float customProgress = 0f;
            float customDuration = transitionTime; // how long your extra work takes
            float timer = 0f;

            while (timer < customDuration)
            {
                timer += Time.deltaTime;
                customProgress = Mathf.Clamp01(timer / customDuration);

                // Blend Unity’s 90% with your 10%
                progressBar.value = 0.9f + customProgress * 0.1f;
                progressText.text = Math.Round((double)(0.9f + customProgress * 0.1f), 2) * 100f + addText;

                yield return null;
            }

            // 3. Activate the scene
            scenesLoading[i].allowSceneActivation = true;
        }
    }
}