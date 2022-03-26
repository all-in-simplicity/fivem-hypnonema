namespace Hypnonema.Client.Utils
{
    using CitizenFX.Core.Native;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class Nui
    {
        public static readonly JsonSerializerSettings NuiSerializerSettings =
            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        public static void SendMessage(string type, object payload)
        {
            var message = new { type, payload };

            API.SendNuiMessage(JsonConvert.SerializeObject(message, NuiSerializerSettings));
        }
    }
}