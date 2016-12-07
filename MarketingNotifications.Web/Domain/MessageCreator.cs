﻿using System;
using System.Threading.Tasks;
using MarketingNotifications.Web.Models;
using MarketingNotifications.Web.Models.Repository;

namespace MarketingNotifications.Web.Domain
{
    public class MessageCreator
    {
        private readonly ISubscribersRepository _repository;
        private const string Subscribe = "add";
        private const string Unsubscribe = "remove";
        private const string Start = "start";
        private const string Stop = "stop";

        public MessageCreator(ISubscribersRepository repository)
        {
            _repository = repository;
        }

        public async Task<String> Create(string phoneNumber, string message)
        {
            var subscriber = await _repository.FindByPhoneNumberAsync(phoneNumber);
            if (subscriber != null)
            {
                return await CreateOutputMessage(subscriber, message.ToLower());
            }

            subscriber = new Subscriber
            {
                PhoneNumber = phoneNumber,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Rv = true,
                Boat = true,
                Bridal = true,
            };
            await _repository.CreateAsync(subscriber);
            return "Thanks for contacting Greenband Enterprises! Text 'add' if you would to receive updates via text message.";
        }

        private async Task<string> CreateOutputMessage(Subscriber subscriber, string message)
        {
            if (!IsValidCommand(message))
            {
                return "Sorry, we don't recognize that command. Available commands are: 'add' or 'remove'.";
            }

            var isSubscribed = message.StartsWith(Subscribe) || message.StartsWith(Start);
            subscriber.Subscribed = isSubscribed;
            subscriber.UpdatedAt = DateTime.Now;
            await _repository.UpdateAsync(subscriber);

            return isSubscribed
                ? "You are now subscribed for updates."
                : "You have unsubscribed from notifications. Text 'add' to start receiving updates again";
        }

        private static bool IsValidCommand(string command)
        {
            return command.StartsWith(Subscribe) || command.StartsWith(Unsubscribe) || command.StartsWith(Start) || command.StartsWith(Stop);
        }
    }
}