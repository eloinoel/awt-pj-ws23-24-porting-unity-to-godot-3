using Godot;
using System;

public class DebugDrawing : Node
{
    Color defaultColor = new Color(1, 0, 0, 1);


    /*
     * Color can be null
     */
    void DrawSphere(Vector3 position, Color color, float radius = 0.1f, float duration = 20)
    {
        if(color == null) { color = defaultColor; }

        MeshInstance meshInstance = SetupSphereMesh(position, radius, color);

        GetTree().Root.AddChild(meshInstance);

        FreeMeshAfterDelay(meshInstance, duration);
    }

    /*
     * Color can be null
     */
    void DrawLine(Vector3 pos1, Vector3 pos2, Color color, float duration = 20)
    {
        if(color == null) { color = defaultColor; }

        ImmediateGeometry immediateGeometry = new ImmediateGeometry();
        SpatialMaterial material = new SpatialMaterial();

        material.AlbedoColor = color;

        immediateGeometry.Begin(Mesh.PrimitiveType.Lines);
        immediateGeometry.AddVertex(pos1);
        immediateGeometry.AddVertex(pos2);
        immediateGeometry.AddVertex(new Vector3(pos1.x + 0.001f, pos1.y, pos1.z));
        immediateGeometry.AddVertex(new Vector3(pos2.x + 0.001f, pos2.y, pos2.z));
        immediateGeometry.End();

        GetTree().Root.AddChild(immediateGeometry);

        FreeGeometryAfterDelay(immediateGeometry, duration);
    }


    //-----------------------------------------------
    //-------------- Private functions --------------
    //-----------------------------------------------

    private MeshInstance SetupSphereMesh(Vector3 position, float radius, Color color)
    {
        MeshInstance meshInstance = new MeshInstance();
        Transform transform = meshInstance.Transform;
        transform.origin = position;
        meshInstance.CastShadow = MeshInstance.ShadowCastingSetting.Off;

        // Setup mesh for the MeshInstance node
        SphereMesh mesh = new SphereMesh();
        mesh.Radius = radius;
        mesh.Height = radius * 2;
        meshInstance.Mesh = mesh;

        SpatialMaterial material = new SpatialMaterial();
        material.AlbedoColor = color;
        meshInstance.MaterialOverride = material;

        return meshInstance;
    }

    private async void FreeMeshAfterDelay(MeshInstance meshInstance, float delay)
    {
        await ToSignal(GetTree().CreateTimer(delay), "timeout");
        meshInstance.QueueFree();
    }

    private async void FreeGeometryAfterDelay(ImmediateGeometry geometry, float delay)
    {
        await ToSignal(GetTree().CreateTimer(delay), "timeout");
        geometry.QueueFree();
    }

}

