using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class GameController : MonoBehaviour{

    public Image hudArrowIcon;
    public GameObject canvas;
    private GameObject quitParent;
    private GameObject resumeParent;
    private string selectedPause;
    [HideInInspector] public static bool isPaused = false;
    private GameObject BgMusic;

    private string[] engText = { "SELECT / ACTION", "GO BACK / CANCEL", "MENU\nCONTROL", "PLAYER\nCONTROL", "RESUME", "QUIT" };
    private string[] espText = { "SELECCION / ACCION", "ATRAS / CANCELAR", "CONTROL MENU", "CONTROL JUGADOR", "REANUDAR", "SALIR" };
    public TextMeshProUGUI[] textControls;

    [HideInInspector] public static Vector3 currentSpawnPoint = new Vector3(121f, 29.09f, 0f);

    Dictionary<string, string[]> fullLanguages = new Dictionary<string, string[]>();

    // Start is called before the first frame update
    void Start() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        BgMusic = GameObject.Find("BgMusic");
        selectedPause = "resume";
        fullLanguages.Add("esp", espText);
        fullLanguages.Add("eng", engText);
        var cont = 0;
        foreach (var textControl in textControls) {
            textControl.text = fullLanguages[MenuController.selectedLang][cont++];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
            PauseGame();

        if (Input.GetKeyDown(KeyCode.W) && canvas.activeSelf) {
            selectedPause = "resume";
            hudArrowIcon.transform.SetParent(resumeParent.transform, false);
        }

        if (Input.GetKeyDown(KeyCode.S) && canvas.activeSelf ) {
            selectedPause = "quit";
            hudArrowIcon.transform.SetParent(quitParent.transform, false);
        }

        if (Input.GetKeyDown(KeyCode.J) && canvas.activeSelf) {
            if (selectedPause == "resume") {
                PauseGame();
            } else {
                PauseGame();
                Destroy(BgMusic);
                SceneManager.LoadScene("MenuScene");
            }
        }
        

    }

    public void PauseGame() {
        if (canvas.activeSelf) {
            isPaused = !isPaused;
            canvas.SetActive(false);
            Time.timeScale = 1;
            
        } else {
            isPaused = !isPaused;
            canvas.SetActive(true);
            var cont = 0;
            foreach (var singleText in textControls) {
                singleText.text = fullLanguages[MenuController.selectedLang][cont++];
            }
            Time.timeScale = 0;
            resumeParent = GameObject.FindWithTag("ResumeMenu");
            quitParent = GameObject.FindWithTag("QuitMenu");
        }
    }

}
