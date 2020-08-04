using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using KSP.UI;
using KSP.UI.Screens;

namespace OrbitAlarm
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class OrbitAlarm : MonoBehaviour
    {
        static List<Guid> alarmedVessels = new List<Guid>(); 
        public void FixedUpdate()
        {

            if (HighLogic.LoadedSceneIsGame && HighLogic.LoadedScene != GameScenes.LOADING && HighLogic.LoadedScene != GameScenes.LOADINGBUFFER && HighLogic.LoadedScene != GameScenes.MAINMENU)
            {
                for (int i = 0; i < FlightGlobals.Vessels.Count; i++)
                {
                    Vessel vessel = FlightGlobals.Vessels.ElementAt(i);
                    if (vessel.situation == Vessel.Situations.ORBITING && vessel != FlightGlobals.ActiveVessel && vessel.vesselType != VesselType.Debris && vessel.vesselType != VesselType.SpaceObject)
                    {
                        var testOrbit = vessel.orbit;
                        var Celestial = testOrbit.referenceBody;
                        double WarningHeight; 
                        if (Celestial.atmosphere)
                        {
                            WarningHeight = Celestial.atmosphereDepth + 20000;
                        } else
                        {
                            WarningHeight = 100000;
                        }
                        
                        if (testOrbit.PeA > WarningHeight + 10000 && alarmedVessels.Contains(vessel.id))
                        {
                            alarmedVessels.Remove(vessel.id);
                        } 
                        else if (testOrbit.PeA < WarningHeight && !alarmedVessels.Contains(vessel.id))
                        {
                            TimeWarp.SetRate(0, true, true);
                            AlertMessage("Orbit Alarm", vessel.name + " has had its Pe drop to " + testOrbit.PeA.ToString());
                            alarmedVessels.Add(vessel.id);
                        }
                    }
                }
            }
        }
        private void AlertMessage(String title, String text)
        {

            MessageSystem.Message m = new MessageSystem.Message(title, text, MessageSystemButton.MessageButtonColor.YELLOW, MessageSystemButton.ButtonIcons.ALERT);
            MessageSystem.Instance.AddMessage(m);
        }
    }

}