import {
  AfterViewChecked,
  Component,
  DestroyRef,
  ElementRef,
  HostListener,
  inject,
  signal,
  ViewChild,
} from '@angular/core';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ChatService } from '../../services/chat.service';
import { SignalRService } from '../../services/signalR.service';
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
  private signalRService = inject(SignalRService);
  messages = this.chatService.messages.asReadonly();
  isFetching = signal(true);
  private destroyRef = inject(DestroyRef);

  @HostListener('window:beforeunload', ['$event'])
  onBeforeUnload(event: BeforeUnloadEvent) {
    this.chatService.cancelMessageWithSignalR();
  }

  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }

  ngOnInit() {
    this.signalRService.startConnection();
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
