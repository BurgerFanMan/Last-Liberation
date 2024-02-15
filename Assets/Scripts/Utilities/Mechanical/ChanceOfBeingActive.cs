using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceOfBeingActive : MonoBehaviour
{
    [Range(0f, 1f)]
    public float chanceOfBeingActive; // the chance of each object being active
    public bool mustBeActive; // if true then chanceOfBeingActive is the percentage of objects which will be active
    public List<GameObject> objectsToActivate;

    void Awake()
    {
        if(mustBeActive)
        {
            int numbOfActiveObjects = (int)Mathf.Floor(objectsToActivate.Count * chanceOfBeingActive);
            List<GameObject> inactiveObjects = new List<GameObject>(objectsToActivate);

            for(int i = 0; i < numbOfActiveObjects; i++)
            {
                int index = Random.Range(0, inactiveObjects.Count);

                inactiveObjects[index].SetActive(true);
                inactiveObjects.Remove(inactiveObjects[index]);
            }
        }
        else
        {
            for(int i = 0; i < objectsToActivate.Count; i++)
            {
                bool shouldBeActive = Random.Range(0f, 1f) < chanceOfBeingActive;

                if (shouldBeActive)
                    objectsToActivate[i].SetActive(true);
            }
        }
    }
}
