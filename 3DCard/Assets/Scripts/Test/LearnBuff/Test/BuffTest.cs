#region

//文件创建者：Egg
//创建时间：04-03 01:45

#endregion

using System;
using UnityEngine;

namespace LearnBuff
{
    public sealed class BuffTest : MonoBehaviour
    {
        private BuffManager _buffManager;

        private void Awake()
        {
            _buffManager = GetComponent<BuffManager>();
        }

        private bool _inPoison;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                _inPoison = !_inPoison;
                if (_inPoison)
                {
                    _buffManager.AddBuff(BuffConstant.BuffName.CAST_POISON, gameObject);
                    Debug.Log("施毒Buff已施加");
                }
                else
                {
                    _buffManager.RemoveBuff(BuffConstant.BuffName.CAST_POISON);
                    Debug.Log("施毒Buff已移除");
                }
            }
        }
    }
}