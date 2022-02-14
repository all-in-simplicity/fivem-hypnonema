namespace Hypnonema.Server.Utils
{
    using JsonNet = Newtonsoft.Json.JsonConvert;

    public class Serializer
    {
        /*  public static T Deserialize<T>(string bson)
          {
              if (!BsonClassMap.IsClassMapRegistered(typeof(NetworkArgument))) RegisterClassMap();
  
              var data = Convert.FromBase64String(bson);
              var networkArgument = BsonSerializer.Deserialize<NetworkArgument>(data);
  
              return JsonNet.DeserializeObject<T>(networkArgument.Payload);
          }
  
          public static string Serialize(object obj)
          {
              if (!BsonClassMap.IsClassMapRegistered(typeof(NetworkArgument))) RegisterClassMap();
  
              var ms = new MemoryStream();
  
              using (var writer = new BsonBinaryWriter(ms))
              {
                  var json = JsonNet.SerializeObject(obj);
                  var networkArgument = new NetworkArgument(json);
  
                  BsonSerializer.Serialize(writer, networkArgument);
              }
  
              return Convert.ToString(ms.ToArray());
          }
  
          private static void RegisterClassMap()
          {
              BsonClassMap.RegisterClassMap<NetworkArgument>(
                  cm =>
                      {
                          cm.AutoMap();
                          cm.MapCreator(n => new NetworkArgument(n.Payload));
                      });
          }*/
    }
}