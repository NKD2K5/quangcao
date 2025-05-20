using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

public static class SessionExtensions
{
    // Extension method để lưu đối tượng vào session dưới dạng JSON
    public static void SetObjectAsJson(this ISession session, string key, object value)
    {
        session.SetString(key, JsonConvert.SerializeObject(value));
    }

    // Extension method để lấy đối tượng từ session từ dạng JSON
    public static T GetObjectFromJson<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default : JsonConvert.DeserializeObject<T>(value);
    }
}
