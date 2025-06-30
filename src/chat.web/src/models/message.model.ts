export interface ChatMessage {
  id: number;
  sentBy: string;
  text: string;
  score: string;
}

export interface MessagePart {
  data: string;
}

export enum Score {
  NotSet = 'NotSet',
  Downvote = 'Downvote',
  Upvote = 'Upvote',
}

export interface GeneratedMessageFromServer {
  text: string;
}

export interface StreamCompletedNotification {
  botMessageId: number;
  fullMessage: string;
}
