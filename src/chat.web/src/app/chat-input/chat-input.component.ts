import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ChatService } from '../../services/chat.service';

@Component({
  selector: 'app-chat-input',
  imports: [FormsModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './chat-input.component.html',
  styleUrl: './chat-input.component.css',
})
export class ChatInputComponent {
  private chatService = inject(ChatService);
  busyReceiving = this.chatService.receivingResponse.asReadonly();
  enteredMessage = '';

  onSubmit() {
    this.chatService.sendMessageWithSignalR(this.enteredMessage);
    this.enteredMessage = '';
  }

  onCancel() {
    this.chatService.cancelMessageWithSignalR();
  }
}
