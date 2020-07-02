using MEC;
using RemoteAdmin;
using ServerMod2.API;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using System.Collections.Generic;
using UnityEngine;

namespace TeleportDemention
{
    internal class TargetTeleport : MonoBehaviour
    {
        private float timer = 0f;
        private readonly float timeIsUp = 0.025f;

        public void Start()
        {
            foreach (Player player in Global.plugin.Server.GetPlayers())
            {
                if (player.PlayerId == gameObject.GetComponent<QueryProcessor>().PlayerId)
                {
                    target = player;
                    break;
                }
            }
            heightY = gameObject.transform.position.y - 1.7f;

            
            Player106CreatePortalEvent player106CreatePortalEvent = new Player106CreatePortalEvent(target, new Vector(Global.portal.x, Global.portal.y, Global.portal.z));
            EventManager.Manager.HandleEvent<IEventHandler106CreatePortal>(player106CreatePortalEvent);
            if (player106CreatePortalEvent.Position != null)
            {
                gameObject.GetComponent<Scp106PlayerScript>().NetworkportalPosition = VectorExtensions.ToVector3(player106CreatePortalEvent.Position);
                Timing.RunCoroutine(DoPortalSetupAnimation(), 0);
            } 
        }
        
        public void Update()
        {
            timer += Time.deltaTime;
            if (!flag)
            {
                Timing.RunCoroutine(DoTeleportAnimation(), 0);
            }
            if (timer >= timeIsUp)
            {
                timer = 0f;
                if (gameObject.transform.position.y > heightY)
                {
                    target.Teleport(new Vector(gameObject.transform.position.x, gameObject.transform.position.y - step, gameObject.transform.position.z), Unstuck);
                }
                if (gameObject.transform.position.y <= heightY && !gameObject.GetComponent<Scp106PlayerScript>().goingViaThePortal)
                {
                    Destroy(gameObject.GetComponent<TargetTeleport>());
                }
            }
        }

        private IEnumerator<float> DoPortalSetupAnimation()
        {
            while (gameObject.GetComponent<Scp106PlayerScript>().portalPrefab == null)
            {
                gameObject.GetComponent<Scp106PlayerScript>().portalPrefab = GameObject.Find("SCP106_PORTAL");
                Global.plugin.Info("portal is null");
                yield return 0f;
            }
            Animator portalAnim = gameObject.GetComponent<Scp106PlayerScript>().portalPrefab.GetComponent<Animator>();
            portalAnim.SetBool("activated", false);
            yield return Timing.WaitForSeconds(0.1f);
            gameObject.GetComponent<Scp106PlayerScript>().portalPrefab.transform.position = gameObject.GetComponent<Scp106PlayerScript>().portalPosition;
            portalAnim.SetBool("activated", true);
            flag = false;
            yield break;
        }

        private IEnumerator<float> DoTeleportAnimation()
        {
            flag = true;
            Component compon = gameObject.GetComponent<Component>();
            Vector3 pos = new Vector3(0f, -2000f, 0f) + Vector3.up * 1.5f;
            Player106TeleportEvent player106TeleportEvent = new Player106TeleportEvent(new SmodPlayer(compon.gameObject), new Vector(pos.x, pos.y, pos.z));
            EventManager.Manager.HandleEvent<IEventHandler106Teleport>(player106TeleportEvent);
            if (player106TeleportEvent.Position == null)
            {
                Global.plugin.Info("portal event is null");
                yield break;
            }
            pos = VectorExtensions.ToVector3(player106TeleportEvent.Position);
            gameObject.GetComponent<Scp106PlayerScript>().CallRpcTeleportAnimation();
            gameObject.GetComponent<Scp106PlayerScript>().goingViaThePortal = true;
            PlyMovementSync pms = compon.GetComponent<PlyMovementSync>();
            yield return Timing.WaitForSeconds(3.5f);
            pms.CallCmdSendPosition(pos);
            yield return Timing.WaitForSeconds(3.5f);
            gameObject.GetComponent<Scp106PlayerScript>().goingViaThePortal = false;
            yield break;
        }

        private bool flag = true;
        private const float step = 0.1f;
        private const bool Unstuck = false;
        private Player target;
        private float heightY;
    }
}
