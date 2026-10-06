import { Component, computed, inject, signal, WritableSignal } from '@angular/core';
import { ListItem } from '../../models/ListItem';
import { ApiService } from '../../services/api-service';
import { toSignal } from '@angular/core/rxjs-interop';
import { form, FormField } from '@angular/forms/signals';

@Component({
  selector: 'app-todo-list',
  imports: [FormField],
  templateUrl: './todo-list.html',
  styleUrl: './todo-list.css',
})
export class TodoList {
  private readonly _apiService = inject(ApiService);

  private activeItem = signal<ListItem>({description:''});
  todoForm = form(this.activeItem);
  descriptionEmpty = computed<boolean>(() => !this.activeItem().description);

  items = signal<ListItem[]>([]);

  constructor() {
    this.reloadItems();
  }

  saveTodo() {
    console.log(this.activeItem());
    if (!this.activeItem().id) {
      this._apiService.CreateItem(this.activeItem()).subscribe(_ => {
        this.reloadItems();
      });
    }
    else {
      this._apiService.UpdateItem(this.activeItem()).subscribe(_ => {
        this.reloadItems();
      });
    }
  }

  startEditingTodo(item: ListItem) {
    this.activeItem.set(item);
  }

  deleteTodo(item: ListItem) {
    this._apiService.DeleteItem(item).subscribe(_ => this.reloadItems());
  }

  reloadItems() {
    this.clearSelection();
    this._apiService.GetAllItems().subscribe(items => this.items.set(items));
  }

  clearSelection() {
    this.activeItem.set({description:''});
  }
}
