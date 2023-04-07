using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Vox.Services.Discord.Extensions;
using Vox.Services.GTP;
using static Vox.Services.Extensions.ExceptionExtensions;

namespace Vox.Services.Discord.Interactions.SlashCommands;

// ReSharper disable once InconsistentNaming
public class AskGPT : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IConfiguration _config;
    private const int MaxFieldLength = 1024;

    /// <inheritdoc />
    public AskGPT(IConfiguration config)
    {
        _config = config;
    }

    [SlashCommand("ask-gpt", "Send a message to ChatGTP")]
    public async Task Execute([Summary("message", "Your message")] string content)
    {
        await DeferAsync(true);

        GPTRepository.UserMessages.TryGetValue(Context.User.Id, out var userMessages);
        if (userMessages is null) userMessages = new List<Message>();

        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add(
            "Authorization",
            $"Bearer {_config.GetValue<string>("ChatGPT_ApiKey")}");

        if (content.Length < 1)
        {
            throw new ExpectedException("Your message can not be empty");
        }

        var message = new Message { Role = "user", Content = content };
        userMessages.Add(message);
        var requestData = new Request { ModelId = "gpt-3.5-turbo", Messages = userMessages };

        using var response = await httpClient.PostAsJsonAsync(
            _config.GetValue<string>("ChatGPT_Endpoint"),
            requestData);

        if (!response.IsSuccessStatusCode)
        {
            throw new ExpectedException($"{response.StatusCode}");
        }

        var responseData = await response.Content.ReadFromJsonAsync<ResponseData>();
        var choices = responseData?.Choices ?? new List<Choice>();

        if (choices.Count == 0)
        {
            throw new ExpectedException("No choices were return by the API");
        }

        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithAuthor("Ask ChatGTP", Context.User.GetAvatarUrl())
            .AddField("Prompt", content);

        var responseText = choices[0].Message.Content.Trim();
        var chunks = new List<string>();

        // Split the response text into chunks of maxFieldLength characters or less
        for (var i = 0; i < responseText.Length; i += MaxFieldLength)
        {
            chunks.Add(responseText.Substring(i, Math.Min(MaxFieldLength, responseText.Length - i)));
        }

        for (var i = 0; i < chunks.Count; i++)
        {
            embed.AddField($"Response {i + 1}/{chunks.Count}", chunks[i]);
        }

        await FollowupAsync(embed: embed.Build());
        GPTRepository.UserMessages[Context.User.Id] = userMessages;
    }
}
