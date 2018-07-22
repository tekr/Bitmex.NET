﻿using Bitmex.NET.Dtos;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Bitmex.NET.IntegrationTests.Tests
{
    [TestClass]
    [TestCategory("WebSocket")]
    public class BitmexApiSocketServiceTradesTests : BaseBitmexIntegrationTests<IBitmexApiSocketService>
    {
        [TestMethod]
        public void should_subscribe_on_trades()
        {
            try
            {
                // arrange
                var connected = Sut.Connect();

                // act
                IEnumerable<TradeDto> dtos = null;
                var dataReceived = new ManualResetEvent(false);
                Sut.Subscribe(BitmetSocketSubscriptions.CreateTradeSubsription(a =>
                {
                    dtos = a.Data;
                    dataReceived.Set();
                }));
                var received = dataReceived.WaitOne(TimeSpan.FromSeconds(20));

                // assert
                // no exception raised
                connected.Should().BeTrue();
                received.Should().BeTrue();
                dtos.Should().NotBeNull();
                dtos.Count().Should().BeGreaterThan(0);
            }
            catch (BitmexWebSocketLimitReachedException)
            {
                Assert.Inconclusive("connection limit reached");
            }
        }

        [TestMethod]
        public void should_subscribe_on_trades_with_arguments()
        {
            try
            {
                // arrange
                var connected = Sut.Connect();

                // act
                IEnumerable<TradeDto> dtos = null;
                var dataReceived = new ManualResetEvent(false);
                Sut.Subscribe(BitmetSocketSubscriptions.CreateTradeSubsription(a =>
                {
                    dtos = a.Data;
                    dataReceived.Set();
                }).WithArgs("XBTUSD"));
                var received = dataReceived.WaitOne(TimeSpan.FromSeconds(20));

                // assert
                // no exception raised
                connected.Should().BeTrue();
                received.Should().BeTrue();
                dtos.Should().NotBeNull();
                dtos.Count().Should().BeGreaterThan(0);
                dtos.All(a => a.Symbol == "XBTUSD").Should().BeTrue();
            }
            catch (BitmexWebSocketLimitReachedException)
            {
                Assert.Inconclusive("connection limit reached");
            }
        }

        [TestMethod]
        public void should_subscribe_on_trades1m_with_arguments()
        {
            try
            {
                // arrange
                var connected = Sut.Connect();

                // act
                IEnumerable<TradeBucketedDto> dtos = null;
                var dataReceived = new ManualResetEvent(false);
                Sut.Subscribe(BitmetSocketSubscriptions.CreateTradeBucket1MSubsription(a =>
                {
                    dtos = a.Data;
                    dataReceived.Set();
                }).WithArgs("XBTUSD"));
                var received = dataReceived.WaitOne(TimeSpan.FromSeconds(60));

                // assert
                // no exception raised
                connected.Should().BeTrue();
                received.Should().BeTrue();
                dtos.Should().NotBeNull();
                dtos.Count().Should().BeGreaterThan(0);
                dtos.All(a => a.Symbol == "XBTUSD").Should().BeTrue();
            }
            catch (BitmexWebSocketLimitReachedException)
            {
                Assert.Inconclusive("connection limit reached");
            }
        }
    }
}
