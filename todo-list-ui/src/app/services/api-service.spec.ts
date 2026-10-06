import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ApiService } from './api-service';
import { environment } from '../../environments/environment';
import { ListItem } from '../models/ListItem';

describe('ApiService', () => {
  let service: ApiService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(ApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('GetAllItems should GET the collection', () => {
    const items: ListItem[] = [{ id: 1, description: 'a' }];
    let result: ListItem[] | undefined;
    service.GetAllItems().subscribe(r => (result = r));

    const req = httpMock.expectOne(environment.apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush(items);
    expect(result).toEqual(items);
  });

  it('CreateItem should POST the item', () => {
    const item: ListItem = { description: 'new' };
    const created: ListItem = { id: 5, description: 'new' };
    let result: ListItem | undefined;
    service.CreateItem(item).subscribe(r => (result = r));

    const req = httpMock.expectOne(environment.apiUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(item);
    req.flush(created);
    expect(result).toEqual(created);
  });

  it('UpdateItem should PUT the item to its id url', () => {
    const item: ListItem = { id: 7, description: 'upd' };
    let result: ListItem | undefined;
    service.UpdateItem(item).subscribe(r => (result = r));

    const req = httpMock.expectOne(`${environment.apiUrl}/7`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(item);
    req.flush(item);
    expect(result).toEqual(item);
  });

  it('DeleteItem should DELETE the item by id', () => {
    const item: ListItem = { id: 9, description: 'del' };
    let completed = false;
    service.DeleteItem(item).subscribe({ complete: () => (completed = true) });

    const req = httpMock.expectOne(`${environment.apiUrl}/9`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
    expect(completed).toBe(true);
  });
});
