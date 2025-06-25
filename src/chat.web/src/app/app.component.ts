import { Component, inject } from '@angular/core';
import { ErrorService } from '../services/error.service';
import { ChatContainerComponent } from './chat-container/chat-container.component';
import { ChatInputComponent } from './chat-input/chat-input.component';
import { ErrorModalComponent } from './error-modal/error-modal.component';
import { HeaderComponent } from './header/header.component';

@Component({
  selector: 'app-root',
  imports: [
    ChatContainerComponent,
    HeaderComponent,
    ChatInputComponent,
    ErrorModalComponent,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'chat.web';
  private errorService = inject(ErrorService);

  error = this.errorService.error;
}
