using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameOverType
{
    CoreDestroyed,
    PlayersDown,
    Victory
}
public enum EventType
{
    LightBreakdown,
    MeteorShower,
    Fire
}
public enum DamageSource
{
    Bullet,
    Fire,
    Alien,
    Meteor,
    Grenade
}
public enum ScoreID
{
    alienGreen = 10,
    alienPurple = 15,
    alienBoss = 50,
    resurrection = 20
}

public enum PlayerID
{
    Player1 = 0,
    Player2 = 1,
    Player3 = 2,
    Player4 = 3,
    NotPlayer = -1
}

public enum SpawnState { SPAWNING, WAITING, COUNTING };
/*
 public const float Bullet = 0.1f;
    public const float Fire = 0.5f;
    public const float Alien = 0.5f;
    public const float Meteor = 0.3f;
    public const float Grenade = 0.3f;
 */
