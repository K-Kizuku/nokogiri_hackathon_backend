using System;
using nokogiri_hackathon_backend.Models;

namespace nokogiri_hackathon_backend.Services
{
	public class ChatServices
	{
		public ChatServices()
		{
		}
        static async Task ProcessLoopAsync(ClientContext context, string message)
        {
            string? input = null;
            do
            {
                input = message;
                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                if (input.StartsWith("/exit") &&
                    AnsiConsole.Confirm("Do you really want to exit?"))
                {
                    break;
                }

                var firstTwoCharacters = input[..2];
                if (firstTwoCharacters is "/n")
                {
                    context = context with { UserName = input.Replace("/n", "").Trim() };
                    AnsiConsole.MarkupLine(
                        "[dim][[STATUS]][/] Set username to [lime]{0}[/]", context.UserName);
                    continue;
                }

                if (firstTwoCharacters switch
                {
                    "/j" => JoinChannel(context, input.Replace("/j", "").Trim()),
                    "/l" => LeaveChannel(context),
                    _ => null
                } is Task<ClientContext> cxtTask)
                {
                    context = await cxtTask;
                    continue;
                }

                if (firstTwoCharacters switch
                {
                    "/h" => ShowCurrentChannelHistory(context),
                    "/m" => ShowChannelMembers(context),
                    _ => null
                } is Task task)
                {
                    await task;
                    continue;
                }

                await SendMessage(context, input);
            } while (input is not "/exit");
        }
    }
}

