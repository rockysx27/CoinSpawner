using Exiled.API.Interfaces;
using System;
namespace CoinSpawner
{
  public class Config : IConfig
  {
    public bool IsEnabled { get; set; } = true;
    public int MaxCoinsPerRound { get; set; } = 21;
    public float CoinSpawnChancePerLocker { get; set; } = 0.1f;
    public float SpawnRangePerRoom { get; set; } = 2f;
    public bool EnableGlow { get; set; } = true;
    public float GlowIntensity { get; set; } = 3f;
    public float GlowRange { get; set; } = 4f;
    public float LightLifetimeSeconds { get; set; } = 300f;
    public bool Debug { get; set; } = false;
  }
}
