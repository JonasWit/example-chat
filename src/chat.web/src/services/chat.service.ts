import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { catchError, tap, throwError } from 'rxjs';
import { ChatMessage } from '../models/message.model';
import { ErrorService } from './error.service';

@Injectable({ providedIn: 'root' })
export class ChatService {
  private errorService = inject(ErrorService);
  private httpClient = inject(HttpClient);
  outputStream = signal<string>('');
  messages = signal<ChatMessage[]>([]);
  receivingResponse = signal<boolean>(false);

  currentBotResponse: ChatMessage | undefined;

  sendMessageWithSignalR(prompt: string) {
    fetch('http://localhost:5000/ai-chat/new-message-signalr', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ text: prompt }),
    })
      .then((response) => {
        if (!response.ok) throw new Error(`Server returned ${response.status}`);
        return response.json() as Promise<ChatMessage>;
      })
      .then((chatMsg: ChatMessage) => {
        this.messages.set([...this.messages(), chatMsg]);
      })
      .catch((err) => {
        console.log('Error on new-messsage', err);
      })
      .finally(() => {
        this.receivingResponse.set(false);
        console.log('Stream finished');
      });
  }

  cancelMessageWithSignalR() {
    fetch('http://localhost:5000/ai-chat/cancel-message', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
    })
      .catch((err) => {
        console.log('Error on cancel-message', err);
      })
      .finally(() => {
        this.receivingResponse.set(false);
        console.log('Stream finished');
      });
  }

  changeBotMessageScore(message: ChatMessage, score: string) {
    const prevScore = message.score;
    message.score = score;
    const request = {
      Id: message.id,
      Score: score,
    };
    this.messages.update((msgs) =>
      msgs.map((m) => (m.id === message.id ? { ...m, score: score } : m))
    );

    console.log('sending request', request.Score);

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
}
