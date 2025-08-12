using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ������Ч������
/// </summary>
public class MusicMgr : BaseManager<MusicMgr>
{
    //�������ֲ������
    private AudioSource bkMusic = null;

    //�������ִ�С
    private float bkMusicValue = 0.8f;

    //�������ڲ��ŵ���Ч
    private List<AudioSource> soundList = new List<AudioSource>();
    //��Ч������С
    private float soundValue = 0.2f;
    //��Ч�Ƿ��ڲ���
    private bool soundIsPlay = true;


    private MusicMgr()
    {
        MonoMgr.Instance.AddFixedUpdateListener(Update);
    }


    private void Update()
    {
        if (!soundIsPlay)
            return;

        //��ͣ�ı������� �����û����Ч������� �������� ���Ƴ�������
        //Ϊ�˱���߱������Ƴ������� ���ǲ����������
        for (int i = soundList.Count - 1; i >= 0; --i)
        {
            if (!soundList[i].isPlaying)
            {
                //��Ч��������� ����ʹ���� ���ǽ������Ч��Ƭ�ÿ�
                soundList[i].clip = null;
                PoolMgr.Instance.PushObj(soundList[i].gameObject);
                soundList.RemoveAt(i);
            }
        }
    }


    //���ű�������
    public void PlayBKMusic(string name)
    {
        //��̬�������ű������ֵ���� ���� ����������Ƴ� 
        //��֤���������ڹ�����ʱҲ�ܲ���
        if (bkMusic == null)
        {
            GameObject obj = new GameObject();
            obj.name = "BKMusic";
            GameObject.DontDestroyOnLoad(obj);
            bkMusic = obj.AddComponent<AudioSource>();
        }

        AddressablesMgr.Instance.LoadAssetAsync<AudioClip>(name, (clip) =>
        {
            bkMusic.clip = clip.Result;
            bkMusic.loop = true;
            bkMusic.volume = bkMusicValue;
            bkMusic.Play();
        });
    }

    //ֹͣ��������
    public void StopBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Stop();
    }

    //��ͣ��������
    public void PauseBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Pause();
    }

    //���ñ������ִ�С
    public void ChangeBKMusicValue(float v)
    {
        bkMusicValue = v;
        if (bkMusic == null)
            return;
        bkMusic.volume = bkMusicValue;
    }

    /// <summary>
    /// ������Ч
    /// </summary>
    /// <param name="name">��Ч����</param>
    /// <param name="isLoop">�Ƿ�ѭ��</param>
    /// <param name="isSync">�Ƿ�ͬ������</param>
    /// <param name="callBack">���ؽ�����Ļص�</param>
    public void PlaySound(string name,
                          Transform followTransform,
                          bool isLoop = false,
                          UnityAction<AudioSource> callBack = null)
    {
        AddressablesMgr.Instance.LoadAssetAsync<AudioClip>(name, (clip) =>
        {
            PoolMgr.Instance.GetObjAsync("SoundObj", (soundObj) =>
            {

                AudioSource source = soundObj.GetComponent<AudioSource>();
                //���ȡ��������Ч��֮ǰ����ʹ�õ� ������ֹͣ��
                source.Stop();
                source.clip = clip.Result;
                source.loop = isLoop;
                source.volume = soundValue;
                source.Play();
                source.transform.SetParent(followTransform, false);
                source.transform.localPosition = Vector3.zero;

                if(!soundList.Contains(source))
                    soundList.Add(source);
                callBack?.Invoke(source);
            });
        });

    }

    /// <summary>
    /// ֹͣ������Ч
    /// </summary>
    /// <param name="source">��Ч�������</param>
    public void StopSound(AudioSource source)
    {
        if (soundList.Contains(source))
        {
            //ֹͣ����
            source.Stop();
            //���������Ƴ�
            soundList.Remove(source);
            //������ �����Ƭ ����ռ��
            source.clip = null;
            //���뻺���
            PoolMgr.Instance.PushObj(source.gameObject);
        }
    }

    /// <summary>
    /// �ı���Ч��С
    /// </summary>
    /// <param name="v"></param>
    public void ChangeSoundValue(float v)
    {
        soundValue = v;
        for (int i = 0; i < soundList.Count; i++)
        {
            soundList[i].volume = v;
        }
    }

    /// <summary>
    /// �������Ż�����ͣ������Ч
    /// </summary>
    /// <param name="isPlay">�Ƿ��Ǽ������� trueΪ���� falseΪ��ͣ</param>
    public void PlayOrPauseSound(bool isPlay)
    {
        if (isPlay)
        {
            soundIsPlay = true;
            for (int i = 0; i < soundList.Count; i++)
                soundList[i].Play();
        }
        else
        {
            soundIsPlay = false;
            for (int i = 0; i < soundList.Count; i++)
                soundList[i].Pause();
        }
    }

    /// <summary>
    /// �����Ч��ؼ�¼ ������ʱ����ջ����֮ǰȥ������
    /// ��Ҫ������˵���飡����
    /// ������ʱ����ջ����֮ǰȥ������
    /// ������ʱ����ջ����֮ǰȥ������
    /// ������ʱ����ջ����֮ǰȥ������
    /// </summary>
    public void ClearSound()
    {
        for (int i = 0; i < soundList.Count; i++)
        {
            soundList[i].Stop();
            soundList[i].clip = null;
            PoolMgr.Instance.PushObj(soundList[i].gameObject);
        }
        //�����Ч�б�
        soundList.Clear();
    }
}
