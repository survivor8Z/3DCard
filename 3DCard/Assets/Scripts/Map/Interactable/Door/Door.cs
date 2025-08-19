using DG.Tweening;
using UnityEngine;

public class Door : InteractableObject
{
    private Tween rotateTween;
    private bool isOpen = false; // 跟踪门的状态

    public void ToggleDoor()
    {
        // 如果当前有其他旋转正在进行，先停止它
        rotateTween?.Kill();

        if (isOpen)
        {
            // 如果门是开着的，让它相对地旋转 +90 度关上
            rotateTween = transform.DORotate(new Vector3(0, 120, 0), 1f, RotateMode.LocalAxisAdd);
            inRoom.AddTheObstacle(this);
            isOpen = false;
        }
        else
        {
            // 如果门是关着的，让它相对地旋转 -90 度打开
            rotateTween = transform.DORotate(new Vector3(0, -120, 0), 1f, RotateMode.LocalAxisAdd);
            inRoom.DelTheObstacle(this);
            isOpen = true;

            MapMgr.Instance.RandomAddTheRoom("RoomStart", inRoom.roomBigWorldPivotCoor + pivotFront);
            //MapMgr.Instance.AddTheRoom("RoomStart", inRoom.roomBigWorldPivotCoor + pivotFront,Vector2Int.down);
        }
    }
}