import { inject, Injectable } from '@angular/core';
import { ListItem } from '../models/ListItem';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private http = inject(HttpClient);

  GetAllItems(): Observable<ListItem[]> {
    return this.http.get<ListItem[]>(`${environment.apiUrl}`);
  }

  CreateItem(item: ListItem): Observable<ListItem> {
    return this.http.post<ListItem>(`${environment.apiUrl}`, item);
  }

  UpdateItem(item: ListItem): Observable<ListItem> {
    return this.http.put<ListItem>(`${environment.apiUrl}/${item.id}`, item);
  }

  DeleteItem(item: ListItem): Observable<any> {
    return this.http.delete<ListItem[]>(`${environment.apiUrl}/${item.id}`);
  }
}
