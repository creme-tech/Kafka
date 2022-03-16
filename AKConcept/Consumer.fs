module Consumer

open Confluent.Kafka
open System
open System.Threading

let consumeTopic clientConfig topicName =
    let clientConfig: ClientConfig = clientConfig
    let topicName: string = topicName

    let config = ConsumerConfig clientConfig

    config.GroupId <- "ak-concept-group-1"
    config.AutoOffsetReset <- AutoOffsetReset.Earliest
    config.EnableAutoCommit <- false

    let cancellation = new CancellationTokenSource()

    Console.CancelKeyPress.Add (fun event ->
        event.Cancel <- true
        cancellation.Cancel())

    let builder = ConsumerBuilder<string, string> config

    use consumer = builder.Build()

    do consumer.Subscribe topicName

    let rec consumeTopic () =
        let record = consumer.Consume cancellation.Token

        printfn "[Consumer] Consumed record with key '%s' and value '%s'" record.Message.Key record.Message.Value

        consumeTopic ()

    try
        consumeTopic ()
    with
    | :? OperationCanceledException -> (* CTRL-C was pressed *) 0
    | exn -> failwithf "[Consumer] Failed to consume message: %s" exn.Message
