#region

//文件创建者：Egg
//创建时间：04-03 11:19

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LearnBuff
{
    public sealed class BuffManager : MonoBehaviour
    {
        private readonly List<BuffRunTimeInfo> _buffRunTimeInfos = new();
        //缓冲区，解决Buff的OnTick生效创建新Buff导致迭代器失效的问题
        private readonly Queue<BuffRunTimeInfo> _buff2Add = new();

        public IReadOnlyList<BuffRunTimeInfo> GetBuffs() => _buffRunTimeInfos.ToList();

        public void ClearBuff() => _buffRunTimeInfos.Clear();

        public bool BuffExist(string buffId) => _buffRunTimeInfos.Any(buff => buff.BuffData.Id == buffId);

        public void TriggerCustom(string callBackName, params object[] paramList)
        {
            foreach (var buffRunTimeInfo in _buffRunTimeInfos)
            {
                buffRunTimeInfo.BuffData.TriggerBuffModule(callBackName, buffRunTimeInfo, paramList);
            }
        }
        
        private void Update()
        {
            var deleteList = new List<BuffRunTimeInfo>();
            foreach (var buff in _buffRunTimeInfos)
            {
                var tick = (IBuffTicker)buff;
                if (!buff.BuffData.IsForever)
                {
                    tick.DurationTimer -= Time.deltaTime;
                    if (tick.DurationTimer < 0)
                    {
                        deleteList.Add(buff);
                    }
                }

                if (buff.BuffData.ExistBuffModule(BuffConstant.BuffCallback.ON_TICK))
                {
                    tick.TickTimer -= Time.deltaTime;
                    if (!(tick.TickTimer < 0)) continue;
                    buff.BuffData.TriggerBuffModule(BuffConstant.BuffCallback.ON_TICK, buff);
                    tick.TickTimer = buff.BuffData.TickTime;
                }
            }

            foreach (var buff in deleteList)
            {
                RemoveBuff(buff);
            }

            while (_buff2Add.Count > 0)
            {
                var buff = _buff2Add.Dequeue();
                _buffRunTimeInfos.Add(buff);
            }
        }

        public bool AddBuff(string buffId, GameObject creator)
        {
            var buffData = BuffModel.GetBuffData(buffId);
            if (buffData != null)
            {
                return AddBuff(new BuffRunTimeInfo
                {
                    BuffData = buffData,
                    Creator  = creator,
                    Target   = gameObject
                });
            }

            return false;
        }

        private bool AddBuff(BuffRunTimeInfo buff)
        {
            var findBuff = _buffRunTimeInfos.Find(runtimeBuff => runtimeBuff.BuffData.Id == buff.BuffData.Id);
            if (findBuff == null)
            {
                _buff2Add.Enqueue(buff);
                var tick = (IBuffTicker)buff;
                tick.DurationTimer = buff.BuffData.Duration;
                tick.TickTimer = buff.BuffData.TickTime;
                buff.BuffData.TriggerBuffModule(BuffConstant.BuffCallback.ON_CREATE, buff);
                if (buff.BuffData.TriggerTickOnCreate)
                {
                    buff.BuffData.TriggerBuffModule(BuffConstant.BuffCallback.ON_TICK, buff);
                }
            }
            else
            {
                var ticker = (IBuffTicker)findBuff;
                switch (findBuff.BuffData.BuffUpdateEnum)
                {
                    case BuffUpdateEnum.AddTime:
                        ticker.DurationTimer += findBuff.BuffData.Duration;
                        break;
                    case BuffUpdateEnum.ReplaceAndAddStack:
                        ticker.DurationTimer = findBuff.BuffData.Duration;
                        if (ticker.CurStack < findBuff.BuffData.MaxStack)
                        {
                            ticker.CurStack++;
                            findBuff.BuffData.TriggerBuffModule(BuffConstant.BuffCallback.ON_ADD_STACK, findBuff);
                        }

                        break;
                    case BuffUpdateEnum.KeepAndAddStack:
                        if (ticker.CurStack < findBuff.BuffData.MaxStack)
                        {
                            ticker.CurStack++;
                            findBuff.BuffData.TriggerBuffModule(BuffConstant.BuffCallback.ON_ADD_STACK, findBuff);
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return true;
        }

        public bool RemoveBuff(string buffId)
        {
            var findBuff = _buffRunTimeInfos.Find(runtimeBuff => runtimeBuff.BuffData.Id == buffId);
            if (findBuff == null) return false;
            return RemoveBuff(findBuff);
        }

        private bool RemoveBuff(BuffRunTimeInfo buff)
        {
            var findBuff = _buffRunTimeInfos.Find(runtimeBuff => runtimeBuff.BuffData.Id == buff.BuffData.Id);
            if (findBuff == null)
            {
                return false;
            }

            //接口的隐式实现，防止外界意外修改
            var tick = (IBuffTicker)findBuff;
            switch (findBuff.BuffData.BuffRemoveEnum)
            {
                case BuffRemoveEnum.Clear:
                    findBuff.BuffData.TriggerBuffModule(BuffConstant.BuffCallback.ON_REDUCE_STACK, findBuff);
                    findBuff.BuffData.TriggerBuffModule
                        (BuffConstant.BuffCallback.ON_REMOVE,       findBuff);
                    _buffRunTimeInfos.Remove(findBuff);
                    break;
                case BuffRemoveEnum.Reduce:
                    tick.CurStack--;
                    tick.DurationTimer = findBuff.BuffData.Duration;
                    findBuff.BuffData.TriggerBuffModule(BuffConstant.BuffCallback.ON_REDUCE_STACK, findBuff);
                    if (tick.CurStack <= 0)
                    {
                        _buffRunTimeInfos.Remove(findBuff);
                        findBuff.BuffData.TriggerBuffModule(BuffConstant.BuffCallback.ON_REMOVE, findBuff);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return true;
        }
    }
}