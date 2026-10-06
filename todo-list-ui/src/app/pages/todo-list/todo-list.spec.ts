import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { TodoList } from './todo-list';
import { ApiService } from '../../services/api-service';
import { ListItem } from '../../models/ListItem';

describe('TodoList', () => {
  let fixture: ComponentFixture<TodoList>;
  let component: TodoList;
  let el: HTMLElement;
  let api: {
    GetAllItems: ReturnType<typeof vi.fn>;
    CreateItem: ReturnType<typeof vi.fn>;
    UpdateItem: ReturnType<typeof vi.fn>;
    DeleteItem: ReturnType<typeof vi.fn>;
  };
  const items: ListItem[] = [
    { id: 1, description: 'first' },
    { id: 2, description: 'second' },
  ];

  const input = () => el.querySelector('input') as HTMLInputElement;
  const buttons = (label: string) =>
    Array.from(el.querySelectorAll('button')).filter(b => b.textContent?.trim() === label);
  const type = async (value: string) => {
    input().value = value;
    input().dispatchEvent(new Event('input'));
    await fixture.whenStable();
  };

  beforeEach(async () => {
    api = {
      GetAllItems: vi.fn(() => of(items)),
      CreateItem: vi.fn((i: ListItem) => of(i)),
      UpdateItem: vi.fn((i: ListItem) => of(i)),
      DeleteItem: vi.fn(() => of(null)),
    };
    await TestBed.configureTestingModule({
      imports: [TodoList],
      providers: [{ provide: ApiService, useValue: api }],
    }).compileComponents();

    fixture = TestBed.createComponent(TodoList);
    component = fixture.componentInstance;
    el = fixture.nativeElement;
    await fixture.whenStable();
  });

  it('should load and render items on creation', () => {
    expect(api.GetAllItems).toHaveBeenCalledTimes(1);
    expect(component.items()).toEqual(items);
    const lis = el.querySelectorAll('li');
    expect(lis.length).toBe(2);
    expect(lis[0].textContent).toContain('first');
  });

  it('should disable Save and Clear while description is empty', () => {
    expect(buttons('Save')[0].disabled).toBe(true);
    expect(buttons('Clear')[0].disabled).toBe(true);
  });

  it('should enable Save and Clear after typing', async () => {
    await type('hello');
    expect(buttons('Save')[0].disabled).toBe(false);
    expect(buttons('Clear')[0].disabled).toBe(false);
  });

  it('should create a new item when none is being edited', async () => {
    await type('brand new');
    buttons('Save')[0].click();
    expect(api.CreateItem).toHaveBeenCalledWith({ description: 'brand new' });
    expect(api.UpdateItem).not.toHaveBeenCalled();
    expect(api.GetAllItems).toHaveBeenCalledTimes(2);
    expect(component.descriptionEmpty()).toBe(true);
  });

  it('should load an item into the form when Edit is clicked', async () => {
    buttons('Edit')[1].click();
    await fixture.whenStable();
    expect(input().value).toBe('second');
  });

  it('should update an existing item when editing', async () => {
    buttons('Edit')[0].click();
    await fixture.whenStable();
    await type('first edited');
    buttons('Save')[0].click();
    expect(api.UpdateItem).toHaveBeenCalledWith({ id: 1, description: 'first edited' });
    expect(api.CreateItem).not.toHaveBeenCalled();
    expect(api.GetAllItems).toHaveBeenCalledTimes(2);
  });

  it('should delete an item and reload', () => {
    buttons('Delete')[0].click();
    expect(api.DeleteItem).toHaveBeenCalledWith(items[0]);
    expect(api.GetAllItems).toHaveBeenCalledTimes(2);
  });

  describe('error handling', () => {
    const errorBox = () => el.querySelector('.error');
    const failure = () => throwError(() => new Error('boom'));

    it('should not show an error initially', () => {
      expect(errorBox()).toBeNull();
      expect(component.errorMessage()).toBe('');
    });

    it('should show an error when loading items fails', async () => {
      api.GetAllItems.mockReturnValue(failure());
      component.reloadItems();
      await fixture.whenStable();
      expect(errorBox()?.textContent).toContain('An error occurred');
      expect(errorBox()?.textContent).toContain('boom');
    });

    it('should show an error and not reload when create fails', async () => {
      api.CreateItem.mockReturnValue(failure());
      await type('x');
      buttons('Save')[0].click();
      await fixture.whenStable();
      expect(errorBox()?.textContent).toContain('boom');
      expect(api.GetAllItems).toHaveBeenCalledTimes(1);
    });

    it('should show an error and not reload when update fails', async () => {
      api.UpdateItem.mockReturnValue(failure());
      buttons('Edit')[0].click();
      await fixture.whenStable();
      buttons('Save')[0].click();
      await fixture.whenStable();
      expect(errorBox()?.textContent).toContain('boom');
      expect(api.GetAllItems).toHaveBeenCalledTimes(1);
    });

    it('should show an error and not reload when delete fails', async () => {
      api.DeleteItem.mockReturnValue(failure());
      buttons('Delete')[0].click();
      await fixture.whenStable();
      expect(errorBox()?.textContent).toContain('boom');
      expect(api.GetAllItems).toHaveBeenCalledTimes(1);
    });
  });

  it('should reset the form when Clear is clicked', async () => {
    await type('something');
    buttons('Clear')[0].click();
    await fixture.whenStable();
    expect(component.descriptionEmpty()).toBe(true);
    expect(input().value).toBe('');
  });
});
