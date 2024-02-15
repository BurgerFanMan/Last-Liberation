using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitRubble : MonoBehaviour
{
    [Range(1, 100)]
    public int maxRubbleCount;
    public float descendRate = -0.2f;
    public float timeToStartDescent = 4f;
    public float timeToForceRemove = 20f;
    public bool replaceOldRubble;

    public List<GameObject> rubble;

    private List<float> timeDone = new List<float>();

    private void Update()
    {
        List<int> rubbleToRemove = new List<int>();

        for(int i = 0; i < rubble.Count; i++)
        {
            GameObject go = rubble[i];

            timeDone[i] += Pause.adjTimeScale;

            if (timeDone[i] > timeToForceRemove)
            {
                rubbleToRemove.Add(i);

                continue;
            }

            if (timeDone[i] < timeToStartDescent)
                continue;

            go.transform.position += new Vector3(0f, descendRate * Pause.adjTimeScale, 0f);

            if (go.transform.position.y > -1f)
                continue;
            rubbleToRemove.Add(i);
        }

        for(int i = 0; i < rubbleToRemove.Count; i++)
        {
            RemoveRubble(rubbleToRemove[i]);
        }
    }

    public void AddRubble(GameObject enemyDestroy)
    {
        if(rubble.Count >= maxRubbleCount)
        {
            if (!replaceOldRubble)
            {
                Destroy(enemyDestroy);

                return;
            }
            else
                RemoveRubble(0);
        }


        rubble.Add(enemyDestroy);
        timeDone.Add(0f);

        Rigidbody[] rigidbodies = enemyDestroy.GetComponentsInChildren<Rigidbody>();

        for (int i = 0; i < rigidbodies.Length; i++)
        {
            Destroy(rigidbodies[i], timeToStartDescent);
        }
    }

    public void RemoveRubble(int index)
    {
        GameObject go = rubble[index];

        rubble.Remove(go);
        timeDone.RemoveAt(index);

        Destroy(go);
    }
}
