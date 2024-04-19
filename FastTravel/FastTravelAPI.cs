using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FastTravel
{
    public class FastTravelAPI : IFastTravelAPI
    {
        public (Transform, Vector3, Quaternion) GetLandingPad(string astroObjectName)
            => FastTravel.GetLandingPad(astroObjectName);

        public void RegisterLandingPad(string astroObjectName, Vector3 position, Quaternion rotation)
            => FastTravel.RegisterLandingPad(astroObjectName, position, rotation);

        public void UnregisterLandingPad(string astroObjectName)
            => FastTravel.UnregisterLandingPad(astroObjectName);

        public void TravelTo(string astroObjectName, bool instant)
            => FastTravel.TravelTo(astroObjectName, instant);
    }
}
