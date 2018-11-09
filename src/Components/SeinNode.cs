﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESeinNodeType
{ 
    Component = 1,
    Actor = 2
};

[AddComponentMenu("Sein/Core Components/Sein Scene Node")]
public class SeinNode : MonoBehaviour {
    public static string extensionName = "Sein_node";

    [Header("Node Type")]
    // If this node is toplevel, selfType will default to Actor
    // If not toplevel, it will default to Inherit
    // If parent's type is Component, it must be Component
    public ESeinNodeType selfType = ESeinNodeType.Actor;

    // If this node is toplevel, childrenType will default to Actor
    // If not toplevel, it will default to Inherit
    public ESeinNodeType childrenType = ESeinNodeType.Component;

    // Only works when selfType is Actor
    [Header("Properties for actor")]
    public new string tag = "";

    public bool persistent = false;
    
    public bool emitComponentsDestroy = true;

    // no logic component
    // onUpdate will not be trigger
    public bool isStatic = true;
}
