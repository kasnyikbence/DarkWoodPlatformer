//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;
//using System;

//public class MainMenuUI : MonoBehaviour
//{
//    [Header("Panelek")]
//    public GameObject mainPanel;      // Ahol a New Game / Continue van
//    public GameObject slotSelectionPanel; // Ahol a 3 slot gomb van

//    [Header("Gombok")]
//    public Button continueButton;
//    public TextMeshProUGUI continueButtonText;

//    [Header("Slot Szövegek")]
//    public TextMeshProUGUI slot1Text;
//    public TextMeshProUGUI slot2Text;
//    public TextMeshProUGUI slot3Text;

//    void Start()
//    {
//        ShowMainPanel();
//        CheckContinueButton();
//    }

//    // --- PANEL VÁLTÁS ---
//    private void ShowMainPanel()
//    {
//        mainPanel.SetActive(true);
//        slotSelectionPanel.SetActive(false);
//    }

//    public void OpenSlotSelection()
//    {
//        mainPanel.SetActive(false);
//        slotSelectionPanel.SetActive(true);
//        RefreshSlotInfos(); // Frissítjük a szövegeket
//    }

//    public void CloseSlotSelection()
//    {
//        ShowMainPanel();
//    }

//    // --- CONTINUE GOMB ELLENÕRZÉS ---
//    private void CheckContinueButton()
//    {
//        bool hasSave = false;

//        // Megnézzük, melyik slottal játszottunk utoljára
//        int lastSlot = PlayerPrefs.GetInt("LastPlayedSlot", -1);
//        if (lastSlot != -1 && GameManager.Instance != null)
//        {
//            // Létezik-e még a fájl?
//            string path = GameManager.Instance.GetSavePath(lastSlot);
//            if (File.Exists(path))
//            {
//                hasSave = true;
//            }
//        }

//        // Gomb aktiválása/színezése
//        continueButton.interactable = hasSave;
//        continueButtonText.color = hasSave ? Color.white : new Color(1f, 1f, 1f, 0.5f);
//    }

//    // --- SLOT INFO KIOLVASÁSA ---
//    private void RefreshSlotInfos()
//    {
//        UpdateSlotText(1, slot1Text);
//        UpdateSlotText(2, slot2Text);
//        UpdateSlotText(3, slot3Text);
//    }

//    private void UpdateSlotText(int slotId, TextMeshProUGUI textComp)
//    {
//        if (GameManager.Instance == null) return;

//        string path = GameManager.Instance.GetSavePath(slotId);

//        if (File.Exists(path))
//        {
//            // Csak megnyitjuk és kiolvassuk az adatokat
//            try
//            {
//                BinaryFormatter formatter = new BinaryFormatter();
//                using (FileStream file = File.Open(path, FileMode.Open))
//                {
//                    SaveGameData data = (SaveGameData)formatter.Deserialize(file);

//                    // Idõ formázása (Óra:Perc)
//                    TimeSpan t = TimeSpan.FromSeconds(data.playTimeSeconds);
//                    string timeStr = string.Format("{0:D2}:{1:D2}", t.Hours, t.Minutes);

//                    // Kiírás: "Slot 1\nLevel 5 | 02:15"
//                    textComp.text = $"Slot {slotId}\nLevel {data.currentLevel} | {timeStr}";
//                }
//            }
//            catch
//            {
//                textComp.text = $"Slot {slotId}\n(Hiba)";
//            }
//        }
//        else
//        {
//            textComp.text = $"Slot {slotId}\nEmpty Slot";
//        }
//    }

//    // --- GOMB ESEMÉNYEK ---

//    public void OnContinueClicked()
//    {
//        if (GameManager.Instance != null)
//        {
//            GameManager.Instance.ContinueGame();
//        }
//    }

//    public void OnNewGameClicked()
//    {
//        OpenSlotSelection();
//    }

//    // Amikor rányomsz az 1., 2. vagy 3. slotra
//    public void OnSlotClicked(int slotId)
//    {
//        if (GameManager.Instance != null)
//        {
//            // Mivel a "New Game" gombbal jöttünk ide, ez mindig Új Játékot indít (Resetel)
//            // és felülírja a slotot, ha volt ott valami.
//            GameManager.Instance.StartNewGame(slotId);
//        }
//    }

//    public void OnQuitClicked()
//    {
//        Application.Quit();
//    }
//}