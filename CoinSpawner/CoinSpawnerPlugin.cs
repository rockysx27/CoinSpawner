using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Server;
using MEC;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoinSpawner
{
    public class CoinSpawnerPlugin : Plugin<Config>
    {
        public override string Name => "CoinSpawner";
        public override string Author => "You";
        public override Version Version => new Version(1, 0, 0);

        private EventHandlers handlers;

        public override void OnEnabled()
        {
            base.OnEnabled();

            handlers = new EventHandlers(Config);
            Exiled.Events.Handlers.Server.RoundStarted += handlers.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += handlers.OnRoundEnded;
            Exiled.Events.Handlers.Player.PickingUpItem += handlers.OnPickingUpItem;
            Exiled.Events.Handlers.Map.FillingLocker += handlers.OnFillingLocker;


      Exiled.API.Features.Log.Info($"{Name} v{Version} enabled.");
        }

        public override void OnDisabled()
        {
            base.OnDisabled();

            Exiled.Events.Handlers.Server.RoundStarted -= handlers.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= handlers.OnRoundEnded;
            Exiled.Events.Handlers.Player.PickingUpItem -= handlers.OnPickingUpItem;
            Exiled.Events.Handlers.Map.FillingLocker -= handlers.OnFillingLocker;


      handlers.CleanupAllSpawned();
            Exiled.API.Features.Log.Info($"{Name} disabled.");
        }
    }

    public class EventHandlers
    {
        private readonly List<Pickup> spawnedPickups = new();
        private readonly System.Random rng = new();
        private readonly Config config;

        public EventHandlers(Config config)
        {
            this.config = config;
        }

        public void OnRoundStarted()
        {
            spawnedPickups.Clear();

            int toSpawn = config.MaxCoinsPerRound;
            int spawned = 0;

            var rooms = new List<Room>(Room.List);
            if (rooms.Count == 0)
            {
                Exiled.API.Features.Log.Warn("No rooms found - coin spawn aborted.");
                return;
            }

            int attempts = 0;
            while (spawned < toSpawn && attempts < toSpawn * 6)
            {
                attempts++;

                Room room = rooms[rng.Next(rooms.Count)];

                // Get all objects in the room
                var roomObjects = room.GameObject.GetComponentsInChildren<Transform>();

                foreach (var obj in roomObjects)
                {
                  string name = obj.name.ToLower();
                  //Exiled.API.Features.Log.Warn($"objectName: {name}");
                  // Only spawn on "allowed" primitives
                  //FUTURE FUNCTION
                }

                Vector3 basePos = room.Position;

                float offsetX = (float)(rng.NextDouble() * 2 * config.SpawnRangePerRoom - config.SpawnRangePerRoom);
                float offsetZ = (float)(rng.NextDouble() * 2 * config.SpawnRangePerRoom - config.SpawnRangePerRoom);
                Vector3 spawnPos2 = basePos + new Vector3(offsetX, 0.2f, offsetZ);

                if (!IsPositionValidForSpawn(spawnPos2))
                    continue;

                try
                {
                    Quaternion rot = Quaternion.Euler(0f, (float)rng.NextDouble() * 360f, 0f);

                    Pickup p = Pickup.CreateAndSpawn(ItemType.Coin, spawnPos2, rot);
                    if (p != null)
                    {
                        spawnedPickups.Add(p);
                        spawned++;

                        if (config.EnableGlow)
                            CreateLightAt(spawnPos2);
                    }
                    else if (config.Debug)
                        Exiled.API.Features.Log.Warn($"Spawn attempt returned null for coin at {spawnPos2}");
                }
                catch (Exception ex)
                {
                    Exiled.API.Features.Log.Error($"Exception while spawning coin: {ex}");
                }
            }
        }

    public void OnRoundEnded(RoundEndedEventArgs ev)
    {
      CleanupAllSpawned();
    }


    public void OnFillingLocker(FillingLockerEventArgs ev)
    {
      if (ev.Locker == null)
        return;

      if (rng.NextDouble() > config.CoinSpawnChancePerLocker)
        return;

      Vector3 spawnPos;

      try
      {
        spawnPos = ev.Locker.RandomChamberPosition; // may throw
      }
      catch (NullReferenceException)
      {
        // Skip this locker if it can't provide a position
        if (config.Debug)
          Exiled.API.Features.Log.Warn($"Skipping locker in room: {ev.Locker.Room.Name} because RandomChamberPosition threw null.");
        return;
      }

      try
      {
        ev.IsAllowed = true; // don't cancel default item spawn
        Pickup coin = Pickup.CreateAndSpawn(ItemType.Coin, spawnPos, Quaternion.identity);

        if (coin != null)
        {
          spawnedPickups.Add(coin);

          if (config.EnableGlow)
            CreateLightAt(spawnPos);
        }
      }
      catch (Exception ex)
      {
        Exiled.API.Features.Log.Error($"Failed to spawn coin in locker: {ex}");
      }
    }





    public void OnPickingUpItem(Exiled.Events.EventArgs.Player.PickingUpItemEventArgs ev)
        {
            if (ev.Pickup?.Type != ItemType.Coin)
                return;

            Exiled.API.Features.Log.Info($"{ev.Player.Nickname} picked up a coin at {ev.Pickup.Transform.position}.");
            spawnedPickups.Remove(ev.Pickup);
        }

        public void CleanupAllSpawned()
        {
            for (int i = spawnedPickups.Count - 1; i >= 0; i--)
            {
                try
                {
                    spawnedPickups[i]?.Destroy();
                }
                catch (Exception ex)
                {
                    if (config.Debug)
                        Exiled.API.Features.Log.Warn($"Error destroying spawned pickup: {ex}");
                }
            }

            spawnedPickups.Clear();
        }

        #region Helpers

        private void CreateLightAt(Vector3 pos)
        {
            GameObject go = new("CoinSpawner_Light");
            go.transform.position = pos + new Vector3(0f, 0.5f, 0f);

            Light light = go.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = config.GlowRange;
            light.intensity = config.GlowIntensity;
            light.color = Color.yellow;

            Timing.CallDelayed(config.LightLifetimeSeconds, () =>
            {
                try
                {
                    UnityEngine.Object.Destroy(go);
                }
                catch { }
            });
        }

        private bool IsPositionValidForSpawn(Vector3 pos)
        {
            if (pos.y < -100f || pos.y > 2000f) return false;
            return true;
        }

        #endregion
    }
}
