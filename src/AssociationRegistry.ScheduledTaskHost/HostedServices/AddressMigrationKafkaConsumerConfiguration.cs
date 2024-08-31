﻿namespace AssociationRegistry.ScheduledTaskHost.HostedServices;

using AssociationRegistry.Hosts.Configuration;
using Confluent.Kafka;

public class AddressMigrationKafkaConsumerConfiguration : ConsumerConfig
{
    public AddressMigrationKafkaConsumerConfiguration(GrarOptions.KafkaOptions options)
    {
        var groupId = options.GroupId;

        BootstrapServers = options.BootstrapServer;
        TopicPartition = new TopicPartition(options.TopicName, Partition.Any);
        SaslUsername = options.Username;
        SaslPassword = options.Password;
        GroupId = groupId;
        GroupInstanceId = DateTime.UtcNow.Ticks.ToString();
        Offset = options.Offset;

        SslEndpointIdentificationAlgorithm = Confluent.Kafka.SslEndpointIdentificationAlgorithm.Https;
        SecurityProtocol = Confluent.Kafka.SecurityProtocol.SaslSsl;
        SaslMechanism = Confluent.Kafka.SaslMechanism.Plain;
        AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
        EnableAutoCommit = false;
    }

    public TopicPartition TopicPartition { get; init; }
    public long Offset { get; set; }
}
