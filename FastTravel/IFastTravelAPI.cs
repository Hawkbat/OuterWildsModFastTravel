using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FastTravel
{
    public interface IFastTravelAPI
    {
        /// <summary>
        /// Gets the registered landing pad for the chosen planet, if it exists, or a random arrival location if not.
        /// Returns the planet's Transform, the arrival point position and rotation relative to the planet, and a bool indicating if the landing pad was explicitly registered (true) or was picked randomly (false).
        /// </summary>
        /// <param name="astroObjectName">The internal name of the planet (an AstroObject.Name value like TimberHearth or TimberMoon, or the config name for a New Horizons planet).</param>
        /// <returns></returns>
        (Transform, Vector3, Quaternion, bool) GetLandingPad(string astroObjectName);
        /// <summary>
        /// Registers a landing pad for the chosen planet, which will cause the ship to always arrive at that location.
        /// </summary>
        /// <param name="astroObjectName">The internal name of the planet (an AstroObject.Name value like TimberHearth or TimberMoon, or the config name for a New Horizons planet).</param>
        /// <param name="position">The position relative to the planet that the ship will teleport to.</param>
        /// <param name="rotation">The rotation relative to the planet that the ship will have after teleporting.</param>
        /// <returns></returns>
        void RegisterLandingPad(string astroObjectName, Vector3 position, Quaternion rotation);
        /// <summary>
        /// Removes the registered landing pad for the chosen planet (for example, if it has been disabled or destroyed).
        /// </summary>
        /// <param name="astroObjectName">The internal name of the planet (an AstroObject.Name value like TimberHearth or TimberMoon, or the config name for a New Horizons planet).</param>
        /// <returns></returns>
        void UnregisterLandingPad(string astroObjectName);
        /// <summary>
        /// Fast-travels the ship to the chosen planet, same as if the player triggered it via the map.
        /// </summary>
        /// <param name="astroObjectName">The internal name of the planet (an AstroObject.Name value like TimberHearth or TimberMoon, or the config name for a New Horizons planet).</param>
        /// <param name="instant">Whether to instantly teleport without loading transitions or not.</param>
        /// <returns></returns>
        void TravelTo(string astroObjectName, bool instant);
    }
}
