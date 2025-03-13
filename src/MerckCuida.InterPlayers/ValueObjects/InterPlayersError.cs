using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace MerckCuida.InterPlayers.ValueObjects
{
    public class InterPlayersError
    {
        public string? Raw { get; set; }
        public DefaultData? Deserialized { get; set; }

        public class DefaultData
        {
            public DefaultDataDetails? data { get; set; }
        }

        public class DefaultDataDetails
        {
            public string? error { get; set; }
            public string? errorDescription { get; set; }

            public bool IsProfissionalInvalido() => error == "LP04";
        }

        public static InterPlayersError Deserialize(string data)
        {
            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.PropertyNameCaseInsensitive = true;

            var deserialized = JsonSerializer.Deserialize<DefaultData>(data, jsonOptions);

            return new InterPlayersError()
            {
                Raw = data,
                Deserialized = deserialized
            };
        }

        public static async Task<InterPlayersError> DeserializeAsync(Stream stream)
        {
            using var streamReader = new StreamReader(stream);
            var data = await streamReader.ReadToEndAsync();

            return Deserialize(data);
        }
    }
}