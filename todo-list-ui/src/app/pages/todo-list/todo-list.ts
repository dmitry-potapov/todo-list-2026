import { Component, inject } from '@angular/core';
import { ListItem } from '../../models/ListItem';
import { ApiService } from '../../services/api-service';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-todo-list',
  imports: [],
  templateUrl: './todo-list.html',
  styleUrl: './todo-list.css',
})
export class TodoList {
  private readonly _apiService = inject(ApiService);

  items = toSignal(this._apiService.GetAllItems(), { initialValue: [] });

  editItem(item: ListItem) {
    alert(`There we will edit item ${item.id}!`);
  }
}
