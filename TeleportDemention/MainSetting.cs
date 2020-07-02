using Smod2;
using Smod2.Attributes;
using Smod2.Config;

namespace TeleportDemention
{
    [PluginDetails(
        author = "Innocence",
        description = "description",
        id = "custom.pd.teleporting.on.portal",
        name = "pocket demention teleporting",
        configPrefix = "cpd",
        SmodMajor = 3,
        SmodMinor = 0,
        SmodRevision = 0,
        version = "MP2.2.A.3"
    )]

    public class MainSetting : Plugin
    {
        public override void Register()
        {
            AddEventHandlers(new SetEvents(this));
            AddCommand("pd", new PdCommand());
            AddConfig(new ConfigSetting("dementiontime", "1", true, "This is a description"));
        }

        public override void OnEnable()
        {
            Info(Details.name + " on");
        }

        public override void OnDisable()
        {
            Info(Details.name + " off");
        }
    }
}
