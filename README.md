# AK Concept

A simple concept of [Apache Avro](https://avro.apache.org/) + [Apache Kafka](https://kafka.apache.org/) with F# 

## Running

1. Set `AK_SERVER`, `AK_USERNAME` and `AK_PASSWORD` environment variables
2. Build and run as usual

## Lint

We use Fantomas tool to lint our project  
Is highly recommended to setup it with `dotnet tool install --global --version 4.6.4 fantomas-tool` and a [`pre-commit` hook](https://github.com/fsprojects/fantomas/blob/master/docs/Documentation.md#a-git-pre-commit-hook-sample)

(Do not forget to give the `pre-commit` file executable permissions)

## License

This project is distributed under the [Apache License 2.0](LICENSE)
