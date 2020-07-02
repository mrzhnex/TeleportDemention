using System;
using ServerMod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using UnityEngine;

namespace TeleportDemention
{
    class SetEvents : IEventHandlerRoundStart, IEventHandler106CreatePortal, IEventHandlerWaitingForPlayers
    {
        private bool flag = false;
        public SetEvents(MainSetting plugin)
        {
            Global.plugin = plugin;
        }

        public void On106CreatePortal(Player106CreatePortalEvent ev)
        {
            if (flag)
            {
                Global.portal = new Vector3(ev.Position.ToVector3().x, ev.Position.ToVector3().y, ev.Position.ToVector3().z) + Vector3.up;
            }
            flag = !flag;
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            GameObject.FindWithTag("FemurBreaker").AddComponent<PortalCheck>();
        }

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            Global.portal = new Vector3(0f, -3000f, 0f) + (Vector3.up * 1.5f);
            try
            {
                Global.TimeSleep = Convert.ToSingle(Global.plugin.GetConfigString("dementiontime"));
            }
            catch (FormatException)
            {
                Global.TimeSleep = 1f;
                Global.plugin.Info("Failed convert <dementiontime> from config file. <dementiontime> set to default value: " + Global.TimeSleep);
            }
        }
    }
}
