using UnityEditor.Rendering;
using UnityEngine;

public class PaperInteraction : MonoBehaviour
{
    private Animator animator;
    private bool isUnfolded = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Raycast hit: " + hit.collider.gameObject.name); // Log the name of the hit object

                if (hit.collider != null && hit.collider.gameObject.name == "Paper0" && !IsAnimationPlaying())
                {
                    Debug.Log("Paper0 clicked");
                    HandlePaperInteraction();
                }
                else if (animator.GetFloat("Speed") == 1 && !IsAnimationPlaying())
                {
                    // Trigger the animation if Speed is 1 and the animation is not playing
                    HandlePaperInteraction();
                }
            }
            else
            {
                Debug.Log("Raycast did not hit anything.");
            }
        }
    }

    private void HandlePaperInteraction()
    {
        if (!isUnfolded)
        {
            animator.SetFloat("Speed", 1);  // Jouer l'animation normalement
            animator.Play("Fold");
            isUnfolded = true;
        }
        else
        {
            animator.SetFloat("Speed", -1); // Jouer l'animation à l'envers
            animator.Play("Unfold");  // Jouer l'animation "Fold" à partir de la fin
            isUnfolded = false;
        }
    }

    private bool IsAnimationPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length >
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}