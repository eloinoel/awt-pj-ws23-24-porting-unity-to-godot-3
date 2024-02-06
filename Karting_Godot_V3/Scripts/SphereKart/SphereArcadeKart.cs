using Godot;
using System;

public class SphereArcadeKart : Spatial
{
    //node references
    private RigidBody sphere;
    private Spatial carMesh;
    private RayCast groundRay;

    //car mesh position relative to sphere
    Vector3 sphereOffset = new Vector3(0f, -0.993f, 0.133f);
    //engine power
    float acceleration = 50f;
    //turn amount in degrees
    float steering = 21f;
    // how quickly the kart turns
    float turnSpeed = 5f;
    //below this speed the car doesn't turn
    float turnStopLimit = 0.75f;

    //input values
    float speedInput = 0f;
    float rotateInput = 0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        sphere = GetNode<RigidBody>("Sphere");
        carMesh = GetNode<Spatial>("Kart");
        groundRay = GetNode<RayCast>("Kart/RayCast");

        groundRay.AddException(sphere);
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        Transform carMeshTransform = carMesh.Transform;
        carMeshTransform.origin = sphere.Transform.origin + sphereOffset;
        carMesh.Transform = carMeshTransform;

        //drive forward
        sphere.AddCentralForce(carMesh.GlobalTransform.basis.z * speedInput);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        if (!groundRay.IsColliding())
            return;

        //accelerate/brake input
        speedInput = Input.GetActionStrength("forward") - Input.GetActionStrength("backward");
        speedInput *= acceleration;

        //steering input
        rotateInput = Input.GetActionStrength("left") - Input.GetActionStrength("right");
        rotateInput *= Mathf.Deg2Rad(steering);

        //rotate car mesh
        if (sphere.LinearVelocity.Length() > turnStopLimit)
        {
            Basis newBasis = carMesh.GlobalTransform.basis.Rotated(carMesh.GlobalTransform.basis.y, rotateInput);
            Transform globalCarTransform = carMesh.GlobalTransform;
            globalCarTransform.basis = carMesh.GlobalTransform.basis.Slerp(newBasis, turnSpeed * delta);
            carMesh.GlobalTransform = globalCarTransform.Orthonormalized();
        }

        //align with ground
        Vector3 groundNormal = groundRay.GetCollisionNormal();
        Transform xform = AlignWithY(carMesh.GlobalTransform, groundNormal.Normalized());
        carMesh.GlobalTransform = carMesh.GlobalTransform.InterpolateWith(xform, delta*20);
    }

    private Transform AlignWithY(Transform xform, Vector3 newY)
    {
        xform.basis.y = newY;
        xform.basis.x = -xform.basis.z.Cross(newY);
        xform.basis = xform.basis.Orthonormalized();
        return xform;
    }
}
