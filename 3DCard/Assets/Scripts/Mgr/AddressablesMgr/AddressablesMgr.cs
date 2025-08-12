using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablesMgr : BaseManager<AddressablesMgr>
{
    private AddressablesMgr() {  }

    //��һ������ �������Ǵ洢 �첽���صķ���ֵ
    public Dictionary<string, IEnumerator> resDic = new Dictionary<string, IEnumerator>();


    //�첽������Դ�ķ���
    public void LoadAssetAsync<T>(string name, Action<AsyncOperationHandle<T>> callBack)
    {
        //���ڴ���ͬ�� ��ͬ������Դ�����ּ���
        //��������ͨ�����ֺ�����ƴ����Ϊ key
        string keyName = name + "_" + typeof(T).Name;
        AsyncOperationHandle<T> handle;
        //����Ѿ����ع�����Դ
        if (resDic.ContainsKey(keyName))
        {
            //��ȡ�첽���ط��صĲ�������
            handle = (AsyncOperationHandle<T>)resDic[keyName];

            //�ж� ����첽�����Ƿ����
            if (handle.IsDone)
            {
                //����ɹ� �Ͳ���Ҫ�첽�� ֱ���൱��ͬ�������� ���ί�к��� �����Ӧ�ķ���ֵ
                callBack(handle);
            }
            //��û�м������
            else
            {
                //������ʱ�� ��û���첽������� ��ô����ֻ��Ҫ ������ ���ʱ��ʲô������
                handle.Completed += (obj) => {
                    if (obj.Status == AsyncOperationStatus.Succeeded)
                        callBack(obj);
                };
            }
            return;
        }

        //���û�м��ع�����Դ
        //ֱ�ӽ����첽���� ���Ҽ�¼
        handle = Addressables.LoadAssetAsync<T>(name);
        handle.Completed += (obj) => {
            if (obj.Status == AsyncOperationStatus.Succeeded)
                callBack(obj);
            else
            {
                Debug.LogWarning(keyName + "��Դ����ʧ��");
                if (resDic.ContainsKey(keyName))
                    resDic.Remove(keyName);
            }
        };
        resDic.Add(keyName, handle);
    }

    //�ͷ���Դ�ķ��� 
    public void Release<T>(string name)
    {
        //���ڴ���ͬ�� ��ͬ������Դ�����ּ���
        //��������ͨ�����ֺ�����ƴ����Ϊ key
        string keyName = name + "_" + typeof(T).Name;
        if (resDic.ContainsKey(keyName))
        {
            //ȡ������ �Ƴ���Դ ���Ҵ��ֵ������Ƴ�
            AsyncOperationHandle<T> handle = (AsyncOperationHandle<T>)resDic[keyName];
            Addressables.Release(handle);
            resDic.Remove(keyName);
        }
    }

    //�첽���ض����Դ ���� ����ָ����Դ
    public void LoadAssetAsync<T>(Addressables.MergeMode mode, Action<T> callBack, params string[] keys)
    {
        //1.����һ��keyName  ֮�����ڴ��뵽�ֵ���
        List<string> list = new List<string>(keys);
        string keyName = "";
        foreach (string key in list)
            keyName += key + "_";
        keyName += typeof(T).Name;
        //2.�ж��Ƿ�����Ѿ����ع������� 
        //������ʲô
        AsyncOperationHandle<IList<T>> handle;
        if (resDic.ContainsKey(keyName))
        {
            handle = (AsyncOperationHandle<IList<T>>)resDic[keyName];
            //�첽�����Ƿ����
            if (handle.IsDone)
            {
                foreach (T item in handle.Result)
                    callBack(item);
            }
            else
            {
                handle.Completed += (obj) =>
                {
                    //���سɹ��ŵ����ⲿ�����ί�к���
                    if (obj.Status == AsyncOperationStatus.Succeeded)
                    {
                        foreach (T item in handle.Result)
                            callBack(item);
                    }
                };
            }
            return;
        }
        //��������ʲô
        handle = Addressables.LoadAssetsAsync<T>(list, callBack, mode);
        handle.Completed += (obj) =>
        {
            if (obj.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError("��Դ����ʧ��" + keyName);
                if (resDic.ContainsKey(keyName))
                    resDic.Remove(keyName);
            }
        };
        resDic.Add(keyName, handle);
    }

    public void LoadAssetAsync<T>(Addressables.MergeMode mode, Action<AsyncOperationHandle<IList<T>>> callBack, params string[] keys)
    {

    }

    public void Releas<T>(params string[] keys)
    {
        //1.����һ��keyName  ֮�����ڴ��뵽�ֵ���
        List<string> list = new List<string>(keys);
        string keyName = "";
        foreach (string key in list)
            keyName += key + "_";
        keyName += typeof(T).Name;

        if (resDic.ContainsKey(keyName))
        {
            //ȡ���ֵ�����Ķ���
            AsyncOperationHandle<IList<T>> handle = (AsyncOperationHandle<IList<T>>)resDic[keyName];
            Addressables.Release(handle);
            resDic.Remove(keyName);
        }
    }


    //�����Դ
    public void Clear()
    {
        resDic.Clear();
        AssetBundle.UnloadAllAssetBundles(true);
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }
}
