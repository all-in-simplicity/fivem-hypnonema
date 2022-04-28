namespace Hypnonema.Server
{
    using CitizenFX.Core;
    using CitizenFX.Core.Native;

    public static class PlayerExtensions
    {
        public static void AddChatMessage(this Player p, string message, int[] color = null)
        {
            if (color == null) color = new[] {0, 128, 128};

            p.TriggerEvent("chat:addMessage", new {color, args = new[] {"[Hypnonema]", $"{message}"}});
        }

        public static bool IsAceAllowed(this Player p, string ace)
        {
            return API.IsPlayerAceAllowed(p.Handle, ace);
        }
    }
}