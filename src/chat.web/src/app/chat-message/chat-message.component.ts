import { CommonModule } from '@angular/common';
import { Component, computed, DestroyRef, inject, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { ChatMessage, Score } from '../../models/message.model';
import { ChatService } from '../../services/chat.service';

@Component({
  selector: 'app-chat-message',
  imports: [MatIconModule, CommonModule],
  templateUrl: './chat-message.component.html',
  styleUrl: './chat-message.component.css',
})
export class ChatMessageComponent {
  private chatService = inject(ChatService);
  busyReceiving = this.chatService.receivingResponse.asReadonly();
  message = input.required<ChatMessage>();
  private destroyRef = inject(DestroyRef);

  changeScore(score: string) {
    const subscription = this.chatService
      .changeBotMessageScore(this.message(), score)
      .subscribe({
        next: (resData) => console.log(resData),
      });

    this.destroyRef.onDestroy(() => {
      subscription.unsubscribe();
    });
  }

  isUpvoted = computed(() => {
    return this.message().score === Score.Upvote;
  });

  isDownvoted = computed(() => {
    return this.message().score === Score.Downvote;
  });

  isBotGenerated = computed(() => {
    return this.message().sentBy === 'Bot';
  });
}
