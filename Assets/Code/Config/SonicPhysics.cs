using UnityEngine;

[CreateAssetMenu(fileName = "SonicPhysics", menuName = "SurgeEngine/Configs/Physics/Sonic")]
public class SonicPhysics : BaseActorPhysics
{
    public float driftMaxSpeed = 20;
    public float driftCentrifugalForce = 10;
    public float driftDeactivateSpeed = 6;
}
