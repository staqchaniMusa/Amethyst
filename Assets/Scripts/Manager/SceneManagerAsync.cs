using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Video;

/// <summary>
/// Contains the build scene references
/// </summary>
public enum sceneIndex
{
    persistent=0,
    lobby=1,
    multiplayer_01=2,
    multiplayer_02 = 3

}

/// <summary>
/// used to move beteen scenes --> ADDITIVE
/// </summary>
public class SceneManagerAsync : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject curved;

    public float timer=8;
    VideoPlayer vidP;
    //singleton
    public static SceneManagerAsync SM;

    //loading image
    public UnityEngine.UI.Image img;

    public Canvas canvas;
    // gameobejct that contains the loading bar
    public GameObject loadingBar;

    //loading progress
    float progress = 0;

    public bool isVR;

    #region UNITY FUNCTIONS
    void Start()
    {
        //set singleton
        SM = this;
        if (!isVR)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        //load first scene (lobby menu)        
        StartCoroutine(LoadAsyncSceneFromTo(-1,(int)sceneIndex.lobby, LoadSceneMode.Additive));

        // loadingBar.SetActive(true);
        if (curved.GetComponent<VideoPlayer>().clip == null)
        {
            img.gameObject.SetActive(true);
        }
        else
        {
            img.gameObject.SetActive(false);
        }

        Application.backgroundLoadingPriority = ThreadPriority.Low;
    }
    #endregion

    #region LOADING ADDITIVE FUNCTIONS
    //this loads a scene from a given scene if indx!=-1. If indx=-1 it loads additive
    IEnumerator LoadAsyncSceneFromTo(int idx1, int idx2, LoadSceneMode load)
    {
        float elapsed = 0;

        while(elapsed<timer)
        {
            img.fillAmount =elapsed/timer;

            elapsed += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }


        curved.SetActive(false);

        //activate the canvas with the loading effect
        yield return null;
      

        //case of input equal to -1, load additive
        if (idx1 == -1)
        {
            //create reference to the async. operation
           AsyncOperation loading = SceneManager.LoadSceneAsync(idx2, load);
                
            loading.allowSceneActivation = false;
            while (!loading.isDone )
            {
                //create loading effect
                yield return new WaitForFixedUpdate();
                progress += Time.fixedDeltaTime/3;
                if(progress>1)
                {
                    progress = 0;
                }
                img.fillAmount = progress;

                if (loading.progress == 0.9f)
                {
                    loading.allowSceneActivation = true;
                }
            }

            //set to inactive when loading
            loadingBar.SetActive(false);

            //activate the other scene to spawn objects
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(idx2));
        }

        //case indx!=-1, unload and load new additive scene
        else
        {
            // if we want to unload scenes to go from one to another use indx1!=-1
            AsyncOperation unloading = SceneManager.UnloadSceneAsync(idx1);
            AsyncOperation loading = SceneManager.LoadSceneAsync(idx2, load);

           
            loading.allowSceneActivation = false;
            
            while (!loading.isDone || !unloading.isDone)
            {

                //set the loading efect
                img.fillAmount = (loading.progress+unloading.progress)/2;
                yield return null;

                if (loading.progress >= 0.9f)
                {
                    loading.allowSceneActivation = true;
                    
                }
            }
            loadingBar.SetActive(false);

            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(idx2));

        }
        
       
    }

    private void FixedUpdate()
    {

    }
    #endregion

    #region MOVING BETWEEN SCENES
    public void GoToMultiplayerScene(int a)
    {
        //Destroy(PhotonRoom2.room.gameObject);
        //StartCoroutine(LoadAsyncSceneFromTo((int)sceneIndex.lobby,(int)sceneIndex.multiplayer, LoadSceneMode.Additive));
        if (a == 0)
        {
            PhotonNetwork.LoadLevel((int)sceneIndex.multiplayer_01);
        }
        else if (a == 1)
        {
            PhotonNetwork.LoadLevel((int)sceneIndex.multiplayer_02);
        }
    }

    public void GoToLobby()
    {
        //StartCoroutine(LoadAsyncSceneFromTo(-1, 0, LoadSceneMode.Single));
        PhotonNetwork.LoadLevel((int)sceneIndex.lobby);
    }
    #endregion
}
