using Godot;
using System;
using System.Data.SqlClient;

interface IDisability
{
    bool isActive { get; set; }

    void OnEnable();
    void OnDisable();
}