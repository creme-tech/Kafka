module KafkaClient

open Confluent.Kafka
open System
open System.Collections.Generic

let private getConfig () =
    let config = Dictionary<string, string>()

    config.Add("bootstrap.servers", Environment.GetEnvironmentVariable "AK_SERVER")
    config.Add("security.protocol", "SASL_SSL")
    config.Add("sasl.mechanisms", "PLAIN")
    config.Add("sasl.username", Environment.GetEnvironmentVariable "AK_USERNAME")
    config.Add("sasl.password", Environment.GetEnvironmentVariable "AK_PASSWORD")
    config.Add("session.timeout.ms", "45000")

    config

let getAdminClientBuilder () =
    let config = getConfig ()
    let adminClientBuilder = AdminClientBuilder config

    adminClientBuilder

let getKafkaClientConfig () =
    let config = getConfig ()
    let kafkaClientConfig = ClientConfig config

    kafkaClientConfig
