namespace Hypnonema.Client.Utils
{
    using CitizenFX.Core.Native;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class Nui
    {
        public static readonly JsonSerializerSettings NuiSerializerSettings =
            new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};

        public static void SendMessage(string method, object data)
        {
            var message = new {app = "hypnonema", method, data};

            API.SendNuiMessage(JsonConvert.SerializeObject(message, NuiSerializerSettings));
        }
    }
}