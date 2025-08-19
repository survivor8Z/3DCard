using DG.Tweening;
using UnityEngine;

public class Door : InteractableObject
{
    private Tween rotateTween;
    private bool isOpen = false; // �����ŵ�״̬

    public void ToggleDoor()
    {
        // �����ǰ��������ת���ڽ��У���ֹͣ��
        rotateTween?.Kill();

        if (isOpen)
        {
            // ������ǿ��ŵģ�������Ե���ת +90 �ȹ���
            rotateTween = transform.DORotate(new Vector3(0, 120, 0), 1f, RotateMode.LocalAxisAdd);
            inRoom.AddTheObstacle(this);
            isOpen = false;
        }
        else
        {
            // ������ǹ��ŵģ�������Ե���ת -90 �ȴ�
            rotateTween = transform.DORotate(new Vector3(0, -120, 0), 1f, RotateMode.LocalAxisAdd);
            inRoom.DelTheObstacle(this);
            isOpen = true;

            MapMgr.Instance.RandomAddTheRoom("RoomStart", inRoom.roomBigWorldPivotCoor + pivotFront);
            //MapMgr.Instance.AddTheRoom("RoomStart", inRoom.roomBigWorldPivotCoor + pivotFront,Vector2Int.down);
        }
    }
}