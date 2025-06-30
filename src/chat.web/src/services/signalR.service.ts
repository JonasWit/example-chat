import { inject, Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import {
  GeneratedMessageFromServer,
  Score,
  StreamCompletedNotification,
} from '../models/message.model';
import { ChatService } from './chat.service';

@Injectable({ providedIn: 'root' })
export class SignalRService {
  private hubConnection!: HubConnection;
  private chatService = inject(ChatService);

  public startConnection(): void {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('http://localhost:5000/bot-hub')
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('SignalR Connected!');
        this.addMessageListener();
        this.addStreamCompletedListener();
      })
      .catch((err) => console.error('SignalR Connection Error:', err));
  }

  public addMessageListener(): void {
    this.hubConnection.on(
      'ResponseGenerated',
      (message: GeneratedMessageFromServer) => {
        if (this.chatService.currentBotResponse === undefined) {
          this.chatService.receivingResponse.set(true);
          this.chatService.currentBotResponse = {
            id: 0,
            sentBy: 'Bot',
            text: message.text,
            score: Score.NotSet,
          };
          this.chatService.messages.set([
            ...this.chatService.messages(),
            this.chatService.currentBotResponse,
          ]);
        } else {
          this.chatService.currentBotResponse.text =
            this.chatService.currentBotResponse.text + ' ' + message.text;
        }
      }
    );
  }

  public addStreamCompletedListener(): void {
    this.hubConnection.on(
      'StreamCompleted',
      (message: StreamCompletedNotification) => {
        this.chatService.receivingResponse.set(false);
        console.log('stream completed message received', message);
        if (this.chatService.currentBotResponse !== undefined) {
          this.chatService.currentBotResponse.id = message.botMessageId;
          this.chatService.currentBotResponse.text = message.fullMessage;
          this.chatService.currentBotResponse = undefined;
        }
      }
    );
  }
}
