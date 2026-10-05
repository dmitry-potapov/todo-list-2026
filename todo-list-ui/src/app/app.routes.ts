import { Routes } from '@angular/router';
import { TodoList } from './pages/todo-list/todo-list';

export const routes: Routes = [
  { path: '', component: TodoList },
  { path: '**', redirectTo: '' }
];
