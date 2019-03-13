// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GSheetReader;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace CafeDeOntemBOT
{
    /// <summary>
    /// Represents a bot that processes incoming activities.
    /// For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
    /// This is a Transient lifetime service.  Transient lifetime services are created
    /// each time they're requested. For each Activity received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single turn, should be carefully managed.
    /// For example, the <see cref="MemoryStorage"/> object and associated
    /// <see cref="IStatePropertyAccessor{T}"/> object are created with a singleton lifetime.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
    public class CafeDeOntemBOTBot : IBot
    {
        private readonly CafeDeOntemBOTAccessors _accessors;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="conversationState">The managed conversation state.</param>
        /// <param name="loggerFactory">A <see cref="ILoggerFactory"/> that is hooked to the Azure App Service provider.</param>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1#windows-eventlog-provider"/>
        public CafeDeOntemBOTBot(ConversationState conversationState, ILoggerFactory loggerFactory)
        {
            if (conversationState == null)
            {
                throw new System.ArgumentNullException(nameof(conversationState));
            }

            if (loggerFactory == null)
            {
                throw new System.ArgumentNullException(nameof(loggerFactory));
            }

            _accessors = new CafeDeOntemBOTAccessors(conversationState)
            {
                CounterState = conversationState.CreateProperty<CounterState>(CafeDeOntemBOTAccessors.CounterStateName),
            };

            _logger = loggerFactory.CreateLogger<CafeDeOntemBOTBot>();
            _logger.LogTrace("Turn start.");
        }

        /// <summary>
        /// Every conversation turn for our Echo Bot will call this method.
        /// There are no dialogs used, since it's "single turn" processing, meaning a single
        /// request and response.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        /// <seealso cref="BotStateSet"/>
        /// <seealso cref="ConversationState"/>
        /// <seealso cref="IMiddleware"/>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Handle Message activity type, which is the main activity type for shown within a conversational interface
            // Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                //Identify command
                if (turnContext.Activity.Text.ToLower().Contains("p ") & (turnContext.Activity.Text.ToLower().Contains("café") || turnContext.Activity.Text.ToLower().Contains("cafe") || turnContext.Activity.Text.ToLower().Contains("filtro")))
                {
                    var parameters = turnContext.Activity.Text.ToLower().Split(" ").ToList();
                    parameters.RemoveAt(0);

                    if (parameters.Exists(x => x.Equals("cafe") || x.Equals("café")))
                    {
                        Reader reader = new Reader();
                        await turnContext.SendActivityAsync("Os próximos para trazerem café são: " + reader.GetInfo("cafe"));
                    }

                    if (parameters.Exists(x => x.Equals("filtro")))
                    {
                        Reader reader = new Reader();
                        await turnContext.SendActivityAsync("Os próximos para trazerem filtro são: " + reader.GetInfo("filtro"));
                    }

                    //await turnContext.SendActivityAsync(turnContext.Activity.Text);
                }
            }
            else
            {
                //await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected");
            }
        }
    }
}
