using UnityEngine;

[CreateAssetMenu(fileName = "CurveParameters", menuName = "Hand Curve Parameters")]
public class CurveParameters : ScriptableObject
{
    public AnimationCurve positioning;
    public float positioningInfluence = .1f;
    public AnimationCurve rotation;
    public float rotationInfluence = 10f;
    public AnimationCurve scaler;
    public float scalerInfluence = 10f;

}
