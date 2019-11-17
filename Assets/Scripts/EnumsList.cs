﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    LightBreakdown,
    MeteorShower,
    Fire
}
public static class DamageSource
{
    public const float Bullet = 0.1f;
    public const float Fire = 0.5f;
    public const float Alien = 0.5f;
    public const float Meteor = 0.3f;
    public const float Grenade = 0.3f;
}
