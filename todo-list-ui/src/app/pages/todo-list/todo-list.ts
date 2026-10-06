import { Component, computed, inject, signal, WritableSignal } from '@angular/core';
import { ListItem } from '../../models/ListItem';
import { ApiService } from '../../services/api-service';
import { form, FormField } from '@angular/forms/signals';
import { Observer } from 'rxjs';

@Component({
  selector: 'app-todo-list',
  imports: [FormField],
  templateUrl: './todo-list.html',
  styleUrl: './todo-list.css',
})
export class TodoList {
  private readonly _apiService = inject(ApiService);

  private activeItem = signal<ListItem>({description:''});
  errorMessage = signal<string>('');
  todoForm = form(this.activeItem);
  descriptionEmpty = computed<boolean>(() => !this.activeItem().description);

  items = signal<ListItem[]>([]);

  updatePageObserver: Observer<ListItem> = {
    next: _ => this.reloadItems(),
    error: err => this.displayError(err.message),
    complete: () => {}
  };

  constructor() {
    this.reloadItems();
  }

  saveTodo() {
      (!this.activeItem().id 
        ? this._apiService.CreateItem(this.activeItem()) 
        : this._apiService.UpdateItem(this.activeItem()))
      .subscribe(this.updatePageObserver);
  }

  startEditingTodo(item: ListItem) {
    this.activeItem.set(item);
  }

  deleteTodo(item: ListItem) {
    this._apiService.DeleteItem(item)
      .subscribe(this.updatePageObserver);
  }

  reloadItems() {
    this.clearSelection();
    this._apiService.GetAllItems()
      .subscribe({
        next: items => this.items.set(items),
        error: (error) => this.displayError(error.message)
      });
  }

  clearSelection() {
    this.activeItem.set({description:''});
  }

  displayError(error: any) {
    this.errorMessage.set(
      `An error occurred. Please refresh the page or try again later:\n${error}`
    );
  }
}
