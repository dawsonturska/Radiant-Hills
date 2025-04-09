using UnityEngine;
using TMPro;  // Import TextMeshPro namespace

public class MainMenuController : MonoBehaviour
{
    public GameObject logo;             // The logo GameObject
    public GameObject buttonsPanel;     // The panel containing the buttons
    public TextMeshProUGUI clickToStartText; // The "Click to Start" TextMeshPro text
    public float slideSpeed = 2f;       // Speed of the logo sliding
    public float maxDistance = 500f;    // Max distance to slide the logo (set this in the inspector)

    private Vector3 logoStartPosition;
    private Vector3 logoEndPosition;
    private bool isLogoMoving = false;

    void Start()
    {
        // Save the initial positions of the logo
        logoStartPosition = logo.transform.position;

        // Calculate the left-center position based on screen width, but use maxDistance to prevent going off-screen
        float xPosition = Mathf.Max(-Screen.width / 4, -maxDistance);
        logoEndPosition = new Vector3(xPosition, logoStartPosition.y, logoStartPosition.z);

        // Initially hide buttons and the "Click to Start" text
        buttonsPanel.SetActive(false);
        clickToStartText.gameObject.SetActive(true); // Ensure it's visible when the game starts
    }

    void Update()
    {
        // Detect mouse click to trigger logo slide and hide "Click to Start" text
        if (Input.GetMouseButtonDown(0) && !isLogoMoving)
        {
            isLogoMoving = true;
            clickToStartText.gameObject.SetActive(false);  // Hide "Click to Start" text
        }

        // Move logo to left-center when the click occurs
        if (isLogoMoving)
        {
            logo.transform.position = Vector3.MoveTowards(logo.transform.position, logoEndPosition, slideSpeed * Time.deltaTime);

            // Once logo reaches the left-center, reveal the buttons
            if (logo.transform.position == logoEndPosition)
            {
                buttonsPanel.SetActive(true);
                isLogoMoving = false;
            }
        }
    }
}
