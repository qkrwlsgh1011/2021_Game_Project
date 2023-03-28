using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FadeState { FadeIn=0,FadeOut,FadeInOut,FadeLoop}

public class FadeEffect : MonoBehaviour
{
    [SerializeField]
    public GameObject sleepimage;

    [SerializeField]
    [Range(0.01f, 10f)]
    public float fadeTime;
    [SerializeField]
    public AnimationCurve fadeCurve;
    public Image image;
    public Text text;
    public FadeState fadeState;
    // Start is called before the first frame update
    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnFade() {
        sleepimage.SetActive(true);
        switch (fadeState) {
            case FadeState.FadeIn:
                StartCoroutine(Fade(1, 0));
                break;
            case FadeState.FadeOut:
                StartCoroutine(Fade(0, 1));
                break;
            case FadeState.FadeInOut:
            case FadeState.FadeLoop:
                StartCoroutine(FadeInOut());
                break;
            }
            //sleepimage.SetActive(false);
    }

    public IEnumerator FadeInOut() {
        while (true) {
            yield return StartCoroutine(Fade(0, 1));
            yield return StartCoroutine(Fade(1, 0));

            if (fadeState == FadeState.FadeInOut) {
                break;
            }
        }
    }

    // Update is called once per frame
   public IEnumerator Fade(float start, float end) {
        float currentTime = 0.0f;
        float percent = 0.0f;

        while (percent < 1) {
            currentTime += Time.deltaTime;
            percent = currentTime / fadeTime;


            Color color = image.color;
            color.a = Mathf.Lerp(start, end, percent);
            image.color = color;

            Color color_t = text.color;
            color_t.a = Mathf.Lerp(start, end, percent);
            text.color = color_t;

            yield return null;
        }
    }
}
