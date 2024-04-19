using OWML.Common;
using OWML.ModHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FastTravel
{
    public class FastTravel : ModBehaviour
    {
        public static FastTravel Instance;
        public static ICommonCameraUtility CommonCameraUtility;
        public static FastTravelController Controller;

        static readonly Dictionary<string, (Vector3, Quaternion)> landingPads = [];

        public override object GetApi() => new FastTravelAPI();

        private void Start()
        {
            Instance = this;

            CommonCameraUtility = ModHelper.Interaction.TryGetModApi<ICommonCameraUtility>("xen.CommonCameraUtility");

            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (loadScene != OWScene.SolarSystem) return;
                ModHelper.Events.Unity.FireOnNextUpdate(() =>
                {
                    Controller = Locator.GetShipBody().gameObject.AddComponent<FastTravelController>();
                });
            };

            RegisterLandingPad(AstroObject.Name.TimberHearth.ToString(), new Vector3(0f, 0f, 329.7696f), Quaternion.Euler(0f, 180f, 180f));

            ModHelper.Console.WriteLine($"{nameof(FastTravel)} is loaded!", MessageType.Success);
        }

        public static void RegisterLandingPad(string astroObjectName, Vector3 position, Quaternion rotation)
        {
            landingPads.Add(astroObjectName, (position, rotation));
        }

        public static void UnregisterLandingPad(string astroObjectName)
        {
            landingPads.Remove(astroObjectName);
        }

        public static (Transform, Vector3, Quaternion) GetLandingPad(string astroObjectName)
        {
            AstroObject ao;

            if (Enum.TryParse<AstroObject.Name>(astroObjectName, out var name))
            {
                ao = Locator.GetAstroObject(name);
            }
            else
            {
                ao = FindObjectsOfType<AstroObject>().FirstOrDefault(o => o._name.ToString() == astroObjectName || o._customName == astroObjectName);
            }

            if (ao != null)
            {
                if (landingPads.TryGetValue(astroObjectName, out var pad))
                {
                    return (ao.transform, pad.Item1, pad.Item2);
                }

                Log($"No known landing pad for {astroObjectName}, picking a random point");

                var height = 1000f;
                var ruleset = ao.GetComponentInChildren<PlanetoidRuleset>();
                if (ruleset != null)
                {
                    height = ruleset.GetAltitudeCeiling();
                    Log($"Found planetoid ruleset radius for {astroObjectName}: {height}m");
                }
                var pos = UnityEngine.Random.onUnitSphere * height;
                var up = pos.normalized;
                var forward = Vector3.Cross(UnityEngine.Random.onUnitSphere, up);
                var rot = Quaternion.LookRotation(forward, up);
                return (ao.transform, pos, rot);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(astroObjectName), $"No AstroObject named {astroObjectName} exists.");
            }
        }

        public static void TravelTo(string astroObjectName, bool instant)
        {
            Controller.TravelTo(astroObjectName, instant);
        }

        public static void Log(string msg)
        {
            Instance.ModHelper.Console.WriteLine(msg, MessageType.Info);
        }
    }

}
