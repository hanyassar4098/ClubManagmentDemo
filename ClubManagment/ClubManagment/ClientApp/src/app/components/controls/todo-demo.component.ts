

import { Component, OnInit, OnDestroy, Input, TemplateRef, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AuthService } from '../../services/auth.service';
import { AlertService, MessageSeverity, DialogType } from '../../services/alert.service';
import { AppTranslationService } from '../../services/app-translation.service';
import { LocalStoreManager } from '../../services/local-store-manager.service';
import { Utilities } from '../../services/utilities';
import { Service } from 'src/app/models/service.model';
import { AccountService } from 'src/app/services/account.service';



@Component({
  selector: 'app-todo-demo',
  templateUrl: './todo-demo.component.html',
  styleUrls: ['./todo-demo.component.scss']
})
export class TodoDemoComponent implements OnInit, OnDestroy {
  public static readonly DBKeyTodoDemo = 'todo-demo.todo_list';

  rows = [];
  rowsCache = [];
  columns = [];
  editing = {};
  //taskEdit: any = {};
  public taskEdit: Service = new Service();
  isDataLoaded = false;
  loadingIndicator = true;
  formResetToggle = true;
  _currentUserId: string;
  _hideCompletedTasks = false;

  @ViewChild('f')
  public form;

  get currentUserId() {
    if (this.authService.currentUser) {
      this._currentUserId = this.authService.currentUser.id;
    }

    return this._currentUserId;
  }


  set hideCompletedTasks(value: boolean) {

    if (value) {
      this.rows = this.rowsCache.filter(r => !r.completed);
    } else {
      this.rows = [...this.rowsCache];
    }


    this._hideCompletedTasks = value;
  }

  get hideCompletedTasks() {
    return this._hideCompletedTasks;
  }


  @Input()
  verticalScrollbar = false;


  @ViewChild('statusHeaderTemplate', { static: true })
  statusHeaderTemplate: TemplateRef<any>;

  @ViewChild('priceTemplate', { static: true })
  priceTemplate: TemplateRef<any>;

  @ViewChild('nameTemplate', { static: true })
  nameTemplate: TemplateRef<any>;

  @ViewChild('descriptionTemplate', { static: true })
  descriptionTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate', { static: true })
  actionsTemplate: TemplateRef<any>;

  @ViewChild('editorModal', { static: true })
  editorModal: ModalDirective;

  @ViewChild('indexTemplate', { static: true })
  indexTemplate: TemplateRef<any>;

  public isSaving = false;
  public showValidationErrors = false;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  constructor(
    private alertService: AlertService, private translationService: AppTranslationService, private localStorage: LocalStoreManager, private authService: AuthService,private accountService: AccountService) {
  }



  ngOnInit() {
    this.loadingIndicator = true;

    // this.fetch((data) => {
    //   this.refreshDataIndexes(data);
    //   this.rows = data;
    //   this.rowsCache = [...data];
    //   this.isDataLoaded = true;

    //   setTimeout(() => { this.loadingIndicator = false; }, 1500);
    // });
      this.loadData();
    


    const gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: 'index', name: '#', width: 40, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'price', name: gT('todoDemo.management.Price'), width: 100, headerTemplate: this.statusHeaderTemplate, cellTemplate: this.priceTemplate},
      { prop: 'name', name: gT('todoDemo.management.Task'), cellTemplate: this.nameTemplate, width: 200 },
      { prop: 'description', name: gT('todoDemo.management.Description'), cellTemplate: this.descriptionTemplate, width: 500 },
      //{ name: '', width: 80, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];
  }

  loadData(){
    this.accountService.getServices().subscribe(results => 
      this.onDataLoadSuccessful(results), error => this.onDataLoadFailed(error));
  }

  
  onDataLoadSuccessful(service: Service[]) {
    debugger;
    this.alertService.stopLoadingMessage();
    this.loadingIndicator = false;

    service.forEach((user, index) => {
      (user as any).index = index + 1;
    });

    this.rowsCache = [...service];
    this.rows = service;

  }
  
  onDataLoadFailed(error: any) {
    this.alertService.stopLoadingMessage();
    this.loadingIndicator = false;

    this.alertService.showStickyMessage('Load Error', `Unable to retrieve users from the server.\r\nErrors: "${Utilities.getHttpResponseMessages(error)}"`,
      MessageSeverity.error, error);
  }

  ngOnDestroy() {
    this.saveToDisk();
  }



  fetch(cb) {
    let data = this.getFromDisk();

    if (data == null) {
      setTimeout(() => {

        data = this.getFromDisk();

        if (data == null) {
          data = [
            { completed: true, important: true, name: 'Create visual studio extension', description: 'Create a visual studio VSIX extension package that will add this project as an aspnet-core project template' },
            { completed: false, important: true, name: 'Do a quick how-to writeup', description: '' },
            {
              completed: false, important: false, name: 'Create aspnet-core/Angular tutorials based on this project', description: 'Create tutorials (blog/video/youtube) on how to build applications (full stack)' +
                ' using aspnet-core/Angular. The tutorial will focus on getting productive with the technology right away rather than the details on how and why they work so audience can get onboard quickly.'
            },
          ];
        }

        cb(data);
      }, 1000);
    } else {
      cb(data);
    }
  }


  refreshDataIndexes(data) {
    let index = 0;

    for (const i of data) {
      i.$$index = index++;
    }
  }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r =>
      Utilities.searchArray(value, false, r.name, r.description) ||
      value === 'important' && r.important ||
      value === 'not important' && !r.important);
  }


  showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  addTask() {
    this.formResetToggle = false;

    setTimeout(() => {
      this.formResetToggle = true;

      this.taskEdit = new Service();

      this.editorModal.show();
    });
  }

  // save() {
  //   this.rowsCache.splice(0, 0, this.taskEdit);
  //   this.rows.splice(0, 0, this.taskEdit);
  //   this.refreshDataIndexes(this.rowsCache);
  //   this.rows = [...this.rows];

  //   this.saveToDisk();
  //   this.editorModal.hide();
  // }


  save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage('Saving changes...');

    this.accountService.newService(this.taskEdit).subscribe(service => this.saveSuccessHelper(service), error => this.saveFailedHelper(error));
   
  }

  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage('Save Error', 'The below errors occured whilst saving your changes:', MessageSeverity.error, error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback) {
      this.changesFailedCallback();
    }
  }

  private saveSuccessHelper(sr?: Service) {
    if (sr) {
      Object.assign(this.taskEdit, sr);
      this.rows
    }
  //  this.loadData();
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    this.alertService.showMessage('Success', `Service \"${this.taskEdit.name}\" was created successfully`, MessageSeverity.success);
    
    this.taskEdit = new Service();

    this.resetForm();


    if (this.changesSavedCallback) {
      this.changesSavedCallback();
    }
    this.editorModal.hide();
  }


  resetForm(replace = false) {
    

    if (!replace) {
      this.form.reset();
    } else {
      this.formResetToggle = false;

      setTimeout(() => {
        this.formResetToggle = true;
      });
    }
  }

  updateValue(event, cell, cellValue, row) {
    this.editing[row.$$index + '-' + cell] = false;
    this.rows[row.$$index][cell] = event.target.value;
    this.rows = [...this.rows];

    this.saveToDisk();
  }


  delete(row) {
    this.alertService.showDialog('Are you sure you want to delete the task?', DialogType.confirm, () => this.deleteHelper(row));
  }


  deleteHelper(row) {
    this.rowsCache = this.rowsCache.filter(item => item !== row);
    this.rows = this.rows.filter(item => item !== row);

    this.saveToDisk();
  }

  getFromDisk() {
    return this.localStorage.getDataObject(`${TodoDemoComponent.DBKeyTodoDemo}:${this.currentUserId}`);
  }

  saveToDisk() {
    if (this.isDataLoaded) {
      this.localStorage.saveSyncedSessionData(this.rowsCache, `${TodoDemoComponent.DBKeyTodoDemo}:${this.currentUserId}`);
    }
  }

  
}
