import {
  AfterViewChecked,
  Component,
  DestroyRef,
  ElementRef,
  inject,
  signal,
  ViewChild,
} from '@angular/core';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ChatService } from '../../services/chat.service';
import { ChatMessageComponent } from '../chat-message/chat-message.component';

@Component({
  selector: 'app-chat-container',
  imports: [ChatMessageComponent, MatProgressSpinnerModule, MatDividerModule],
  templateUrl: './chat-container.component.html',
  styleUrl: './chat-container.component.css',
})
export class ChatContainerComponent implements AfterViewChecked {
  @ViewChild('scrollContainer')
  private scrollContainer!: ElementRef<HTMLDivElement>;
  private chatService = inject(ChatService);
  messages = this.chatService.messages.asReadonly();
  isFetching = signal(true);
  private destroyRef = inject(DestroyRef);

  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }

  ngOnInit() {
    const subscription = this.chatService.loadPreviousMessages().subscribe({
      complete: () => {
        this.isFetching.set(false);
      },
    });
    this.destroyRef.onDestroy(() => {
      subscription.unsubscribe();
    });
  }

  scrollToBottom(): void {
    try {
      if (this.scrollContainer !== undefined) {
        const container = this.scrollContainer.nativeElement;
        container.scrollTop = container.scrollHeight;
      }
    } catch (error) {
      console.error('Scroll error:', error);
    }
  }
}
