using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange:MonoBehaviour
{
    // !Variables!
    // The current scene's index value which is used to identify each scene and thier order.
    [SerializeField] int sceneNumber; 

    // Start is called before the first frame update
    public void Start()  
    {
        // Finds the current scene's index value and sets int 'sceneNumber' as the index value.
        sceneNumber = SceneManager.GetActiveScene().buildIndex; 
    }

    // !Methods!
    // Loads the main menu scene.
    public void MainMenu() 
    {
        SceneManager.LoadScene(0);
    }
    // Loads the 'Intro1' scene.
    public void NewGame() 
    {
        SceneManager.LoadScene(1);
    }
    // Loads the 'PrimordialPrarie' scene
    public void Level1()
    {
        SceneManager.LoadScene(4);
    }
   
    // Loads the scene after this one.
    public void NextScene() 
    {
        SceneManager.LoadScene(sceneNumber+1);
    }
    // Restarts the current scene.
    public void Restart()
    {
        SceneManager.LoadScene(sceneNumber);
    }
    // Exits the game.
    public void Quit()
    {
        Application.Quit();
    }
}
