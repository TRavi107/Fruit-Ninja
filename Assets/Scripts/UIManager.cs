using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RamailoGames;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public List<UIPanel> uiPanels;
    public GameObject playCircle;

    public UIPanel activeUIPanel;
    public GameObject soundBtn;
    public GameObject musicBtn; 
    public GameObject pausesoundBtn;
    public GameObject pausemusicBtn;

    public Canvas effectCanvas;

    public GameObject ComboObjPrefab;

    private GameObject comboSpwaned;

    [Space(10)]
    [Header("Only for Main Menu")]
    public TMP_Text highscoreText;
    public GameObject mainMenuFruit;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        SwitchCanvas(uiPanels[0].uiPanelType);
        if (activeUIPanel.uiPanelType == UIPanelType.mainmenu)
        {
            
            GetHighScore();
        }
        soundManager.instance.PlaySound(SoundType.backgroundSound);
        if(soundManager.instance.backGroundAudioVolume == 0)
        {
            EnableDisableMusicBtns(musicBtn, pausemusicBtn, 0.5f);
        }
        else
        {
            EnableDisableMusicBtns(musicBtn, pausemusicBtn, 1f);
        }

        if (soundManager.instance.soundeffectVolume == 0)
        {
            EnableDisableMusicBtns(soundBtn, pausesoundBtn, 0.5f);
        }
        else
        {
            EnableDisableMusicBtns(soundBtn, pausesoundBtn, 1f);
            
        }

    }
    private void Update()
    {
        if(playCircle != null)
        {
            playCircle.transform.Rotate(new Vector3(0, 0, -10 * Time.deltaTime));
        }
    }

    private void EnableDisableMusicBtns(GameObject MainGameBTN,GameObject pauseMenuBTN,float alphaVAlue)
    {
        MainGameBTN.GetComponent<Image>().color = new Color(
                MainGameBTN.GetComponent<Image>().color.r,
                MainGameBTN.GetComponent<Image>().color.g,
                MainGameBTN.GetComponent<Image>().color.b,
                alphaVAlue);

        if (pauseMenuBTN == null)
            return;
        pauseMenuBTN.GetComponent<Image>().color = new Color(
                pauseMenuBTN.GetComponent<Image>().color.r,
                pauseMenuBTN.GetComponent<Image>().color.g,
                pauseMenuBTN.GetComponent<Image>().color.b,
                alphaVAlue);
    }
    void GetHighScore()
    {
        ScoreAPI.GetData((bool s, Data_RequestData d) => {
            if (s)
            {
                highscoreText.text = d.high_score.ToString();
            }
        });
    }


    public void SwitchCanvas(UIPanelType targetPanel)
    {


        foreach (UIPanel panel in uiPanels)
        {

            if (panel.uiPanelType == targetPanel)
            {
                
                activeUIPanel = panel;
            }
            else
            {
                if (mainMenuFruit != null)
                    mainMenuFruit.SetActive(false);
                panel.gameObject.SetActive(false);
            }
        }
        
        activeUIPanel.gameObject.SetActive(true);
        if (activeUIPanel.uiPanelType == UIPanelType.mainmenu)
        {
            if (mainMenuFruit != null)
                mainMenuFruit.SetActive(true);
        }
    }

    public void OnMusicBTNClickded()
    {
        if(musicBtn.GetComponent<Image>().color.a == 1)
        {
            EnableDisableMusicBtns(musicBtn, pausemusicBtn, 0.5f);


            soundManager.instance.MusicVolumeChanged(0);
        }
        else
        {
            EnableDisableMusicBtns(musicBtn, pausemusicBtn, 1f);

            soundManager.instance.MusicVolumeChanged(1);
        }
    }

    public void OnSoundBTNClickded()
    {
        if (soundBtn.GetComponent<Image>().color.a == 1)
        {
            EnableDisableMusicBtns(soundBtn, pausesoundBtn, 0.5f);

            soundManager.instance.SoundVolumeChanged(0);
        }
        else
        {
            EnableDisableMusicBtns(soundBtn, pausesoundBtn, 1f);
            soundManager.instance.SoundVolumeChanged(1);
        }
    }

    public void ShowCombo(Vector3 pos,int combotext)
    {
        StopCoroutine("AutoDisableCombo");
        if(comboSpwaned==null)
            comboSpwaned = Instantiate(ComboObjPrefab, effectCanvas.transform);
        EnableCombo();
        comboSpwaned.transform.position = pos;
        comboSpwaned.GetComponent<TMP_Text>().text ="Combo\n" +combotext.ToString();
        StartCoroutine("AutoDisableCombo");
    }

    public void DisableCombo()
    {
        if (comboSpwaned != null)
            comboSpwaned.SetActive(false);
    }
    public void EnableCombo()
    {
        if (comboSpwaned != null)
            comboSpwaned.SetActive(true);
    }

    IEnumerator AutoDisableCombo()
    {
        yield return new WaitForSeconds(1);
        DisableCombo();
        // Code to execute after the delay
    }

}
