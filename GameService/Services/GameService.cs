﻿using System;
using System.Threading.Tasks;
using GameService.Models;
using GameService.Interface;
using SharedClasses;
using GameService.Messaging;
using SharedClasses.Interface;

namespace GameService.Services
{
    public class GameService : IGameService
    {
        private IGame game;
        private IMessagePublisher publisher;

        public GameService(IGame game, IMessagePublisher messagePublisher)
        {
            this.game = game ?? throw new ArgumentNullException(nameof(game));
            this.publisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
        }

        public async Task<WalletUpdateMessage> PlayBetAsync(BetMessage betMessage)
        {
            var bet = new Bet(betMessage.Amount);
            var gameOutcome = this.game.PlayGame(bet);

            if (gameOutcome.isWin)
            {
                await this.publisher.PublishMessageAsync<WalletUpdateMessage>(
                    new WalletUpdateMessage
                    {
                        Amount = gameOutcome.amount,
                        IsDeposit = true,
                        IsBet = true
                    },
                    "walletQueue"
                );
            }
            else
            {
                var message = new WalletBalanceUpdateMessage
                {
                    CurrentBalance = betMessage.CurrentBalance,
                    IsSuccessful = true,
                    Message = $"No luck this time! Your current balance is {betMessage.CurrentBalance}"
                };

                await this.publisher.PublishMessageAsync<WalletBalanceUpdateMessage>(message, "gameOutcomeQueue");
            }

            return new WalletUpdateMessage
            {
                Amount = gameOutcome.amount,
                IsDeposit = gameOutcome.isWin
            };
        }
    }
}
