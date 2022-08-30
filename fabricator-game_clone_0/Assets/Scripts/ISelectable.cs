using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable
{
    GameObject gameObject { get; }
    void Select();
    void Deselect();
}
