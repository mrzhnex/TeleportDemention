using Smod2.API;
using UnityEngine;

namespace TeleportDemention
{
    class PortalCheck : MonoBehaviour
    {
        private float timer = 0f;
        private readonly float timeIsUp = 0.2f;

        public void Update()
        {
            timer = timer + Time.deltaTime;
            if (timer >= timeIsUp)
            {
                timer = 0f;
                foreach (Player p in Global.plugin.Server.GetPlayers())
                {
                    if (p.TeamRole.Team == Smod2.API.Team.SCP || p.TeamRole.Team == Smod2.API.Team.TUTORIAL || p.TeamRole.Team == Smod2.API.Team.SPECTATOR)
                    {
                        continue;
                    }
                    GameObject target = p.GetGameObject() as GameObject;
                    if (target.GetComponent<TargetTeleport>() != null)
                    {
                        continue;
                    }
                    if (!target.GetComponent<FallDamage>().isGrounded)
                    {
                        continue;
                    }
                    if (Vector3.Distance(Global.portal, target.transform.position) < Global.distance)
                    {
                        if (target.GetComponent<TimeHoleStuck>() == null)
                        {
                            target.AddComponent<TimeHoleStuck>();
                        }
                        target.GetComponent<TimeHoleStuck>().timeHole = target.GetComponent<TimeHoleStuck>().timeHole + timeIsUp;
                        if (target.GetComponent<TimeHoleStuck>().timeHole >= Global.TimeSleep)
                        {
                            target.AddComponent<TargetTeleport>();
                            Destroy(target.GetComponent<TimeHoleStuck>());
                        }
                    }
                    else
                    {
                        if (target.GetComponent<TimeHoleStuck>().timeHole > 0)
                        {
                            target.GetComponent<TimeHoleStuck>().timeHole = target.GetComponent<TimeHoleStuck>().timeHole - (timeIsUp / 2);
                        }
                    }
                }
            }
        }

    }
}
