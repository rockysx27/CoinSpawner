# 🪙 CoinSpawner — Random Coin Spawner Plugin for SCP: Secret Laboratory (EXILED)

A lightweight **SCP:SL EXILED plugin** that randomly spawns **coins** around the facility each round — in rooms, and optionally inside lockers — with glowing effects for visibility and configurable behavior.

---

## ✨ Features

- 💰 Randomly spawns coins across rooms every round  
- 🔦 Optional glowing light effects under each coin  
- 🎒 Chance-based coin spawning in lockers  
- 🧹 Automatic cleanup of all spawned coins at round end  
- ⚙️ Fully configurable spawn count, glow settings, and more  
- 🧾 Debug logging to help you tune spawn behavior  

---

## ✨ FUTURE UPDATE WILL INCLUDE:

- 💰 Will randomly spawn coins across the facility every round, however, they will only spawn on specified `GameObject`s. A list of string names of the game-objects will be used to derive which game-object should have a chance for a coin to spawn on it.
This change is needed so that coins don't spawn on the floor.

---

## ⚙️ Configuration (`config.yml`)

Example config file:

```yml
# Maximum number of coins to spawn per round
MaxCoinsPerRound: 10

# How far from a room center coins can appear (in meters)
SpawnRangePerRoom: 5.0

# Chance (0.0 - 1.0) for a coin to appear inside a locker
CoinSpawnChancePerLocker: 0.2

# Whether to enable glowing lights under each coin
EnableGlow: true

# Glow settings (only applies if EnableGlow = true)
GlowRange: 5.0
GlowIntensity: 3.0
LightLifetimeSeconds: 30.0

# Enable debug logging in the console
Debug: false
