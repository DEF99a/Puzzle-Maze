using Assets.Scripts.Define;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;

    [Range(.1f, 3f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}

public enum SoundName
{
    None,

    block_bounce,
    block_collision,
    bomb_bip,
    bomb_explode,
    bomberman_explode,
    click_btn_common,
    click_fail,
    clock_count_ingame,
    clock_count_lose,
    coin_counter,
    collect_coin,
    collect_diamond,
    combat_music,
    count_star,
    disappear_in_gate,
    enemy_die,
    gift_break,
    gift_burn,
    ground_music,
    jelly_bounce,
    jelly_break,
    level_complete,
    level_fail,
    melon_break,
    mine_break,
    open_gate,
    pick_angry,
    pick_ballon,
    pick_coin,
    pick_invisible,
    pick_speed,
    player_appear,
    player_die,
    player_kick,
    player_select,
    player_unlock,
    player_walk_common,
    puzzle_music,
    quest_complete,
    quest_noti,
    ready,
    skill_bomberman,
    skill_bowser,
    skill_dino,
    skill_foo,
    skill_goku_end,
    skill_goku_loop,
    skill_haunter,
    skill_impostor,
    skill_koffling,
    skill_mario,
    skill_weipinbell,
    soil_break_1,
    soil_break_2,
    soil_break_3,
    soil_break_4,
    soil_break_5,
    soil_break_6,
    soil_break_7,
    soil_break_8,
    stone_break,
    text_type_tut,
    tree_break_1,
    tree_break_2,
    upgrade_life,
    upgrade_speed,
    freeze_break,
    freeze_loop,
    kill_combo2,
    kill_combo3,
    pick_clock,
    pick_freeze,
    pick_life,
    skill_mario_hit,
    tree_respawn,
    key_unlock
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    public Sound[] sounds;

    private IEnumerator randomIE;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        foreach (var s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            if (string.IsNullOrEmpty(s.name))
                s.name = s.clip.name;
            //Debug.Log("s.name:" + s.name);
        }
    }

    private void Play(string name, bool restart = false)
    {
        var sound = Array.Find(sounds, s => s.name == name);
        if (!sound.source.isPlaying)
            sound?.source.Play();
        else if (restart)
            sound?.source.Play();
    }

    private void Stop(string name)
    {
        var sound = Array.Find(sounds, s => s.name == name);
        sound?.source.Stop();
    }

    public void StopAll()
    {
        foreach (var s in sounds)
        {
            s.source.Stop();
        }
    }

    public void PlayMusic(SoundName soundName)
    {
        if (ES3.Load(StringDefine.Music, 1) == 1)
            Play(soundName.ToString());
    }

    public void StopMusic(SoundName soundName)
    {
        Stop(soundName.ToString());
    }

    public void PlaySound(SoundName soundType, float delay = 0)
    {
        if (ES3.Load(StringDefine.Sound, 1) == 1)
        {
            if (delay == 0)
                Play(soundType.ToString());
            else
            {
                StartCoroutine(PlaySoudDelay());
            }
        }
        IEnumerator PlaySoudDelay()
        {
            yield return new WaitForSeconds(delay);
            Play(soundType.ToString());
        }
    }

    public void StopSound(SoundName soundType)
    {
        if (ES3.Load(StringDefine.Sound, 1) == 1)
            Play(soundType.ToString());
    }
}
