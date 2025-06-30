namespace chat.service.Endpoints.Contracts;

public record ScoreChangeRequest(long Id, string Score);

public record IncomingMessage(string Text);