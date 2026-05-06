using System;
using System.Collections.Generic;

[Serializable]
public sealed class MinimapPointDto
{
    public float x;
    public float y;

    public MinimapPointDto(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}