open Confluent.Kafka
open System.Threading

[<EntryPoint>]
let main _ =
    let clientBuilder = KafkaClient.getAdminClientBuilder ()
    let clientConfig = KafkaClient.getKafkaClientConfig ()

    let topicName = "topic-1"

    Producer.ensureTopic clientBuilder topicName
    |> Async.AwaitTask
    |> Async.RunSynchronously

    let message = Message<string, string> ()

    message.Key <- "message-key"
    message.Value <- "message-value"

    let producer =
        new Thread (
            new ThreadStart (fun () ->
                Thread.Sleep 2048 (* Delay to consumer set up properly *)

                Producer.produceMessage clientConfig topicName message
                |> ignore
            )
        )

    producer.Start ()

    Consumer.consumeTopic clientConfig topicName
