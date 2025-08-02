using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VCMgr : SingletonMono<VCMgr>
{
    public CinemachineVirtualCamera currentVC;
    public CinemachineVirtualCamera defaultFirstPersonVC;
    public List<CinemachineVirtualCamera> allVCs = new List<CinemachineVirtualCamera>();

    private void Start()
    {
        currentVC = defaultFirstPersonVC;
    }
    public void SetCurrentVC(CinemachineVirtualCamera VC)
    {
        VC.Priority = 9999;
        currentVC.Priority = 10;
        currentVC = VC;
    }
    public void SetDefaultVC()
    {
        SetCurrentVC(defaultFirstPersonVC);
    }
}
