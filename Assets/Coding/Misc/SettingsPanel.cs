using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UniversalMachine;


public class SettingsPanel : MonoBehaviour
{
    // UI Elements
    public VisualElement root;
    public VisualElement settingsPanel;

    public Contactor Contactor;


    // Entry Display
    public Label entryNameLabel;


    // Entry Navigation
    public UnityEngine.UIElements.Button previousEntryButton;
    public UnityEngine.UIElements.Button nextEntryButton;


    // Entry Input and Settings
    public TextField entryInput;
    public UnityEngine.UIElements.Button setEntryButton;


    // Settings Entries
    private string[] entryNames = {
        "Disc Rotation Speed",
        "Disc Text",
        "Disc Torque"
        // Add more entries here as needed
    };


    private int currentEntryIndex = 0;


    // Data Storage (Use your existing system for saving and loading)
    public float discRotationSpeed;
    public string discTextValue;
    public float discTorque;


    void Start()
    {
        // Get the root visual element of your UI Document
        root = GetComponent<UIDocument>().rootVisualElement;


        // Create the settings panel
        settingsPanel = new VisualElement();
        settingsPanel.name = "SettingsPanel";


        // Add styling (optional)
        // settingsPanel.style.backgroundColor = Color.gray;
        // settingsPanel.style.padding = 10;


        // Create UI elements
        entryNameLabel = new Label();
        entryNameLabel.name = "EntryNameLabel";


        previousEntryButton = new UnityEngine.UIElements.Button();
        previousEntryButton.name = "PreviousEntryButton";
        previousEntryButton.text = "Previous";
        previousEntryButton.clicked += PreviousEntry;


        nextEntryButton = new UnityEngine.UIElements.Button();
        nextEntryButton.name = "NextEntryButton";
        nextEntryButton.text = "Next";
        nextEntryButton.clicked += NextEntry;


        entryInput = new TextField();
        entryInput.name = "EntryInput";


        setEntryButton = new UnityEngine.UIElements.Button();
        setEntryButton.name = "SetEntryButton";
        setEntryButton.text = "Set";
        setEntryButton.clicked += SetEntry;


        // Load existing settings (adjust based on your saving/loading method)
        LoadSettings();


        // Initialize UI
        UpdateEntry();


        // Add elements to the panel
        settingsPanel.Add(entryNameLabel);
        settingsPanel.Add(previousEntryButton);
        settingsPanel.Add(nextEntryButton);
        settingsPanel.Add(entryInput);
        settingsPanel.Add(setEntryButton);


        // Add the panel to the root
        root.Add(settingsPanel);
        // ... (Your existing code) ...

        // Create a container for the settings elements
        VisualElement contentContainer = new VisualElement();

        // Add UI elements to the container
        contentContainer.Add(entryNameLabel);
        contentContainer.Add(previousEntryButton);
        contentContainer.Add(nextEntryButton);
        contentContainer.Add(entryInput);
        contentContainer.Add(setEntryButton);

        // Move the container down
        contentContainer.style.top = 50; 

        // Add the container to the settings panel
        settingsPanel.Add(contentContainer); 

        // ... (Your existing code) ...
    }


    // Function to navigate to the previous entry
    private void PreviousEntry()
    {
        currentEntryIndex = (currentEntryIndex - 1 + entryNames.Length) % entryNames.Length;
        UpdateEntry();
    }


    // Function to navigate to the next entry
    private void NextEntry()
    {
        currentEntryIndex = (currentEntryIndex + 1) % entryNames.Length;
        UpdateEntry();
    }


    // Function to set the current entry
    private void SetEntry()
    {
        switch (entryNames[currentEntryIndex])
        {
            case "Disc Rotation Speed":
                discRotationSpeed = float.Parse(entryInput.value);
                break;
            case "Disc Text":
                discTextValue = entryInput.value;
                break;
            case "Disc Torque":
                discTorque = float.Parse(entryInput.value);
                break;
            // Add cases for other entries here
        }


        // Update the UI and save the settings (adjust based on your saving/loading method)
        UpdateEntry();
        SaveSettings();

        SetSetting();
    }


    // Function to update the displayed entry
    private void UpdateEntry()
    {
        entryNameLabel.text = entryNames[currentEntryIndex];
        entryInput.value = GetEntryValue(entryNames[currentEntryIndex]);
    }


    // Helper function to retrieve the value of a specific entry
    private string GetEntryValue(string entryName)
    {
        switch (entryName)
        {
            case "Disc Rotation Speed":
                return discRotationSpeed.ToString();
            case "Disc Text":
                return discTextValue;
            case "Disc Torque":
                return discTorque.ToString();
            // Add cases for other entries here
        }
        return "";
    }

    private void SetSetting()
    {
        Contactor.Meaning.Enscription = discTextValue;
        Contactor.Meaning.RotationRate = discRotationSpeed;
        Contactor.Meaning.TorqueStrength = discTorque;

        Contactor.Meaning.CreateCharacterObjects();
    }


    // Function to save the settings (adjust based on your saving/loading method)
    private void SaveSettings()
    {
        
    }


    // Function to load the settings (adjust based on your saving/loading method)
    private void LoadSettings()
    {
        // ... (Implement your loading logic here) ...
    }
}
