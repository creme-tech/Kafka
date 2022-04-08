open Confluent.Kafka
open System.Threading
open CremeEntity
open Serilog

let main _ =
    let clientBuilder = KafkaClient.getAdminClientBuilder ()
    let clientConfig = KafkaClient.getKafkaClientConfig ()

    let topicName = "ak-concept-topic-1"

    Producer.ensureTopic clientBuilder topicName
    |> Async.AwaitTask
    |> Async.RunSynchronously

    let message = Message<string, string>()

    message.Key <- "message-key"
    message.Value <- "message-value"

    let producer =
        new Thread(
            new ThreadStart(fun () ->
                Thread.Sleep 2048 (* Delay to consumer set up properly *)

                Producer.produceMessage clientConfig topicName message
                |> ignore)
        )

    producer.Start()

    Consumer.consumeTopic clientConfig topicName

let handle log consumeResult =
    task {
        let log: ILogger = log
        let consumeResult: ConsumeResult<string, string> = consumeResult
        let dto =
            consumeResult.Message.Value
            |> Helper.deserialize<CremeEvent<NotificationEventDTO>>
        
        match dto with
        | Ok dtoData ->
            log.Information $"[Consumer] Consumed record with key %s{consumeResult.Message.Key} and action %s{dtoData.data.Action}"
            log.Information $"[Consumer] Consumed record with key %s{consumeResult.Message.Key} and targets %s{dtoData.data.TargetsType}"
        | Error _ ->
            log.Error $"[Consumer] Cannot decode event"
    }

[<EntryPoint>]
let main2 _ =
    let consumerConfig = CremeKafka.config
    let topic = CremeKafka.Topic CremeKafka.settings.Topic
    let cancellation = new CancellationTokenSource()
    use logger = LoggerConfiguration()
                     .WriteTo.Console()
                     .CreateLogger()
    let consumeTopic = CremeKafka.consumeTopic logger handle cancellation.Token consumerConfig
    
    topic
    |> consumeTopic
    |> Async.AwaitTask
    |> Async.RunSynchronously
    
    0
    