using UnityEngine;

public class RodAnimationHelper : MonoBehaviour
{
    private FishingMechanic fishingMechanic;
    public Animator rodAnimator;
    void Start()
    {
        // Find the FishingMechanic script on the player
        fishingMechanic = FindObjectOfType<FishingMechanic>();
        rodAnimator = GetComponent<Animator>();
    }

    // This gets called by Animation Event
    public void CallCastLine()
    {
        fishingMechanic.CastLine();
         rodAnimator.SetTrigger("rodCasted");
    }
}