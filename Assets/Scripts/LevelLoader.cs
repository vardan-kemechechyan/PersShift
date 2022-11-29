using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Networking;

public class LevelLoader : MonoBehaviour
{
    bool isCheckConnection;
    bool connection;

    AsyncOperation operation;

    [SerializeField] Slider progressBar;
    [SerializeField] GameObject connectionPopup;

    void Start()
    {
        StartCoroutine(LoadAsyncronously());
        StartCoroutine("CheckConnection");
    }

    WaitForSeconds checkConnectionTimeout = new WaitForSeconds(1.0f);

    IEnumerator CheckConnection()
    {
        yield return new WaitUntil(() => FirebaseManager.IsInitialized);
        yield return new WaitUntil(() => FirebaseManager.IsFetchedRemoteConfig);
        yield return new WaitUntil(() => operation != null);

        bool internetRequired = false;

        // Disabled on current release
        //internetRequired = FirebaseManager.GetRemoteConfigBoolean("no_internet");

        if (internetRequired)
        {
            while (true)
            {
                using (UnityWebRequest www = UnityWebRequest.Get($"https://google.com"))
                {
                    www.timeout = 3;
                    yield return www.SendWebRequest();

                    if (www.result != UnityWebRequest.Result.Success)
                    {
                        connectionPopup.SetActive(true);
                    }
                    else
                    {
                        ContinueLoading();
                    }

                    yield return checkConnectionTimeout;
                }
            }
        }
        else 
        {
            ContinueLoading();
        }
    }

    private void ContinueLoading() 
    {
        SceneManager.MoveGameObjectToScene(FirebaseManager.GetInstance().gameObject, SceneManager.GetSceneByName("Main"));

        connectionPopup.SetActive(false);
        operation.allowSceneActivation = true;

        StopCoroutine("CheckConnection");
    }

    WaitForSeconds w = new WaitForSeconds(1.0f);

    IEnumerator LoadAsyncronously()
    {
        yield return new WaitForSeconds(1.0f);

        operation = SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);

        operation.allowSceneActivation = false;

        float progress = 0;

        while (operation.progress < 0.9f)
        {
            yield return w;

            progress = Mathf.Clamp01(operation.progress * 0.9f);
            progressBar.value = progress;
        }
    }
}

