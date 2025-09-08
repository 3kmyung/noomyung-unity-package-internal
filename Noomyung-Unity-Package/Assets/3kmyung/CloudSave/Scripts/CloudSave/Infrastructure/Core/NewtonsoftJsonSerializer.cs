using _3kmyung.CloudSave.Domain;

namespace _3kmyung.CloudSave.Infrastructure
{
    /// <summary>
    /// Newtonsoft.Json을 사용한 JSON 직렬화기 구현
    /// </summary>
    public sealed class NewtonsoftJsonSerializer : IJsonSerializer
    {
        public string Serialize<T>(T value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }

        public T Deserialize<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }
    }
}
