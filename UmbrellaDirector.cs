using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Umbrella
{
    None,
    Normal,
    //Special,
}

public class UmbrellaDirector : MonoBehaviour
{
    public static Umbrella UmbState = Umbrella.None;

    Color None_Color;
    public Slider NM_Slider = null;

    // Start is called before the first frame update
    void Start()
    {
        // 기본 칼라 받아놓기
        None_Color = GetComponent<RawImage>().color;
    }

    // Update is called once per frame
    void Update()
    {
        if (UmbState == Umbrella.None)
        {
            GetComponent<RawImage>().color = None_Color;
            NM_Slider.GetComponent<Slider>().value = 1;
            NM_Slider.gameObject.SetActive(false);
        } // if (UmbState == Umbrella.None)
        else if (UmbState == Umbrella.Normal)
        {
            GetComponent<RawImage>().color = new Color(255 / 255, 255 / 255, 255 / 255);
            NM_Slider.gameObject.SetActive(true);
            if (PlayerController.Cat == CatState.Fall)
            {
                GetComponent<AudioSource>().Play();
                NM_Slider.GetComponent<Slider>().value -= 0.01f;

                if (NM_Slider.GetComponent<Slider>().value <= 0)
                {
                    UmbState = Umbrella.None;
                    FindObjectOfType<PlayerController>().HasUmb = false;
                }
            } // if (PlayerController.Cat == CatState.Fall)
        } // else if (UmbState == Umbrella.Normal)
    }

    public void NewUmbrella()
    {
        NM_Slider.GetComponent<Slider>().value = 1;
    }
}
