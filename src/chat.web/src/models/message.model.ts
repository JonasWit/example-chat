export interface ChatMessage {
  id: number;
  sentBy: string;
  text: string;
  score: number;
}

export interface MessagePart {
  data: string;
}

export type ParsedNDJSON = ChatMessage | MessagePart;

export enum Score {
  NotSet = 0,
  Downvote = 1,
  Upvote = 2,
}
