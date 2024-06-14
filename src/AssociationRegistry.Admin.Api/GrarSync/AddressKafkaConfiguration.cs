﻿namespace AssociationRegistry.Admin.Api.GrarSync;

using Configuration;
using Confluent.Kafka;
using System;

public class AddressKafkaConfiguration : ConsumerConfig
{
    public AddressKafkaConfiguration(GrarOptions.KafkaOptions options)
    {
        var groupId = $"{options.GroupId}-COMPETING_6";

        BootstrapServers = options.BootstrapServer;
        TopicPartition = new(options.TopicName, Partition.Any);
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
