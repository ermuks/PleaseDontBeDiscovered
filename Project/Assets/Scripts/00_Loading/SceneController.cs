using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private void Awake()
    {
        EventManager.AddEvent("OpenScene", (p) =>
        {
            SceneManager.LoadScene((string)p[0]);
        });
    }
}
