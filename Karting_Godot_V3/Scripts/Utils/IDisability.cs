using Godot;
using System;
using System.Data.SqlClient;

interface IDisability
{
    bool IsActive { get; set; }

    void OnEnable();
    void OnDisable();
}