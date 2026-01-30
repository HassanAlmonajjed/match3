using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillBarController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private List<Image> stars;
    [SerializeField] private Sprite filledSprite;
    [SerializeField] private Sprite unfilledSprite;

    private void Start()
    {
        UpdateProgress(0f);
    }

    public void UpdateProgress(float progress)
    {
        slider.value = progress;
        int filledCount = CalculateFilledStars(progress);

        for (int i = 0; i < stars.Count; i++)
        {
            stars[i].sprite = i < filledCount ? filledSprite : unfilledSprite;
        }
    }

    public int CalculateFilledStars(float progress)
    {
        return CalculateFilledStars(progress, stars.Count);
    }

    public static int CalculateFilledStars(float progress, int starCount)
    {
        int count = 0;
        for (int i = 0; i < starCount; i++)
        {
            float threshold = (float)(i + 1) / starCount * 0.9f;
            if (progress >= threshold)
            {
                count++;
            }
        }
        return count;
    }
}
