using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// A class that simulates fake Odin Attributes
/// to avoid compilation errors when I remove Odin from the project.
/// (I remove Odin from the project because it is a paid third-party assets
/// and I don't have the rights to redistribute it)
/// 
/// Reinstalling Odin should be straightforward: import the Odin package, delete this script from your project
/// and you should have access to the Odin Inspectors I added!

public class ShowInInspectorAttribute : Attribute
{
}
public class ReadOnlyAttribute : Attribute
{
}
public class ButtonAttribute : Attribute
{
}
public class ShowIfAttribute : Attribute
{
    public ShowIfAttribute(string str) { }
}
public class HideIfAttribute : Attribute
{
    public HideIfAttribute(string str)
    {
    }
}
public class FoldoutGroupAttribute : Attribute
{
    public FoldoutGroupAttribute(string str)
    {
    }
}
public class OnValueChangedAttribute : Attribute
{
    public OnValueChangedAttribute(string str) { }
}