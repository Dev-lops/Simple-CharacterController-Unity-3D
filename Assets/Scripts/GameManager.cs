using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    [Range(1f,100f)]
    public float mouseSensiblity;
    private void Awake() {
        if(!inst){
            inst = this;
            DontDestroyOnLoad(gameObject);
        }else Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
