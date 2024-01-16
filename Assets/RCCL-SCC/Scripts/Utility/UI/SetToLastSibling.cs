using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetToLastSibling : MonoBehaviour
{
    private void OnEnable()
    {
        this.transform.SetAsLastSibling();
    }
}
