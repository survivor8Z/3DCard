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

    //��Э�����첽������Դ�ķ���
    public void LoadAssetCoroutine<T>(string name, Action<AsyncOperationHandle<T>> callback)
    {
        MonoMgr.Instance.StartGlobalCoroutine(LoadAssetAsyncInternal(name, callback));
    }

    private IEnumerator LoadAssetAsyncInternal<T>(string name, Action<AsyncOperationHandle<T>> callback)
    {
        string keyName = name + "_" + typeof(T).Name;
        AsyncOperationHandle<T> handle;

        if (resDic.ContainsKey(keyName))
        {
            handle = (AsyncOperationHandle<T>)resDic[keyName];
            yield return handle;
            callback(handle);
        }
        else
        {
            handle = Addressables.LoadAssetAsync<T>(name);
            resDic.Add(keyName, handle);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
                callback(handle);
            else
            {
                Debug.LogWarning(keyName + " ��Դ����ʧ��");
                if (resDic.ContainsKey(keyName))
                    resDic.Remove(keyName);
            }
        }
    }

    /// <summary>
    /// ÿ��load��һ����Դ�ͻص�һ��
    /// ȷ��name��Ψһ��
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mode"></param>
    /// <param name="callBack"></param>
    /// <param name="keys"></param>
    public void LoadAssetsCoroutinePer<T>( Action<T> callBack, params string[] keys)
    {
        MonoMgr.Instance.StartGlobalCoroutine(LoadAssetsAsyncInternalPer( callBack, keys));
    }
    private IEnumerator LoadAssetsAsyncInternalPer<T>(Action<T> callBack, params string[] keys)
    {
        if (keys == null || keys.Length == 0)
        {
            Debug.LogWarning("δ�ṩ�κ���Դ����");
            yield break;
        }

        // ����ÿһ����Դ������˳�����
        foreach (string key in keys)
        {
            string keyName = key + "_" + typeof(T).Name;
            AsyncOperationHandle<T> handle;

            // ��黺��
            if (resDic.ContainsKey(keyName))
            {
                handle = (AsyncOperationHandle<T>)resDic[keyName];
            }
            else
            {
                // �����µļ�������
                handle = Addressables.LoadAssetAsync<T>(key);
                resDic.Add(keyName, handle);
            }

            // �ȴ�������Դ�������
            yield return handle;

            // ������ɺ󣬼��״̬�����ûص�
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                callBack(handle.Result);
            }
            else
            {
                Debug.LogError($"��Դ {keyName} ˳�����ʧ�ܣ�");
                if (resDic.ContainsKey(keyName))
                    resDic.Remove(keyName);

                // ����ʧ��ʱֹͣ��������
                yield break;
            }
        }
    }

    /// <summary>
    /// ������Դ������Ϻ�ص�һ��
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mode"></param>
    /// <param name="callBack"></param>
    /// <param name="keys"></param>
    public void LoadAssetsCoroutineUni<T>(Addressables.MergeMode mode, Action<IList<T>> callBack, params string[] keys)
    {
        MonoMgr.Instance.StartGlobalCoroutine(LoadAssetsAsyncInternalUni(mode, callBack, keys));
    }
    private IEnumerator LoadAssetsAsyncInternalUni<T>(Addressables.MergeMode mode, Action<IList<T>> callBack, params string[] keys)
    {
        List<string> list = new List<string>(keys);
        string keyName = "";
        foreach (string key in list)
            keyName += key + "_";
        keyName += typeof(T).Name;

        // ��ȡ�첽�������
        AsyncOperationHandle<IList<T>> handle;

        // �����Դ�Ƿ����ڻ�����
        if (resDic.ContainsKey(keyName))
        {
            // ����ѻ��棬ֱ�Ӵ��ֵ���ȡ�����
            handle = (AsyncOperationHandle<IList<T>>)resDic[keyName];
        }
        else
        {
            // ���δ���棬�����µ��첽��������ע�����ﴫ�� null
            // ��Ϊ��������Э�����ֶ�����ص�
            handle = Addressables.LoadAssetsAsync<T>(list, null, mode);
            // ��������뻺���ֵ�
            resDic.Add(keyName, handle);
        }

        // �ȴ��������
        yield return handle;

        // ������ɺ󣬼��״̬
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            // ֻ�е��������������ɹ�ʱ���ŵ��ûص�
            // ����������Դ�б�
            callBack(handle.Result);
            Debug.Log($"��Դ�� {keyName} ���سɹ���");
        }
        else
        {
            // �������ʧ��
            Debug.LogError($"��Դ�� {keyName} ����ʧ�ܣ�");
            // ʧ�ܺ���ֵ����Ƴ��������´μ���ʱֱ�Ӵ�ʧ�ܵľ���л�ȡ
            if (resDic.ContainsKey(keyName))
            {
                resDic.Remove(keyName);
            }
        }
    }





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

    

    //�첽���ض����Դ ���� ����ָ����Դ
    public void LoadAssetsAsync<T>(Addressables.MergeMode mode, Action<T> callBack, params string[] keys)
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



    








    #region ͬ��������Դ
    /// <summary>
    /// ͬ�����ص�����Դ
    /// </summary>
    public T LoadAsset<T>(string name)
    {
        string keyName = name + "_" + typeof(T).Name;
        AsyncOperationHandle<T> handle;

        // �����Դ�Ѿ����첽�����л�����ɣ�ֱ�ӵȴ����
        if (resDic.ContainsKey(keyName))
        {
            handle = (AsyncOperationHandle<T>)resDic[keyName];
            handle.WaitForCompletion();
            return handle.Result;
        }
        else
        {
            // �����Դ��δ���أ������ͬ������
            handle = Addressables.LoadAssetAsync<T>(name);
            handle.WaitForCompletion();

            // �ɹ����غ�����ֵ�
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                resDic.Add(keyName, handle);
                return handle.Result;
            }
            else
            {
                Debug.LogError($"ͬ��������Դʧ��: {name}");
                return default;
            }
        }
    }

    /// <summary>
    /// ͬ�����ض����Դ
    /// </summary>
    public IList<T> LoadAssets<T>(Addressables.MergeMode mode, params string[] keys)
    {
        List<string> list = new List<string>(keys);
        string keyName = "";
        foreach (string key in list)
            keyName += key + "_";
        keyName += typeof(T).Name;

        AsyncOperationHandle<IList<T>> handle;

        // �����Դ�Ѿ����첽�����л�����ɣ�ֱ�ӵȴ����
        if (resDic.ContainsKey(keyName))
        {
            handle = (AsyncOperationHandle<IList<T>>)resDic[keyName];
            handle.WaitForCompletion();
            return handle.Result;
        }
        else
        {
            // �����Դ��δ���أ������ͬ������
            handle = Addressables.LoadAssetsAsync<T>(list, null, mode);
            handle.WaitForCompletion();

            // �ɹ����غ�����ֵ�
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                resDic.Add(keyName, handle);
                return handle.Result;
            }
            else
            {
                Debug.LogError($"ͬ�����ض����Դʧ��: {keyName}");
                return null;
            }
        }
    }
    #endregion
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
    // �ͷ���Դ�ķ��������ڶ����Դ
    public void Release<T>(params string[] keys)
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
