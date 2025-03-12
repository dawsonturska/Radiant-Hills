using UnityEngine;

public class PageTurnController : MonoBehaviour
{
    public Material pageMaterial;
    public float turnSpeed = 1.0f;
    private float turnAmount = 0;

    void Update()
    {
        if (Input.GetKey(KeyCode.Space)) // Press Space to turn the page
        {
            turnAmount = Mathf.Clamp01(turnAmount + Time.deltaTime * turnSpeed);
            pageMaterial.SetFloat("_TurnAmount", turnAmount);
        }
    }
}