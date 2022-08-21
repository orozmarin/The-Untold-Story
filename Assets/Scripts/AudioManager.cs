using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource[] sfx;
    public AudioSource[] bgm;

    public static AudioManager instance;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(instance);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlayBGM(3);
        }
    }

    public void PlaySFX(int soundToPlay) // play sound effects
    {
        if(soundToPlay < sfx.Length) // ako je unutar broja zvukova
        {
            sfx[soundToPlay].Play();
        }
    }

    public void PlayBGM(int musicToPlay) //play music
    {
        if (!bgm[musicToPlay].isPlaying)
        {
            StopMusic(); // prvo zaustaviti sto god svira

            if (musicToPlay < bgm.Length)
            {
                bgm[musicToPlay].Play();
            }
        }
    }

    public void StopMusic()
    {
        for (int i = 0;i<bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }
}
