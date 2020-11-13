﻿using ConsoleBot.Bots;
using ConsoleBot.Clients.ExternalMessagingClient;
using ConsoleBot.Exceptions;
using ConsoleBot.Helpers;
using ConsoleBot.Mule;
using D2NG.Core;
using Serilog;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleBot.Bots
{
    public abstract class SingleClientBotBase
    {
        protected readonly BotConfiguration _config;
        protected readonly IExternalMessagingClient _externalMessagingClient;
        protected readonly IMuleService _muleService;
        protected bool NeedsMule = false;

        public SingleClientBotBase(
            BotConfiguration config,
            IExternalMessagingClient externalMessagingClient,
            IMuleService muleService)
        {
            _config = config;
            _externalMessagingClient = externalMessagingClient;
            _muleService = muleService;
        }

        protected abstract Task<bool> RunSingleGame(Client client);

        protected async Task CreateGameLoop(Client client)
        {
            try
            {
                if (!RealmConnectHelpers.ConnectToRealm(client, _config.Realm, _config.KeyOwner, _config.GameFolder, _config.Username, _config.Password, _config.Character))
                {
                    throw new Exception("Could not connect to realm");
                }

                int totalCount = 0;
                int gameCount = 0;
                int successiveFailures = 0;
                int gameDescriptionIndex = 0;
                while (true)
                {
                    if(successiveFailures > 10 && totalCount > 15)
                    {
                        Log.Error($"bot stopping due to high successive failures: {successiveFailures} with run total {totalCount}");
                        await _externalMessagingClient.SendMessage($"bot stopping due to high successive failures: {successiveFailures} with run total {totalCount}");
                        client.Disconnect();
                        break;
                    }
                    else if(successiveFailures > 5)
                    {
                        gameDescriptionIndex++;
                        if (gameDescriptionIndex == _config.GameDescriptions?.Count)
                        {
                            gameDescriptionIndex = 0;
                        }
                        await _externalMessagingClient.SendMessage($"Many successive failures, swithing GS to {_config.GameDescriptions?.ElementAtOrDefault(gameDescriptionIndex)} ");
                        bool reconnectResult = await RealmConnectHelpers.ConnectToRealmWithRetry(client, _config.Realm, _config.KeyOwner, _config.GameFolder, _config.Username, _config.Password, _config.Character, 10);
                        if (!reconnectResult)
                        {
                            await _externalMessagingClient.SendMessage($"Reconnect tries of 10 reached, restarting bot");
                            return;
                        }
                    }

                    if(gameCount >= 100)
                    {
                        gameCount = 1;
                    }

                    if(NeedsMule && await _muleService.MuleItemsForClient(client, _config))
                    {
                        NeedsMule = false;
                        await _externalMessagingClient.SendMessage($"{client.LoggedInUserName()}: finished mule");
                    }

                    try
                    {
                        gameCount++;
                        totalCount++;
                        if (client.CreateGame(_config.Difficulty, $"{_config.GameNamePrefix}{gameCount}", _config.GamePassword, _config.GameDescriptions?.ElementAtOrDefault(gameDescriptionIndex)))
                        {
                            if(!await RunSingleGame(client))
                            {
                                successiveFailures += 1;
                            }
                            else
                            {
                                successiveFailures = 0;
                            }
                        }
                        else
                        {
                            successiveFailures += 1;
                            Thread.Sleep(10000);
                        }

                        if (client.Game.IsInGame())
                        {
                            client.Game.LeaveGame();
                        }

                        if(!client.RejoinMCP())
                        {
                            throw new Exception("Rejoining MCP failed");
                        }
                    }
                    catch (Exception e)
                    {
                        gameDescriptionIndex++;
                        if(gameDescriptionIndex == _config.GameDescriptions?.Count)
                        {
                            gameDescriptionIndex = 0;
                        }

                        successiveFailures += 1;
                        Log.Warning($"Disconnecting client due to exception {e}, reconnecting to realm, game description is now: {_config.GameDescriptions?.ElementAtOrDefault(gameDescriptionIndex)}");
                        bool reconnectResult = await RealmConnectHelpers.ConnectToRealmWithRetry(client, _config.Realm, _config.KeyOwner, _config.GameFolder, _config.Username, _config.Password, _config.Character, 10);
                        if (!reconnectResult)
                        {
                            await _externalMessagingClient.SendMessage($"Reconnect tries of 10 reached, restarting bot");
                            return;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, $"Unhandled Exception: {e}");
                await _externalMessagingClient.SendMessage($"bot crashed with exception: {e}");
                client.Disconnect();
                throw e;
            }
        }
    }
}
