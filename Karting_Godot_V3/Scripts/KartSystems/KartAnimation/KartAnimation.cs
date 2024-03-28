using Godot;
using System;
using System.Numerics;

public class KartAnimation : Node
{
    public class Wheel {
        public Basis m_SteerlessLocalRotation;
        public MeshInstance wheelObj;

        public void Setup(MeshInstance wheel) {
            wheelObj = wheel;
            var wheelTransform = wheelObj.Transform;
            m_SteerlessLocalRotation = wheelTransform.basis;
        }

        //public void StoreDefaultRotation() => m_SteerlessLocalRotation = wheelTransform.basis;
        public void SetToDefaultRotation() {
            var wheelTransform = wheelObj.Transform;
            wheelTransform.basis = m_SteerlessLocalRotation;
            wheelObj.Transform = wheelTransform;
        }

        public void rotateByDegrees(float angle) {
            wheelObj.RotateY(Mathf.Deg2Rad(angle));
        }
    }

    // The damping for the appearance of steering compared to the input.  The higher the number the less damping.
    public float steeringAnimationDamping = 10f;
    // The maximum angle in degrees that the front wheels can be turned away from their default positions, when the Steering input is either 1 or -1.
    public float maxSteeringAngle = 30f;
    float m_SmoothedSteeringInput;

    [Export]
    public NodePath frontLeftWheelPath;
    [Export]
    public NodePath frontRightWheelPath;
    //[Export]
    //public NodePath rearLeftWheelPath;
    //[Export]
    //public NodePath rearRightWheelPath;
    public Wheel frontLeftWheel = new Wheel();
    public Wheel frontRightWheel = new Wheel();
    //public Wheel rearLeftWheel;
    //public Wheel rearRightWheel;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var leftVehicleWheel = GetNode<MeshInstance>(frontLeftWheelPath);
        var rightVehicleWheel = GetNode<MeshInstance>(frontRightWheelPath);
        frontLeftWheel.Setup(leftVehicleWheel);
        frontRightWheel.Setup(rightVehicleWheel);
        //rearLeftWheel.Setup(GetNode<VehicleWheel>(rearLeftWheelPath));
        //rearRightWheel.Setup(GetNode<VehicleWheel>(rearRightWheelPath));
        GD.Print(leftVehicleWheel);

    }

    public override void _PhysicsProcess(float delta)
    {
        m_SmoothedSteeringInput = Mathf.Lerp(m_SmoothedSteeringInput, Input.GetAxis("right","left"), steeringAnimationDamping * delta);

        float rotationAngle = m_SmoothedSteeringInput * maxSteeringAngle;
        //GD.Print("rotation angle: " + rotationAngle);

        //frontLeftWheel.SetToDefaultRotation();
        frontLeftWheel.rotateByDegrees(-rotationAngle);

        //frontRightWheel.SetToDefaultRotation();
        frontRightWheel.rotateByDegrees(rotationAngle);
        
        //MeshInstance wheelMesh = (MeshInstance) frontLeftWheel.wheelObj.GetChild(0);
        //wheelMesh.RotateY(Mathf.Deg2Rad(2));

        //frontLeftWheel.wheelObj.RotateY(Mathf.Deg2Rad(20));
        //GD.Print(frontLeftWheel.wheelObj.RotationDegrees);
    }
}
