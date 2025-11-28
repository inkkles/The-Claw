using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class QuitScript : MonoBehaviour
{
    public string returnToLauncher;
    bool launched;
    float ttkMax = 120; 
    float ttk = 60; 

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.O) || Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
        {
            ttk = ttkMax;
        } else 
        {
            ttk -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Q) || ttk <= 0)
        {
            LaunchAndKillSelf();
        }
    }

    public void LaunchAndKillSelf()
    {
        //move current directory
        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

        //start process
        if (!launched)
        {
            Process process = new Process();
            process.StartInfo.FileName = returnToLauncher;
            process.Start();

            //kill application
            Application.Quit();

            launched = true;
        }

    }
}
