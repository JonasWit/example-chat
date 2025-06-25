import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { catchError, tap, throwError } from 'rxjs';
import {
  ChatMessage,
  MessagePart,
  ParsedNDJSON,
  Score,
} from '../models/message.model';
import { ErrorService } from './error.service';

@Injectable({ providedIn: 'root' })
export class ChatService {
  private errorService = inject(ErrorService);
  private httpClient = inject(HttpClient);
  outputStream = signal<string>('');
  messages = signal<ChatMessage[]>([]);
  receivingResponse = signal<boolean>(false);

  private abortController: AbortController | null = null;
  currentBotResponse: ChatMessage | undefined;

  changeBotMessageScore(message: ChatMessage, score: Score) {
    const prevScore = message.score;
    message.score = score;
    const request = {
      Id: message.id,
      Score: score,
    };
    this.messages.update((msgs) =>
      msgs.map((m) => (m.id === message.id ? { ...m, score: score } : m))
    );

    return this.httpClient
      .put('http://localhost:5000/ai-chat/rate-message', request)
      .pipe(
        catchError((error) => {
          message.score = prevScore;
          this.errorService.showError('Failed to change score');
          return throwError(() => {
            new Error('Failed to change score');
          });
        })
      );
  }

  loadPreviousMessages() {
    return this.fetchAllMessages(
      'http://localhost:5000/ai-chat/all-messages',
      'Error fetching Previous Messages'
    ).pipe(
      tap({
        next: (messages) => {
          this.messages.set(messages);
        },
      })
    );
  }

  private fetchAllMessages(url: string, errorMessage: string) {
    return this.httpClient.get<ChatMessage[]>(url, {}).pipe(
      catchError((error, obs) => {
        this.errorService.showError(
          'Could not fetch the previous conversation'
        );
        return throwError(() => {
          new Error('Could not fetch the previous conversation');
        });
      })
    );
  }

  cancelMessage() {
    if (this.abortController) {
      this.abortController.abort();
      this.abortController = null;
    }
  }

  isChatMessage(obj: any): obj is ChatMessage {
    const id = obj.id ?? obj.Id;
    const sentBy = obj.sentBy ?? obj.SentBy;
    const text = obj.text ?? obj.Text;
    const score = obj.score ?? obj.Score;

    return (
      typeof id === 'number' &&
      typeof sentBy === 'string' &&
      typeof text === 'string' &&
      typeof score === 'number'
    );
  }

  isChatChunk(obj: any): obj is MessagePart {
    return typeof obj.data === 'string';
  }

  parseNDJSON(input: string): ParsedNDJSON[] {
    const result: ParsedNDJSON[] = [];
    const lines = input.split('\n').filter((line) => line.trim() !== '');
    for (const line of lines) {
      try {
        const obj = JSON.parse(line);

        if (this.isChatMessage(obj)) {
          result.push({
            id: obj.id,
            sentBy: obj.sentBy,
            text: obj.text,
            score: obj.score,
          });
        } else if (this.isChatChunk(obj)) {
          result.push(obj);
        } else {
          console.warn('Unrecognized NDJSON object format:', obj);
        }
      } catch (error) {
        console.error('Error parsing line:', line, error);
      }
    }

    return result;
  }

  sendMessage(prompt: string) {
    this.abortController = new AbortController();

    fetch('http://localhost:5000/ai-chat/new-message', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ text: prompt }),
      signal: this.abortController.signal,
    })
      .then(async (res) => {
        const reader = res.body!.getReader();
        const decoder = new TextDecoder();

        this.receivingResponse.set(true);
        while (true) {
          const { value, done } = await reader.read();
          if (done) {
            break;
          }

          const chunk = decoder.decode(value, { stream: true });
          const parsedObjects = this.parseNDJSON(chunk);

          parsedObjects.forEach((po) => {
            if (this.isChatChunk(po)) {
              if (this.currentBotResponse !== undefined) {
                this.currentBotResponse.text =
                  this.currentBotResponse.text + ' ' + po.data;
              }
            } else {
              this.messages.set([...this.messages(), po]);
              if (po.sentBy === 'Bot') {
                this.currentBotResponse = po;
              }
            }
          });
        }
      })
      .catch((err) => {
        console.log('Fetch aborted', err);
        if (err.name === 'AbortError') {
          console.log('Fetch aborted');
        } else if (err.message.includes('NetworkError')) {
          this.errorService.showError('Network issue');
        } else {
          console.error('Streaming error:', err);
        }
      })
      .finally(() => {
        this.receivingResponse.set(false);
        console.log('Stream finished');
      });
  }
}
