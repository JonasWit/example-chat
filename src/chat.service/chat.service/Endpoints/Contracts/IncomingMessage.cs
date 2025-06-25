using System.Net.Mime;
using chat.service.Data;

namespace chat.service.Endpoints.Contracts;

public record ScoreChangeRequest(long Id, int Score);
public record IncomingMessage(string Text);