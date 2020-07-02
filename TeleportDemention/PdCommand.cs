using System;
using System.Collections.Generic;
using System.Threading;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.Commands;
using UnityEngine;
using ServerMod2.API;
using MEC;

namespace TeleportDemention
{
    class PdCommand : ICommandHandler
    {
        private bool flag;

        public string GetCommandDescription()
        {
            return "usage: pd <id> or <nickname>";
        }

        public string GetUsage()
        {
            return "<pd> id/nick";
        }

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            if (args.Length < 1)
            {
                return new string[] { "out of arguments" };
            }
            int id = -1;
            string name = "";
            try
            {
                id = Convert.ToInt16(args[0]);
            }
            catch (FormatException)
            {
                id = -1;
                name = args[0];
            }
            if (id == -1)
            {
                foreach (Player p in Global.plugin.Server.GetPlayers())
                {
                    if (p.Name.ToLower().Contains(name.ToLower()))
                    {
                        if (p.TeamRole.Team == Smod2.API.Team.SCP || p.TeamRole.Team == Smod2.API.Team.SPECTATOR)
                        {
                            return new string[] { "Player " + p.Name + " is scp or spectator" };
                        }
                        if ((p.GetGameObject() as GameObject).GetComponent<TargetTeleport>() == null)
                        {
                            if ((p.GetGameObject() as GameObject).GetComponent<TimeHoleStuck>() != null)
                            {
                                (p.GetGameObject() as GameObject).GetComponent<TimeHoleStuck>().timeHole = 0f;
                            }
                            Thread thread = new Thread(delegate ()
                            {
                                GetScp106(p.GetGameObject() as GameObject);
                            });
                            thread.Start();
                            Global.plugin.Info("Admin " + (sender as Player).Name + " run pd command on " + p.Name);
                            return new string[] { "Player " + p.Name + " teleport in demention" };
                        }
                        else
                        {
                            Global.plugin.Info("Admin " + (sender as Player).Name + " run pd command on " + p.Name + " (player already teleport)");
                            return new string[] { "Cancel: " + p.Name + " is already teleport" };
                        }
                    }
                }
            }
            else
            {
                foreach (Player p in Global.plugin.Server.GetPlayers())
                {
                    if (p.PlayerId == id)
                    {
                        if (p.TeamRole.Team == Smod2.API.Team.SCP || p.TeamRole.Team == Smod2.API.Team.SPECTATOR)
                        {
                            return new string[] { "Player " + p.Name + " is scp or spectator" };
                        }
                        if ((p.GetGameObject() as GameObject).GetComponent<TargetTeleport>() == null)
                        {
                            if ((p.GetGameObject() as GameObject).GetComponent<TimeHoleStuck>() != null)
                            {
                                (p.GetGameObject() as GameObject).GetComponent<TimeHoleStuck>().timeHole = 0f;
                            }
                            Thread thread = new Thread(delegate ()
                            {
                                GetScp106(p.GetGameObject() as GameObject);
                            });
                            thread.Start();
                            Global.plugin.Info("Admin " + (sender as Player).Name + " run pd command on " + p.Name);
                            return new string[] { "Player " + p.Name + " teleport in demention" };
                        }
                        else
                        {
                            Global.plugin.Info("Admin " + (sender as Player).Name + " run pd command on " + p.Name + " (player already teleport)");
                            return new string[] { "Cancel: " + p.Name + " is already teleport" };
                        }
                    }
                }
            }
            Global.plugin.Info("Admin " + (sender as Player).Name + " tried run pd command");
            return new string[] { "Player not found" };
        }

        private void GetScp106(GameObject gameobj)
        {
            Scp106PlayerScript component = gameobj.GetComponent<Scp106PlayerScript>();
            this.flag = true;
            Player106CreatePortalEvent player106CreatePortalEvent = new Player106CreatePortalEvent(new SmodPlayer(gameobj.GetComponent<Component>().gameObject), new Vector(Global.portal.x, Global.portal.y, Global.portal.z));
            EventManager.Manager.HandleEvent<IEventHandler106CreatePortal>(player106CreatePortalEvent);
            bool flag = player106CreatePortalEvent.Position == null;
            if (!flag)
            {
                component.NetworkportalPosition = VectorExtensions.ToVector3(player106CreatePortalEvent.Position);
                Timing.RunCoroutine(this.DoPortalSetupAnimation(component), 0);
                while (this.flag) { }
                gameobj.AddComponent<TargetTeleport>();
                Timing.RunCoroutine(this.DoTeleportAnimation(component, gameobj), 0);
            }
        }

        private IEnumerator<float> DoPortalSetupAnimation(Scp106PlayerScript scp106)
        {
            while (scp106.portalPrefab == null)
            {
                scp106.portalPrefab = GameObject.Find("SCP106_PORTAL");
                Global.plugin.Info("portal is null");
                yield return 0f;
            }
            Animator portalAnim = scp106.portalPrefab.GetComponent<Animator>();
            portalAnim.SetBool("activated", false);
            yield return Timing.WaitForSeconds(0.1f);
            scp106.portalPrefab.transform.position = scp106.portalPosition;
            portalAnim.SetBool("activated", true);
            this.flag = false;
            yield break;
        }

        private IEnumerator<float> DoTeleportAnimation(Scp106PlayerScript scp106, GameObject targetObject)
        {
            Component compon = targetObject.GetComponent<Component>();
            Vector3 pos = new Vector3(0f, -2000f, 0f) + Vector3.up * 1.5f;
            Player106TeleportEvent player106TeleportEvent = new Player106TeleportEvent(new SmodPlayer(compon.gameObject), new Vector(pos.x, pos.y, pos.z));
            EventManager.Manager.HandleEvent<IEventHandler106Teleport>(player106TeleportEvent);
            bool flag = player106TeleportEvent.Position == null;
            if (flag)
            {
                Global.plugin.Info("portal event is null");
                yield break;
            }
            pos = VectorExtensions.ToVector3(player106TeleportEvent.Position);
            scp106.CallRpcTeleportAnimation();
            scp106.goingViaThePortal = true;
            PlyMovementSync pms = compon.GetComponent<PlyMovementSync>();
            yield return Timing.WaitForSeconds(3.5f);
            pms.CallCmdSendPosition(pos);
            yield return Timing.WaitForSeconds(3.5f);
            scp106.goingViaThePortal = false;
            //remove
            yield break;
        }

    }
}
