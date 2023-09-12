using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAvatarLobby : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int ii = 0; ii < transform.childCount; ii++)
        {
            //set active meshes
            if (ii != PlayerInfo.PI.myMesh)
            {
                transform.GetChild(ii).gameObject.SetActive(false);
            }
            else
            {
                transform.GetChild(ii).gameObject.SetActive(true);


                //change colors of avatars
                for (int jj = 0; jj < transform.GetChild(ii).childCount; jj++)
                {
                    if (transform.GetChild(ii).GetChild(jj).GetComponent<Renderer>() != null)
                    {
                        if (PlayerInfo.PI.myTeam == 0)
                        {
                            transform.GetChild(ii).GetChild(jj).GetComponent<Renderer>().material.color = PlayerInfo.PI.colAlpha;
                        }
                        else
                        {
                            transform.GetChild(ii).GetChild(jj).GetComponent<Renderer>().material.color = PlayerInfo.PI.colBravo;
                        }
                    }
                }
            }
        }
        
    }
}
