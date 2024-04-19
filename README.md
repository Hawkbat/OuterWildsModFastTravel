# Fast Travel
![A banner depicting a ship traveling quickly between planets.](banner.png)


Enables fast travel to planets while using the map and piloting the ship.

Includes a fake loading screen for maximum immersion!

The mod picks a random arrival point unless a specific landing pad is registered for that planet. This can be done via the mod's API:

```csharp
var fastTravelAPI = ModHelper.Interaction.TryGetModApi<IFastTravelAPI>("Hawkbar.FastTravel");
// Defines a fast-travel landing pad for the Attlerock ("TimberMoon"):
fastTravelAPI.RegisterLandingPad("TimberMoon", new Vector3(150f, 0f, 0f), Quaterion.Euler(180f, 180f, 0f));
```
