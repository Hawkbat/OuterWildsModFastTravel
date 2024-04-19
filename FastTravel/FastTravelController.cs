using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FastTravel
{
    public class FastTravelController : MonoBehaviour
    {
        AstroObject targetAstroObject;
        ScreenPrompt travelPrompt;

        ReferenceFrameTracker referenceFrameTracker;

        void Awake()
        {
            travelPrompt = new ScreenPrompt(InputLibrary.markEntryOnHUD, "Fast Travel");

            referenceFrameTracker = FindObjectOfType<ReferenceFrameTracker>();

            GlobalMessenger<ReferenceFrame>.AddListener("TargetReferenceFrame", OnTargetReferenceFrame);
            GlobalMessenger.AddListener("UntargetReferenceFrame", OnUntargetReferenceFrame);
            GlobalMessenger.AddListener("EnterMapView", OnEnterMapView);
            GlobalMessenger.AddListener("ExitMapView", OnExitMapView);
        }

        void OnDestroy()
        {
            GlobalMessenger<ReferenceFrame>.RemoveListener("TargetReferenceFrame", OnTargetReferenceFrame);
            GlobalMessenger.RemoveListener("UntargetReferenceFrame", OnUntargetReferenceFrame);
            GlobalMessenger.RemoveListener("EnterMapView", OnEnterMapView);
            GlobalMessenger.RemoveListener("ExitMapView", OnExitMapView);
        }

        void Update()
        {
            if (PlayerState.InMapView())
            {
                var hasTarget = targetAstroObject != null;
                var inShip = PlayerState.AtFlightConsole();
                travelPrompt.SetVisibility(hasTarget && inShip);
                if (OWInput.IsNewlyPressed(InputLibrary.markEntryOnHUD) && hasTarget && inShip)
                {
                    var name = targetAstroObject.GetAstroObjectName() == AstroObject.Name.CustomString ? targetAstroObject.GetCustomName() : targetAstroObject.GetAstroObjectName().ToString();
                    referenceFrameTracker.UntargetReferenceFrame(false);
                    Locator.GetMapController().ExitMapView();
                    TravelTo(name, false);
                }
            }
        }

        void OnTargetReferenceFrame(ReferenceFrame referenceFrame)
        {
            targetAstroObject = referenceFrame.GetAstroObject();
        }

        void OnUntargetReferenceFrame()
        {
            targetAstroObject = null;
        }

        void OnEnterMapView()
        {
            Locator.GetPromptManager().AddScreenPrompt(travelPrompt, PromptPosition.BottomCenter);
        }

        void OnExitMapView()
        {
            Locator.GetPromptManager().RemoveScreenPrompt(travelPrompt, PromptPosition.BottomCenter);
        }

        public void TravelTo(string astroObjectName, bool instant)
        {
            if (instant)
            {
                if (!PlayerState.AtFlightConsole())
                {
                    var playerSpawner = Locator.GetPlayerBody().GetComponent<PlayerSpawner>();
                    playerSpawner.DebugWarp(playerSpawner.GetSpawnPoint(SpawnLocation.Ship));
                    var cockpit = Locator.GetShipBody().GetComponentInChildren<ShipCockpitController>();
                    cockpit.OnPressInteract();
                }

                var (planetT, pos, rot, known) = FastTravel.GetLandingPad(astroObjectName);
                var worldPos = planetT.TransformPoint(pos);
                var worldRot = planetT.rotation * rot;
                var ship = Locator.GetShipBody();
                ship.WarpToPositionRotation(worldPos, worldRot);
                var body = planetT.GetAttachedOWRigidbody();
                if (body != null)
                {
                    ship.SetVelocity(body.GetPointVelocity(worldPos));
                }
                if (!known)
                {
                    StartCoroutine(DoAvoidCollisions());
                }
            }
            else
            {
                StartCoroutine(DoTravelTo(astroObjectName));
            }
        }

        IEnumerator DoAvoidCollisions()
        {
            var shipDamageController = Locator.GetShipBody().GetComponent<ShipDamageController>();
            shipDamageController.ToggleInvincibility();
            // Wait for sector colliders to be re-enabled
            yield return null;
            yield return null;
            yield return null;
            var ship = Locator.GetShipBody();
            var pos = ship.transform.position;
            var up = ship.transform.up;
            var raycastTop = pos + up * 5000f;
            if (Physics.SphereCast(raycastTop, 20f, -up, out var hit, 5100f, OWLayerMask.physicalMask))
            {
                pos = hit.point;
            }
            var steps = 5000;
            while (Physics.CheckSphere(pos, 20f, OWLayerMask.physicalMask) && steps-- > 0)
            {
                pos += up;
            }
            yield return null;
            shipDamageController.ToggleInvincibility();
        }

        IEnumerator DoTravelTo(string astroObjectName)
        {
            Locator.GetPlayerAudioController().PlayOneShotInternal(AudioType.ShipThrustIgnition);
            LoadManager.s_instance._fadeCanvas.enabled = true;
            LoadManager.s_instance._fadeImage.color = Color.black;
            SpinnerUI.Show();
            yield return new WaitForSeconds(4f);
            TravelTo(astroObjectName, true);
            LoadManager.s_instance._fadeCanvas.enabled = false;
            SpinnerUI.Hide();
        }
    }
}
