import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  inject,
} from "@angular/core";
import { CategoryFormComponent } from "./ui/category-form.component";
import { CategoryListComponent } from "./ui/category-list.component";
import { CategoryModel } from "./category.model";
import { provideComponentStore } from "@ngrx/component-store";
import { CategoryStore } from "./category.store";
import { AsyncPipe } from "@angular/common";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { NotificationService } from "../shared/notification.service";
import { CategoryFilterComponent } from "./ui/category-filter.component";

@Component({
  selector: "app-category",
  imports: [
    CategoryFormComponent,
    CategoryListComponent,
    AsyncPipe,
    MatProgressSpinnerModule,
    CategoryFilterComponent,
  ],
  styles: [``],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [provideComponentStore(CategoryStore)],
  template: `
    @if(vm$ | async; as vm)
    {
    <div style="position: relative;">
      
    @if(vm.loading){
      <div class="spinner-center">
        <mat-spinner diameter="50"></mat-spinner>
      </div>
    }
      

        @if(vm$ | async; as vm)
        {
        <h1>Add/Update category</h1>

        <app-category-form
          [categories]="vm.categories"
          [updateFormData]="categoryToUpdate"
          (submit)="onSubmit($event)"
          (reset)="onReset()"
        />
        
        <app-category-filter (filter)="onFilter($event)" />
        @if(vm.categories && vm.categories.length > 0)
          {
        <h1 style="margin-top:15px">Categories</h1>   

        <app-category-list
          [categories]="vm.categories"
          (edit)="onEdit($event)"
          (delete)="onDelete($event)"
        />
          }
          @else{
<p style="margin-top:20px;font-size:21px">No record(s) found</p>
          }
        }
    </div>
        }
  `
})
export class CategoryComponent implements OnInit {
  categoryStore = inject(CategoryStore);
  notificationService = inject(NotificationService);
  vm$ = this.categoryStore.vm$;
  categoryToUpdate: CategoryModel | null = null;

  onSubmit(category: CategoryModel) {
    if (typeof category.categoryId === "string") category.categoryId = null;
    if (category.id > 0) this.categoryStore.updateCategory(category);
    else {
      category.id = 0;
      this.categoryStore.saveCategory(category);
    }
    this.notificationService.send({
      message: "saved successfully",
      severity: "success",
    });
    this.categoryToUpdate = null;
  }
  onReset() {
    this.categoryToUpdate = null;
  }

  onEdit(category: CategoryModel) {
    this.categoryToUpdate = category;
  }

  onDelete(category: CategoryModel) {
    if (confirm("Are you sure?")) {
      this.categoryStore.deleteCategory(category.id);
      this.notificationService.send({
        message: "Deleted successfully",
        severity: "success",
      });
    }
  }

  onFilter(searchTerm: string) {
    this.categoryStore.setSearchTerm(searchTerm);
    // console.log(searchTerm);
  }

  ngOnInit() { }
}
