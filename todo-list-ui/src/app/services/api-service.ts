import { Injectable } from '@angular/core';
import { ListItem } from '../models/ListItem';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  GetAllItems(): Observable<ListItem[]> {
    return of([
      { id: 1, description: "Test item 1!" },
      { id: 2, description: "Test item 2!" }
    ]);
  }
}
