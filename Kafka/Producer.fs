module Producer

open Confluent.Kafka
open Confluent.Kafka.Admin

let ensureTopic clientBuilder topicName =
    task {
        let clientBuilder : AdminClientBuilder = clientBuilder
        let topicName : string = topicName

        let specification = TopicSpecification ()

        specification.Name <- topicName
        specification.NumPartitions <- 1
        specification.ReplicationFactor <- int16 3

        use builder = clientBuilder.Build ()

        try
            do! builder.CreateTopicsAsync ([ specification ])
        with
        | :? CreateTopicsException as exn ->
            if exn.Results[0].Error.Code
               <> ErrorCode.TopicAlreadyExists then
                failwithf "[Producer] An error ocurred creating topic '%s': %s" topicName exn.Results[0].Error.Reason

        | exn -> failwithf "[Producer] An unknown error ocurred creating topic '%s': %s" topicName exn.Message
    }

let produceMessage clientConfig topicName message =
    let clientConfig : ClientConfig = clientConfig
    let topicName : string = topicName
    let message : Message<string, string> = message

    let builder = ProducerBuilder<string, string> clientConfig

    use producer = builder.Build ()

    let deliveryHandler =
        fun report ->
            let report : DeliveryReport<string, string> = report

            if report.Error.Code <> ErrorCode.NoError then
                failwithf "[Producer] Failed to deliver message: %s" report.Error.Reason

    producer.Produce (topicName, message, deliveryHandler)
    producer.Flush ()

    printfn "[Producer] Message sent successfully"
