using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UITest : MonoBehaviour {

    [SerializeField]
    private Image arrow;

    [SerializeField]
    private Image puck;

    private bool inLoop = false;

    private bool shotPuck = false;

    [SerializeField]
    private Image goal;


    // Update is called once per frame
    void Update ()
    {
        if (!inLoop && !shotPuck)
        {
            StartCoroutine(ProcessLoop());
        }
        if (shotPuck && !inLoop && puck.transform.position.y < goal.transform.position.y)
        {

                Vector3 currentRot = arrow.transform.localEulerAngles;
                Quaternion rotation = arrow.transform.rotation;
                puck.transform.rotation = rotation;

                puck.transform.position += rotation * Vector3.up * 6.0f;
            

        }
    }

    IEnumerator ProcessLoop()
    {
        Vector3 currentRot = arrow.transform.localEulerAngles;
        inLoop = true;
        while ((currentRot.z < 40 || currentRot.z > 315 && !shotPuck))
        {
            currentRot.z += 1;
            arrow.transform.rotation = Quaternion.Euler(currentRot);
            yield return new WaitForSeconds(0.01f);
            currentRot= arrow.transform.localEulerAngles;
        }
        while (( currentRot.z > 320 || currentRot.z < 45) && !shotPuck)
        {
            currentRot.z -= 1;
            arrow.transform.rotation = Quaternion.Euler(currentRot);
            yield return new WaitForSeconds(0.01f);
            currentRot =arrow.transform.localEulerAngles;
        }
        inLoop = false;
    }

    public void ShootPuck()
    {
        shotPuck = true;
        
    }
}
