using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public List<Image> Hearts;
    public Sprite FullHeart;
    public Sprite EmptyHeart;

    public void SetHealth(int toHealth)
    {
        for (int ii = 0; ii < Hearts.Count; ii++)
        {
            if (ii < toHealth)
            {
                Hearts[ii].sprite = FullHeart;
            }
            else
            {
                Hearts[ii].sprite = EmptyHeart;
            }
        }
    }
}
