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
        (Transform, Vector3, Quaternion) GetLandingPad(string astroObjectName);
        void RegisterLandingPad(string astroObjectName, Vector3 position, Quaternion rotation);
        void UnregisterLandingPad(string astroObjectName);
        void TravelTo(string astroObjectName, bool instant);
    }
}
