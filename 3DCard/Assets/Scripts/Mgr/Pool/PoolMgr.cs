using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations; // Addressables ����������ռ�

/// <summary>
/// ���루�����е����ݣ�����
/// </summary>
public class PoolData
{
    // �����洢�����еĶ��� ��¼����û��ʹ�õĶ���
    private Stack<GameObject> dataStack = new Stack<GameObject>();

    // ������¼ʹ���еĶ���� 
    private List<GameObject> usedList = new List<GameObject>();

    // �������� ������ͬʱ���ڵĶ�������޸���
    private int maxNum;

    // ��������� �������в��ֹ���Ķ���
    private GameObject rootObj;

    // -- ���ĸĶ� --
    // ���ڻ����Addressables���ص�Prefab
    public GameObject prefab;

    // ��ȡ�������Ƿ��ж���
    public int Count => dataStack.Count;

    public int UsedCount => usedList.Count;

    /// <summary>
    /// ����ʹ���ж�������������������бȽ� С�ڷ���true ��Ҫʵ����
    /// </summary>
    public bool NeedCreate => usedList.Count < maxNum;

    /// <summary>
    /// ��ʼ�����캯��
    /// </summary>
    /// <param name="root">���ӣ�����أ�������</param>
    /// <param name="name">���븸���������</param>
    public PoolData(GameObject root, string name)
    {
        if (PoolMgr.isOpenLayout)
        {
            rootObj = new GameObject(name);
            rootObj.transform.SetParent(root.transform);
        }
    }

    /// <summary>
    /// ��ʼ���������ݣ��ر����������
    /// </summary>
    public void InitPoolData(GameObject initialObj)
    {
        // ��¼��һ����ʵ�����Ķ���
        PushUsedList(initialObj);

        // ͨ����һ�������ȡ��PoolObj�ű������ó�������
        // ע�⣺���Ԥ��������Ҫ����һ������PoolObj�Ľű����������ڳ��е��������
        PoolObj poolObj = initialObj.GetComponent<PoolObj>();
        if (poolObj == null)
        {
            Debug.LogError("��Ϊʹ�û���ع��ܵ�Ԥ����������PoolObj�ű� ����������������: " + initialObj.name);
            maxNum = 10; // ��һ��Ĭ��ֵ�������
            return;
        }
        maxNum = poolObj.maxNum;
    }

    /// <summary>
    /// �ӳ����е������ݶ���
    /// </summary>
    public GameObject Pop()
    {
        GameObject obj;

        if (Count > 0)
        {
            obj = dataStack.Pop();
            usedList.Add(obj);
        }
        else // �������û�����ˣ��͸���һ���Ѿ������
        {
            obj = usedList[0];
            usedList.RemoveAt(0);
            usedList.Add(obj);
        }

        obj.SetActive(true);
        if (PoolMgr.isOpenLayout)
            obj.transform.SetParent(null);

        return obj;
    }

    /// <summary>
    /// ��������뵽���������
    /// </summary>
    public void Push(GameObject obj)
    {
        obj.SetActive(false);
        if (PoolMgr.isOpenLayout)
            obj.transform.SetParent(rootObj.transform);
        dataStack.Push(obj);
        usedList.Remove(obj);
    }

    /// <summary>
    /// ������ѹ�뵽ʹ���е������м�¼
    /// </summary>
    public void PushUsedList(GameObject obj)
    {
        usedList.Add(obj);
    }
}

/*
 * ע��: Ϊ��������� InitPoolData ������������,
 * ����Ҫ�������Ϸ����Ԥ���� (Prefab) �Ϲ���һ���������������Ľű���
 * * public class PoolObj : MonoBehaviour
 * {
 * public int maxNum = 10; // ����˶����ڳ��е��������
 * }
 * */


/// <summary>
/// �������ֵ䵱������ʽ�滻ԭ�� �洢�������
/// </summary>
public abstract class PoolObjectBase { }

/// <summary>
/// ���ڴ洢 ���ݽṹ�� �� �߼��� �����̳�mono�ģ�������
/// </summary>
public class PoolObject<T> : PoolObjectBase where T : class
{
    public Queue<T> poolObjs = new Queue<T>();
}

/// <summary>
/// ��Ҫ�����õ� ���ݽṹ�ࡢ�߼��� ������Ҫ�̳иýӿ�
/// </summary>
public interface IPoolObject
{
    /// <summary>
    /// �������ݵķ���
    /// </summary>
    void ResetInfo();
}


/// <summary>
/// �����(�����)ģ�� ������ (�Ѽ���Addressables)
/// </summary>
public class PoolMgr : BaseManager<PoolMgr>
{
    // GameObject �����
    private Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
    // C# ������
    private Dictionary<string, PoolObjectBase> poolObjectDic = new Dictionary<string, PoolObjectBase>();
    // ���Ӹ�����
    private GameObject poolObj;
    // �Ƿ������ֹ���
    public static bool isOpenLayout = true;

    private PoolMgr()
    {
        if (poolObj == null && isOpenLayout)
            poolObj = new GameObject("Pool");
    }

    #region --- ��������ͬ����ȡ GameObject ���� ---

    /// <summary>
    /// ��ͬ�����Ӷ���ػ�ȡһ��GameObject��
    /// �����桿�˷������������߳�ֱ����Դ������ɣ����ܵ��¿��٣������ʹ�ã�
    /// </summary>
    /// <param name="name">����Ԥ���壩��Addressable Key</param>
    /// <returns>��ȡ����GameObjectʵ�����������ʧ���򷵻�null</returns>
    public GameObject GetObjSync(string name)
    {
        // ȷ�����������
        if (poolObj == null && isOpenLayout)
            poolObj = new GameObject("Pool");

        // ���1 & 2�����ӿ��ã�ֱ�Ӵ��л�ȡ
        if (poolDic.ContainsKey(name) && (poolDic[name].Count > 0 || !poolDic[name].NeedCreate))
        {
            return poolDic[name].Pop();
        }

        // ���3����Ҫ���ػ�ʵ����һ���¶���
        // ���Prefab�Ƿ��Ѽ���
        if (poolDic.ContainsKey(name) && poolDic[name].prefab != null)
        {
            // Prefab�Ѿ����ع��ˣ�ֱ��������ʵ����
            GameObject obj = GameObject.Instantiate(poolDic[name].prefab);
            obj.name = name;
            poolDic[name].PushUsedList(obj);
            return obj;
        }
        else
        {
            // Prefabû���ع�����Ҫͨ��Addressablesͬ������
            // �����������̵߳Ĳ�����
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(name);
            GameObject prefab = handle.WaitForCompletion(); // �ȴ��������

            if (handle.Status == AsyncOperationStatus.Succeeded && prefab != null)
            {
                // ʵ�����¶���
                GameObject obj = GameObject.Instantiate(prefab);
                obj.name = name;

                // ��������ǵ�һ�δ���
                if (!poolDic.ContainsKey(name))
                {
                    PoolData newPoolData = new PoolData(poolObj, name);
                    newPoolData.prefab = prefab;
                    newPoolData.InitPoolData(obj);
                    poolDic.Add(name, newPoolData);
                }
                else // �����Ѵ���
                {
                    poolDic[name].prefab = prefab;
                    poolDic[name].PushUsedList(obj);
                }

                // ע�⣺ͬ������ʱ����Դ�����Ҫ�ֶ������ͷš�
                // AddressablesMgrͨ���ᴦ��������⣬�����������Ƕ������صġ�
                // һ���򵥵Ĳ�������Addressables�����ü���ϵͳ�Զ�����
                // ����ClearPoolʱ����Release�ͷš�

                return obj;
            }
            else
            {
                Debug.LogError($"[PoolMgr-Sync] Failed to load asset from Addressables: {name}");
                Addressables.Release(handle); // ����ʧ�ܣ��ͷž��
                return null;
            }
        }
    }

    #endregion
    #region --- GameObject ����� (�Ѽ��� Addressables) ---

    /// <summary>
    /// �첽�شӶ���ػ�ȡһ��GameObject
    /// </summary>
    /// <param name="name">����Ԥ���壩��Addressable Key</param>
    /// <param name="callback">��ȡ�������ִ�еĻص�</param>
    public void GetObjAsync(string name, Action<GameObject> callback)
    {
        if (poolObj == null && isOpenLayout)
            poolObj = new GameObject("Pool");

        // ���1�������Ѵ��ڣ����ҳ�����δʹ�õĶ���
        if (poolDic.ContainsKey(name) && poolDic[name].Count > 0)
        {
            GameObject obj = poolDic[name].Pop();
            callback?.Invoke(obj);
            return;
        }

        // ���2�������Ѵ��ڣ��������޶������Ѵﵽ������������� (�����Ѽ������)
        if (poolDic.ContainsKey(name) && !poolDic[name].NeedCreate)
        {
            GameObject obj = poolDic[name].Pop();
            callback?.Invoke(obj);
            return;
        }

        // ���3����Ҫ���ز�ʵ����һ���¶���
        // ���Prefab�Ƿ��Ѽ���
        if (poolDic.ContainsKey(name) && poolDic[name].prefab != null)
        {
            // Prefab�Ѿ����ع��ˣ�ֱ��������ʵ����
            GameObject obj = GameObject.Instantiate(poolDic[name].prefab);
            obj.name = name; // �Ƴ�(Clone)��׺
            poolDic[name].PushUsedList(obj);
            callback?.Invoke(obj);
        }
        else
        {
            // Prefabû���ع�����Ҫͨ��Addressables�첽����
            AddressablesMgr.Instance.LoadAssetAsync<GameObject>(name, (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject prefab = handle.Result;
                    GameObject obj = GameObject.Instantiate(prefab);
                    obj.name = name;

                    // ��������ǵ�һ�δ���
                    if (!poolDic.ContainsKey(name))
                    {
                        PoolData newPoolData = new PoolData(poolObj, name);
                        newPoolData.prefab = prefab; // ������ص���Prefab
                        newPoolData.InitPoolData(obj); // ��ʼ��������Ϣ�����������ޣ�
                        poolDic.Add(name, newPoolData);
                    }
                    else // �����Ѵ��ڣ�ֻ���ڲ������
                    {
                        poolDic[name].prefab = prefab; // Ҳ����һ��
                        poolDic[name].PushUsedList(obj);
                    }

                    callback?.Invoke(obj);
                }
                else
                {
                    Debug.LogError($"[PoolMgr] Failed to load asset from Addressables: {name}");
                    callback?.Invoke(null);
                }
            });
        }
    }




    /// <summary>
    /// ��GameObject�Żس���
    /// </summary>
    public void PushObj(GameObject obj)
    {
        if (obj == null) return;

        if (poolDic.ContainsKey(obj.name))
        {
            poolDic[obj.name].Push(obj);
        }
        else
        {
            Debug.LogWarning($"[PoolMgr] Trying to push an object '{obj.name}' to a non-existent pool. Destroying it instead.");
            GameObject.Destroy(obj);
        }
    }

    #endregion

    #region --- C# ������ (��MonoBehaviour) ---

    /// <summary>
    /// ��ȡ�Զ�������ݽṹ����߼������
    /// </summary>
    public T GetObj<T>(string nameSpace = "") where T : class, IPoolObject, new()
    {
        string poolName = nameSpace + "_" + typeof(T).Name;
        if (poolObjectDic.ContainsKey(poolName))
        {
            PoolObject<T> pool = poolObjectDic[poolName] as PoolObject<T>;
            if (pool.poolObjs.Count > 0)
            {
                return pool.poolObjs.Dequeue() as T;
            }
        }
        return new T();
    }

    /// <summary>
    /// ���Զ������ݽṹ����߼��� ���������
    /// </summary>
    public void PushObj<T>(T obj, string nameSpace = "") where T : class, IPoolObject
    {
        if (obj == null) return;
        string poolName = nameSpace + "_" + typeof(T).Name;

        if (!poolObjectDic.TryGetValue(poolName, out PoolObjectBase poolBase))
        {
            poolBase = new PoolObject<T>();
            poolObjectDic.Add(poolName, poolBase);
        }

        PoolObject<T> pool = poolBase as PoolObject<T>;
        obj.ResetInfo();
        pool.poolObjs.Enqueue(obj);
    }

    #endregion

    #region --- ������ͷ� ---

    /// <summary>
    /// ����ָ���ĵ���GameObject����أ����ͷ���Addressable��Դ
    /// </summary>
    /// <param name="name">Ҫ����ĳ��ӵ����� (Addressable Key)</param>
    public void ClearPool(string name)
    {
        if (poolDic.TryGetValue(name, out PoolData poolData))
        {
            // �ͷŶ�Ӧ��Addressable��Դ
            // ע�⣺�������AddressablesMgr����ȷ�����ü�����������֪������Ψһʹ�ø���Դ�ĵط�
            if (poolData.prefab != null)
            {
                AddressablesMgr.Instance.Release<GameObject>(name);
            }
            poolDic.Remove(name);
        }
    }

    /// <summary>
    /// �������ж���� (ͨ�����л�����ʱ����)
    /// </summary>
    public void ClearAllPools()
    {
        // ��������GameObject���ӣ��ͷ����ǻ����Addressable��Դ
        foreach (var pair in poolDic)
        {
            if (pair.Value.prefab != null)
            {
                AddressablesMgr.Instance.Release<GameObject>(pair.Key);
            }
        }

        poolDic.Clear();
        poolObjectDic.Clear();

        // ���ٳ����еĳ��Ӹ�����
        if (poolObj != null)
        {
            GameObject.Destroy(poolObj);
            poolObj = null;
        }
    }

    #endregion
}