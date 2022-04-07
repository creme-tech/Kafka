module CremeKafka

open Confluent.Kafka
open System.Threading
open Serilog

type Topic =
    | Topic of string
    
    member self.Value =
        match self with
        | Topic value -> value

let settings = Settings.getKafkaSettings()

let config =
    let config = ConsumerConfig()
    
    config.SecurityProtocol <- SecurityProtocol.SaslSsl
    config.SaslMechanism <- SaslMechanism.ScramSha512
    config.AutoOffsetReset <- AutoOffsetReset.Earliest
    config.SocketTimeoutMs <- 20_000
    config.EnableAutoCommit <- true
    config.EnableAutoOffsetStore <- false
    config.GroupId <- settings.GroupId
    config.BootstrapServers <- settings.Brokers
    config.SaslUsername <- settings.Username
    config.SaslPassword <- settings.Password
    
    config

let client = config |> AdminClientBuilder

let consumeTopic log handle cancellationToken config topic =
    task {
        let log: ILogger = log
        let topic: Topic = topic
        let config: ConsumerConfig = config
        let cancellationToken: CancellationToken = cancellationToken
        printf ("[Consumer] Initialized")
        
        let builder = ConsumerBuilder<string, string> config
        use consumer = builder.Build()
        consumer.Subscribe topic.Value
        
        try while not cancellationToken.IsCancellationRequested do
            try 
                let record = consumer.Consume cancellationToken
                do! handle log record
                
                
                consumer.StoreOffset record
            with
            | :? ConsumeException as e ->
                log.Warning(e, "Consuming... exception {name}", consumer.Name)
            | :? System.OperationCanceledException ->
                log.Warning("Consuming... cancelled {name}", consumer.Name)
        finally
            consumer.Close()
    }