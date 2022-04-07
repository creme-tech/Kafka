module Settings

open Microsoft.Extensions.Configuration

let private builder =
    let config = ConfigurationBuilder()

    config.AddEnvironmentVariables()

[<CLIMutable>]
type KafkaSettings =
    { Brokers: string
      GroupId: string
      Topic: string
      Username: string
      Password: string }

let getKafkaSettings () =
    builder
        .Build()
        .GetSection("KafkaSettings")
        .Get<KafkaSettings>()