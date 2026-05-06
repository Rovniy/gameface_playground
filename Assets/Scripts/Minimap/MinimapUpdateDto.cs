using System;
using System.Collections.Generic;

[Serializable]
public sealed class MinimapUpdateDto
{
    public float x;
    public float y;
    public List<MinimapPointDto> trail;

    public MinimapUpdateDto(float x, float y, List<MinimapPointDto> trail)
    {
        this.x = x;
        this.y = y;
        this.trail = trail;
    }
}