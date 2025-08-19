using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// �㼶ö��
/// </summary>
public enum E_UILayer
{
    /// <summary>
    /// ��ײ�
    /// </summary>
    Bottom,
    /// <summary>
    /// �в�
    /// </summary>
    Middle,
    /// <summary>
    /// �߲�
    /// </summary>
    Top,
    /// <summary>
    /// ϵͳ�� ��߲�
    /// </summary>
    System,
}

/// <summary>
/// ��������UI���Ĺ�����
/// ע�⣺���Ԥ������Ҫ���������һ�£���������
/// </summary>
public class UIMgr : BaseManager<UIMgr>
{
    private abstract class BasePanelInfo { }

    private class PanelInfo<T> : BasePanelInfo where T : BasePanel
    {
        public T panel;
        public UnityAction<T> callBack;
        public bool isHide;

        public PanelInfo(UnityAction<T> callBack)
        {
            this.callBack += callBack;
        }
    }

    private Camera uiCamera;
    private Canvas uiCanvas;
    private EventSystem uiEventSystem;

    private Transform bottomLayer;
    private Transform middleLayer;
    private Transform topLayer;
    private Transform systemLayer;

    private Dictionary<string, BasePanelInfo> panelDic = new Dictionary<string, BasePanelInfo>();

    private UIMgr()
    {

        //GameObject cameraPrefab = AddressablesMgr.Instance.LoadAsset<GameObject>("UICamera");
        //if (cameraPrefab != null)
        //{
        //    uiCamera = GameObject.Instantiate(cameraPrefab).GetComponent<Camera>();
        //    GameObject.DontDestroyOnLoad(uiCamera.gameObject);
        //}

        GameObject canvasPrefab = AddressablesMgr.Instance.LoadAsset<GameObject>("Canvas");
        if (canvasPrefab != null)
        {
            uiCanvas = GameObject.Instantiate(canvasPrefab).GetComponent<Canvas>();
            uiCanvas.worldCamera = uiCamera;
            GameObject.DontDestroyOnLoad(uiCanvas.gameObject);

            bottomLayer = uiCanvas.transform.Find("Bottom");
            middleLayer = uiCanvas.transform.Find("Middle");
            topLayer = uiCanvas.transform.Find("Top");
            systemLayer = uiCanvas.transform.Find("System");
        }

        //GameObject eventPrefab = AddressablesMgr.Instance.LoadAsset<GameObject>("EventSystem");
        //if (eventPrefab != null)
        //{
        //    uiEventSystem = GameObject.Instantiate(eventPrefab).GetComponent<EventSystem>();
        //    GameObject.DontDestroyOnLoad(uiEventSystem.gameObject);
        //}
    }

    public Transform GetLayerFather(E_UILayer layer)
    {
        switch (layer)
        {
            case E_UILayer.Bottom: return bottomLayer;
            case E_UILayer.Middle: return middleLayer;
            case E_UILayer.Top: return topLayer;
            case E_UILayer.System: return systemLayer;
            default: return middleLayer; // �ṩһ��Ĭ��ֵ
        }
    }

    /// <summary>
    /// ��ʾ��� (Addressables �첽����)
    /// </summary>
    public void ShowPanel<T>(E_UILayer layer = E_UILayer.Middle, UnityAction<T> callBack = null) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            if (panelInfo.panel == null)
            {
                panelInfo.isHide = false;
                if (callBack != null)
                    panelInfo.callBack += callBack;
            }
            else
            {
                if (!panelInfo.panel.gameObject.activeSelf)
                    panelInfo.panel.gameObject.SetActive(true);

                panelInfo.panel.ShowMe();
                callBack?.Invoke(panelInfo.panel);
            }
            return;
        }

        panelDic.Add(panelName, new PanelInfo<T>(callBack));
        AddressablesMgr.Instance.LoadAssetAsync<GameObject>(panelName, (handle) =>
        {
            // ���������ʱ��������Ƿ��Ѿ������Ϊ����
            if (!panelDic.ContainsKey(panelName)) return; // �����ڼ��ع����б�������
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            if (panelInfo.isHide)
            {
                // ����ڼ��ع����б����Ϊ���أ�ֱ���ͷ���Դ���Ƴ���¼
                panelDic.Remove(panelName);
                AddressablesMgr.Instance.Release<GameObject>(panelName);
                return;
            }

            Transform father = GetLayerFather(layer);

            // ʹ�� handle.Result ��ȡ���ص��� GameObject ��Դ
            GameObject panelObj = GameObject.Instantiate(handle.Result, father, false);

            T panel = panelObj.GetComponent<T>();
            panel.ShowMe();

            panelInfo.callBack?.Invoke(panel);
            panelInfo.callBack = null;
            panelInfo.panel = panel;
        });
    }

    /// <summary>
    /// �������
    /// </summary>
    public void HidePanel<T>(bool isDestroy = false) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            if (panelInfo.panel == null)
            {
                panelInfo.isHide = true;
                panelInfo.callBack = null;
            }
            else
            {
                panelInfo.panel.HideMe();
                if (isDestroy)
                {
                    GameObject.Destroy(panelInfo.panel.gameObject);
                    panelDic.Remove(panelName);
                    AddressablesMgr.Instance.Release<GameObject>(panelName);
                }
                else
                {
                    panelInfo.panel.gameObject.SetActive(false);
                }
            }
        }
    }




    /// <summary>
    /// ��ȡ���
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    public void GetPanel<T>( UnityAction<T> callBack ) where T:BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            //ȡ���ֵ����Ѿ�ռ��λ�õ�����
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            //���ڼ�����
            if(panelInfo.panel == null)
            {
                //������ Ӧ�õȴ����ؽ��� ��ͨ���ص����ݸ��ⲿȥʹ��
                panelInfo.callBack += callBack;
            }
            else if(!panelInfo.isHide)//���ؽ��� ����û������
            {
                callBack?.Invoke(panelInfo.panel);
            }
        }
    }


    /// <summary>
    /// Ϊ�ؼ�����Զ����¼�
    /// </summary>
    /// <param name="control">��Ӧ�Ŀؼ�</param>
    /// <param name="type">�¼�������</param>
    /// <param name="callBack">��Ӧ�ĺ���</param>
    public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> callBack)
    {
        //�����߼���Ҫ�����ڱ�֤ �ؼ���ֻ�����һ��EventTrigger
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = control.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callBack);

        trigger.triggers.Add(entry);
    }
}
