using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformSize : Drop
{
    public float NewSize = Platform.SizeNormal;

    protected override void Pickup()
    {
        if (Platform.Instance != null && !Platform.Instance.IsTransforming)
        {
            Platform.Instance.Resize(NewSize);
        }
    }
}
